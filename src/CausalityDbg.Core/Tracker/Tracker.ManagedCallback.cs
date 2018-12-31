// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using CausalityDbg.Configuration;
using CausalityDbg.Core.CorDebugApi;
using CausalityDbg.Metadata;

namespace CausalityDbg.Core
{
	partial class Tracker
	{
		sealed class ManagedCallback : ManagedCallbackBase
		{
			public ManagedCallback(Tracker tracker)
			{
				_config = tracker._config;
				_tracker = tracker;
				_trackerCallback = tracker._trackerCallback;
				_cache = new MetaProvider();
				_frameFactory = new FrameDataFactory(_cache);
				_eventMapper = new EventMapper();
			}

			protected override void LoadModuleCore(ICorDebugAppDomain appDomain, ICorDebugModule module)
			{
				HookModule(module);
			}

			protected override void CreateThreadCore(ICorDebugAppDomain appDomain, ICorDebugThread thread)
			{
				StartThreadScope(thread);
			}

			protected override void ExitThreadCore(ICorDebugAppDomain appDomain, ICorDebugThread thread)
			{
				EndThreadScope(thread);
			}

			protected override void BreakCore(ICorDebugAppDomain appDomain, ICorDebugThread thread)
			{
				Report(thread, thread.GetActiveFrame(), _tracker._config.BreakCategory, null);
			}

			protected override void ExceptionCore(ICorDebugAppDomain appDomain, ICorDebugThread thread, ICorDebugFrame frame, uint offset, CorDebugExceptionCallbackType eventType, CorDebugExceptionFlags flags)
			{
				if (eventType == CorDebugExceptionCallbackType.DEBUG_EXCEPTION_FIRST_CHANCE)
				{
					ReportException(thread, frame);
				}
			}

			protected override void LogMessageCore(ICorDebugAppDomain appDomain, ICorDebugThread thread, int level, string logSwitchName, string message)
			{
				ReportTrace(thread, message);
			}

			protected override void CreateProcessCore(ICorDebugProcess process)
			{
				_tracker.Start(process);
			}

			protected override void ExitProcessCore(ICorDebugProcess process)
			{
				_tracker.Terminated();
			}

			protected override void BreakpointSetErrorCore(ICorDebugAppDomain appDomain, ICorDebugThread thread, ICorDebugBreakpoint breakpoint, int error)
			{
				var function = ((ICorDebugFunctionBreakpoint)breakpoint).GetFunction();
				_tracker._trackerCallback.NotifyBreakpointFail(function.GetModule(), function.GetToken(), error);
			}

			protected override void ReportException(Exception ex)
			{
				_tracker._trackerCallback.ReportException(ex);
			}

			void HookupMethod(string className, ICorDebugClass type, ConfigMethodRef methodRef)
			{
				var matched = false;

				foreach (var function in type.FindFunctions(methodRef.Method))
				{
					matched = true;

					foreach (var hook in methodRef.Hooks)
					{
						var keyAccessor = GetKeyAccessor(function, className, methodRef.Method.Name, hook.Key);
						Action<ICorDebugThread> action;

						switch (hook.HookType)
						{
							case ConfigHookType.Event:
								action = thread => Report(thread, thread.GetActiveFrame(), hook.Category, keyAccessor);
								break;

							case ConfigHookType.Scope:
								action = thread => PushScope(thread, hook.Category, keyAccessor);
								break;

							default:
								continue;
						}

						var breakpoint = function.CreateBreakpoint();
						RegisterBreakpointAction(breakpoint, action);
						breakpoint.Activate(true);
					}
				}

				if (!matched)
				{
					_trackerCallback.NotifyMissingMethod(type.GetModule(), methodRef.Method.Name, className);
				}
			}

			void HookupClass(ICorDebugModule module, ConfigClass configClass)
			{
				var type = module.FindClass(configClass.ClassName);

				if (type == null)
				{
					_trackerCallback.NotifyMissingClass(module, configClass.ClassName);
				}
				else
				{
					HookupClass(type, configClass);
				}
			}

			void HookupClass(ICorDebugClass type, ConfigClass configClass)
			{
				foreach (var methodRef in configClass.Methods)
				{
					HookupMethod(configClass.FullClassName, type, methodRef);
				}

				foreach (var nestedClass in configClass.NestedClasses)
				{
					var nestedType = type.FindClass(nestedClass.ClassName);

					if (nestedType == null)
					{
						_trackerCallback.NotifyMissingNestedClass(type.GetModule(), configClass.FullClassName, nestedClass.ClassName);
					}
					else
					{
						HookupClass(nestedType, nestedClass);
					}
				}
			}

			void HookModule(ICorDebugModule module)
			{
				if (!module.IsDynamic())
				{
					var assemblyConfig = _config.FindAssembly(module);

					if (assemblyConfig != null)
					{
						TryDissableOptomisations(module);

						foreach (var cl in assemblyConfig.Classes)
						{
							HookupClass(module, cl);
						}
					}
				}
			}

			void TryDissableOptomisations(ICorDebugModule module)
			{
				var hr = ((ICorDebugModule2)module).SetJITCompilerFlags(CorDebugJITCompilerFlags.CORDEBUG_JIT_DISABLE_OPTIMIZATION);

				if (hr < 0)
				{
					if (hr == (int)HResults.CORDBG_E_CANT_CHANGE_JIT_SETTING_FOR_ZAP_MODULE)
					{
						_trackerCallback.NotifyIsZapModule(module);
					}
					else if (hr == (int)HResults.CORDBG_E_CANNOT_BE_ON_ATTACH)
					{
						_trackerCallback.NotifyModuleIsAttaching(module);
					}
					else
					{
						throw Marshal.GetExceptionForHR(hr);
					}
				}
			}

			void StepperExit(ICorDebugThread thread, ScopeData scope)
			{
				scope.Active = false;
				PurgeClosedScopes(thread);
			}

			void StepperExitExceptionScope(ICorDebugThread thread, ScopeData scope, CorDebugStepReason reason)
			{
				StepperExit(thread, scope);
				PushExceptionHandlerScopeAtEndOfBatch(thread, reason);
			}

			void Report(ICorDebugThread thread, ICorDebugFrame topFrame, ConfigCategory category, Func<ICorDebugILFrame, ICorDebugValue> keyAccessor)
			{
				if (_scopeIdLookup.TryGetValue(thread, out var scope))
				{
					var item = new DataItem(category, _frameFactory.GetTrace(scope.Trace, topFrame));
					var eventId = ReportCore(scope, item);

					if (keyAccessor != null &&
						keyAccessor((ICorDebugILFrame)topFrame) is ICorDebugReferenceValue keyValue)
					{
						_eventMapper.Set(category, keyValue, eventId);
					}
				}
			}

			void AbortCurrentExceptionTrace(ICorDebugThread thread)
			{
				if (_currentExceptionData.TryGetValue(thread, out var data))
				{
					AbortStepper(data.CurrentStepper);
					_currentExceptionData.Remove(thread);
				}
			}

			void ReportException(ICorDebugThread thread, ICorDebugFrame topFrame)
			{
				if (_scopeIdLookup.TryGetValue(thread, out var scope))
				{
					AbortCurrentExceptionTrace(thread);

					var exception = thread.GetCurrentException();
					var exceptionType = ((ICorDebugValue2)exception).GetExactType();
					var module = topFrame.GetFunction().GetModule();
					var type = _cache.GetCompound(module, exceptionType);

					var item = new ExceptionDataItem(
						_config.ExceptionCategory,
						_frameFactory.GetTrace(scope.Trace, topFrame),
						MetaFormatter.Format(type));

					var messageFunc = exceptionType.GetClass().FindPropertyGetterInherit("Message");
					var eval = thread.CreateEval();
					eval.CallFunction(messageFunc, 1, new ICorDebugValue[] { exception });

					RegisterEvalAction(eval, (e, s) =>
					{
						if (s)
						{
							var evalResult = (ICorDebugReferenceValue)e.GetResult();

							if (!evalResult.IsNull())
							{
								item.ExceptionMessage = ((ICorDebugStringValue)evalResult.Dereference()).GetStringValue();
							}
							else
							{
								item.ExceptionMessage = "<null>";
							}
						}
						else
						{
							item.ExceptionMessage = "<error getting value>";
						}
					});

					var eventID = ReportCore(scope, item);
					_eventMapper.Set(_config.ExceptionCategory, (ICorDebugReferenceValue)exception, eventID);

					var stepper = topFrame.CreateStepper();
					stepper.SetInterceptMask(CorDebugIntercept.INTERCEPT_EXCEPTION_FILTER);
					stepper.Step(true);

					_currentExceptionData.Add(thread, new ExceptionData(stepper));
					RegisterStepperAction(stepper, PushExceptionHandlerScopeAtEndOfBatch);
				}
			}

			void ReportTrace(ICorDebugThread thread, string text)
			{
				if (_scopeIdLookup.TryGetValue(thread, out var scope))
				{
					var item = new TraceDataItem(
						_config.TraceCategory,
						_frameFactory.GetTrace(scope.Trace, thread.GetActiveFrame()),
						text);

					ReportCore(scope, item);
				}
			}

			int ReportCore(ScopeData host, DataItem item)
			{
				var newEventId = Interlocked.Increment(ref _nextEventId) - 1;
				_trackerCallback.NewEvent(newEventId, host.ScopeId, Stopwatch.GetTimestamp(), item);
				return newEventId;
			}

			void StartThreadScope(ICorDebugThread thread)
			{
				if (!_scopeIdLookup.ContainsKey(thread))
				{
					var item = new DataItem(_config.ManagedThreadCategory, TraceData.Empty);
					var newScopeId = Interlocked.Increment(ref _nextScopeId) - 1;

					int? triggerID = null;

					if (thread.GetObject() is ICorDebugReferenceValue threadRef &&
						_eventMapper.TryGetValue(_config.ManagedThreadCategory, threadRef, out var tmp))
					{
						triggerID = tmp;
					}

					_trackerCallback.NewScope(newScopeId, null, triggerID, Stopwatch.GetTimestamp(), item);
					_scopeIdLookup.Add(thread, new ScopeData(null, null, newScopeId));
				}
			}

			void EndThreadScope(ICorDebugThread thread)
			{
				if (_scopeIdLookup.TryGetValue(thread, out var scope))
				{
					_trackerCallback.CloseScope(scope.ScopeId, Stopwatch.GetTimestamp());
					_scopeIdLookup.Remove(thread);
				}
			}

			void PushScope(ICorDebugThread thread, ConfigCategory category, Func<ICorDebugILFrame, ICorDebugValue> keyAccessor)
			{
				if (_scopeIdLookup.TryGetValue(thread, out var scope))
				{
					var item = new DataItem(category, _frameFactory.GetTrace(scope.Trace, thread.GetActiveFrame()));
					var newScopeId = Interlocked.Increment(ref _nextScopeId) - 1;

					int? triggerId = null;

					if (keyAccessor != null &&
						keyAccessor((ICorDebugILFrame)thread.GetActiveFrame()) is ICorDebugReferenceValue keyValue)
					{
						triggerId = GetEventId(category, keyValue);
					}

					_trackerCallback.NewScope(newScopeId, scope.ScopeId, triggerId, Stopwatch.GetTimestamp(), item);
					scope = new ScopeData(scope, item.StackTrace, newScopeId);
					_scopeIdLookup[thread] = scope;

					var stepper = thread.CreateStepper();
					stepper.StepOut();

					RegisterStepperAction(stepper, (t, r) => { StepperExit(t, scope); });
				}
			}

			void PushExceptionHandlerScopeAtEndOfBatch(ICorDebugThread thread, CorDebugStepReason reason)
			{
				// Delay starting the next scope until the end of the batch as there may be
				// other callbacks that need to close their scopes first. Failing to do so
				// could result in incorrect nesting.
				ScheduleForEndOfThreadBatch(thread, delegate
				{
					PushExceptionHandlerScope(thread, reason);
				});
			}

			void PushExceptionHandlerScope(ICorDebugThread thread, CorDebugStepReason reason)
			{
				if (!_scopeIdLookup.TryGetValue(thread, out var scope))
				{
					return;
				}

				var exception = (ICorDebugReferenceValue)thread.GetCurrentException();
				var frame = (ICorDebugILFrame)thread.GetActiveFrame();
				var ip = frame.GetIP();
				var function = frame.GetFunction();
				var clauses = function.GetExceptionClauses();
				var eventId = GetEventId(_config.ExceptionCategory, exception);
				var isFilter = reason == CorDebugStepReason.STEP_EXCEPTION_FILTER || reason == CorDebugStepReason.STEP_INTERCEPT;
				var isTerminal = false;

				int start;
				int end;
				ConfigCategory category;

				if (isFilter)
				{
					var clause = clauses.FromFilterOffset(ip.Value);
					start = clause.FilterOffset;
					end = clause.HandlerOffset;
					category = _config.FilterCategory;
				}
				else
				{
					var clause = clauses.FromHandlerOffset(ip.Value);
					start = clause.HandlerOffset;
					end = clause.HandlerOffset + clause.HandlerLength;

					switch (clause.Flags)
					{
						case ExceptionHandlingClauseOptions.Clause:
						case ExceptionHandlingClauseOptions.Filter:
							category = _config.CatchCategory;
							isTerminal = true;
							break;

						case ExceptionHandlingClauseOptions.Fault:
							category = _config.FaultCategory;
							break;

						case ExceptionHandlingClauseOptions.Finally:
							category = _config.FinallyCategory;
							break;

						default:
							throw new InvalidOperationException();
					}
				}

				var item = new DataItem(category, _frameFactory.GetTrace(scope.Trace, thread.GetActiveFrame()));
				var newScopeId = Interlocked.Increment(ref _nextScopeId) - 1;

				_trackerCallback.NewScope(newScopeId, scope.ScopeId, eventId, Stopwatch.GetTimestamp(), item);
				scope = new ScopeData(scope, item.StackTrace, newScopeId);
				_scopeIdLookup[thread] = scope;

				var stepper = thread.CreateStepper();
				stepper.StepRange(start, end);

				if (isTerminal)
				{
					_eventMapper.Remove(_config.ExceptionCategory, exception);
					RegisterStepperAction(stepper, (t, r) => StepperExit(t, scope));
					_currentExceptionData.Remove(thread);
				}
				else
				{
					RegisterStepperAction(stepper, (t, r) => StepperExitExceptionScope(t, scope, r));
					var exData = _currentExceptionData[thread];
					exData.CurrentStepper = stepper;
				}
			}

			void PurgeClosedScopes(ICorDebugThread thread)
			{
				if (_scopeIdLookup.TryGetValue(thread, out var scope))
				{
					while (scope != null && !scope.Active)
					{
						_trackerCallback.CloseScope(scope.ScopeId, Stopwatch.GetTimestamp());
						scope = scope.Old;
					}

					_scopeIdLookup[thread] = scope;
					_eventMapper.Purge();
				}
			}

			int? GetEventId(ConfigCategory category, ICorDebugReferenceValue key)
			{
				return _eventMapper.TryGetValue(category, key, out var tmp) ? tmp : new int?();
			}

			Func<ICorDebugILFrame, ICorDebugValue> GetKeyAccessor(ICorDebugFunction function, string className, string methodName, string key)
			{
				// It is very possible that some of the parameter types are defined in modules that have not
				// yet been loaded, so we cant use the metadata cache here.
				int paramIndex;

				if (string.IsNullOrEmpty(key))
				{
					return null;
				}
				else if (key == "this")
				{
					if ((function.GetCallingConventionLight() & CallingConventions.HasThis) != 0)
					{
						paramIndex = 0;
					}
					else
					{
						_trackerCallback.NotifyNoThis(function.GetModule(), className, methodName);
						return null;
					}
				}
				else if ((paramIndex = function.GetParamIndex(key)) < 0)
				{
					_trackerCallback.NotifyNoParam(function.GetModule(), className, methodName, key);
					return null;
				}

				return (ICorDebugILFrame frame) =>
				{
					var hr = frame.GetArgument(paramIndex, out var value);

					if (hr == (int)HResults.CORDBG_E_IL_VAR_NOT_AVAILABLE)
					{
						return null;
					}
					else if (hr < 0)
					{
						throw Marshal.GetExceptionForHR(hr);
					}
					else
					{
						return value;
					}
				};
			}

			int _nextEventId;
			int _nextScopeId;

			readonly Dictionary<ICorDebugThread, ScopeData> _scopeIdLookup = new Dictionary<ICorDebugThread, ScopeData>();
			readonly Dictionary<ICorDebugThread, ExceptionData> _currentExceptionData = new Dictionary<ICorDebugThread, ExceptionData>();
			readonly FrameDataFactory _frameFactory;
			readonly EventMapper _eventMapper;
			readonly MetaProvider _cache;

			readonly Config _config;
			readonly Tracker _tracker;
			readonly ITrackerCallback _trackerCallback;
		}
	}
}
