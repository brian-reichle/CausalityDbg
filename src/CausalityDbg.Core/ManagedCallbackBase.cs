// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using CausalityDbg.Core.CorDebugApi;

namespace CausalityDbg.Core
{
	abstract class ManagedCallbackBase : ICorDebugManagedCallback, ICorDebugManagedCallback2
	{
		protected ManagedCallbackBase()
		{
			_schedule = new Dictionary<ICorDebugThread, List<Action>>();
			_breakpointLookup = new Dictionary<ICorDebugFunctionBreakpoint, Action<ICorDebugThread>>();
			_stepperLookup = new Dictionary<ICorDebugStepper, Action<ICorDebugThread, CorDebugStepReason>>();
			_evalLookup = new Dictionary<ICorDebugEval, Action<ICorDebugEval, bool>>();
		}

		public void RegisterBreakpointAction(ICorDebugFunctionBreakpoint breakpoint, Action<ICorDebugThread> action)
		{
			_breakpointLookup.Add(breakpoint, action);
		}

		public void RegisterStepperAction(ICorDebugStepper stepper, Action<ICorDebugThread, CorDebugStepReason> action)
		{
			_stepperLookup.Add(stepper, action);
		}

		public void AbortStepper(ICorDebugStepper stepper)
		{
			_stepperLookup.Remove(stepper);
			stepper.Deactivate();
		}

		public void RegisterEvalAction(ICorDebugEval eval, Action<ICorDebugEval, bool> action)
		{
			_evalLookup.Add(eval, action);
		}

		public void ScheduleForEndOfThreadBatch(ICorDebugThread thread, Action action)
		{
			if (!thread.HasQueuedCallbacks())
			{
				action();
				return;
			}

			if (!_schedule.TryGetValue(thread, out var list))
			{
				list = new List<Action>();
				_schedule.Add(thread, list);
			}

			list.Add(action);
		}

		public void ClearRegistrations()
		{
			foreach (var pair in _breakpointLookup)
			{
				pair.Key.Activate(false);
				Marshal.FinalReleaseComObject(pair.Key);
			}

			foreach (var pair in _stepperLookup)
			{
				pair.Key.Deactivate();
				Marshal.FinalReleaseComObject(pair.Key);
			}

			_breakpointLookup.Clear();
			_stepperLookup.Clear();
		}

		protected abstract void LoadModuleCore(
			ICorDebugAppDomain appDomain,
			ICorDebugModule module);

		protected abstract void CreateThreadCore(
			ICorDebugAppDomain appDomain,
			ICorDebugThread thread);

		protected abstract void ExitThreadCore(
			ICorDebugAppDomain appDomain,
			ICorDebugThread thread);

		protected abstract void BreakCore(
			ICorDebugAppDomain appDomain,
			ICorDebugThread thread);

		protected abstract void ExceptionCore(
			ICorDebugAppDomain appDomain,
			ICorDebugThread thread,
			ICorDebugFrame frame,
			uint offset,
			CorDebugExceptionCallbackType eventType,
			CorDebugExceptionFlags flags);

		protected abstract void LogMessageCore(
			ICorDebugAppDomain appDomain,
			ICorDebugThread thread,
			int level,
			string logSwitchName,
			string message);

		protected abstract void CreateProcessCore(
			ICorDebugProcess process);

		protected abstract void ExitProcessCore(
			ICorDebugProcess process);

		protected abstract void BreakpointSetErrorCore(
			ICorDebugAppDomain appDomain,
			ICorDebugThread thread,
			ICorDebugBreakpoint breakpoint,
			int error);

		#region ICorDebugManagedCallback Members

		public void Breakpoint(
			ICorDebugAppDomain appDomain,
			ICorDebugThread thread,
			ICorDebugBreakpoint breakpoint)
		{
			try
			{
				if (breakpoint is ICorDebugFunctionBreakpoint functionBreakpoint &&
					_breakpointLookup.TryGetValue(functionBreakpoint, out var action))
				{
					action(thread);
				}
			}
			catch (Exception ex)
			{
				ReportException(ex);
			}

			ProcessEndOfBatch(thread);
			Continue(appDomain.GetProcess());
		}

		public void StepComplete(
			ICorDebugAppDomain appDomain,
			ICorDebugThread thread,
			ICorDebugStepper stepper,
			CorDebugStepReason reason)
		{
			try
			{
				if (_stepperLookup.TryGetValue(stepper, out var action))
				{
					_stepperLookup.Remove(stepper);
					action(thread, reason);
					Marshal.FinalReleaseComObject(stepper);
				}
			}
			catch (Exception ex)
			{
				ReportException(ex);
			}

			ProcessEndOfBatch(thread);
			Continue(appDomain.GetProcess());
		}

		public void Break(
			ICorDebugAppDomain appDomain,
			ICorDebugThread thread)
		{
			try
			{
				BreakCore(appDomain, thread);
			}
			catch (Exception ex)
			{
				ReportException(ex);
			}

			ProcessEndOfBatch(thread);
			Continue(appDomain.GetProcess());
		}

		public void Exception(
			ICorDebugAppDomain appDomain,
			ICorDebugThread thread,
			bool unhandled)
		{
			ProcessEndOfBatch(thread);
			Continue(appDomain.GetProcess());
		}

		public void EvalComplete(
			ICorDebugAppDomain appDomain,
			ICorDebugThread thread,
			ICorDebugEval eval)
		{
			try
			{
				if (_evalLookup.TryGetValue(eval, out var action))
				{
					_evalLookup.Remove(eval);
					action(eval, true);
					Marshal.FinalReleaseComObject(eval);
				}
			}
			catch (Exception ex)
			{
				ReportException(ex);
			}

			ProcessEndOfBatch(thread);
			Continue(appDomain.GetProcess());
		}

		public void EvalException(
			ICorDebugAppDomain appDomain,
			ICorDebugThread thread,
			ICorDebugEval eval)
		{
			try
			{
				if (_evalLookup.TryGetValue(eval, out var action))
				{
					_evalLookup.Remove(eval);
					action(eval, false);
					Marshal.FinalReleaseComObject(eval);
				}
			}
			catch (Exception ex)
			{
				ReportException(ex);
			}

			ProcessEndOfBatch(thread);
			Continue(appDomain.GetProcess());
		}

		public void CreateProcess(
			ICorDebugProcess process)
		{
			try
			{
				CreateProcessCore(process);
			}
			catch (Exception ex)
			{
				ReportException(ex);
			}

			Continue(process);
		}

		public void ExitProcess(
			ICorDebugProcess process)
		{
			try
			{
				ExitProcessCore(process);
			}
			catch (Exception ex)
			{
				ReportException(ex);
			}
		}

		public void CreateThread(
			ICorDebugAppDomain appDomain,
			ICorDebugThread thread)
		{
			try
			{
				CreateThreadCore(appDomain, thread);
			}
			catch (Exception ex)
			{
				ReportException(ex);
			}

			ProcessEndOfBatch(thread);
			Continue(appDomain.GetProcess());
		}

		public void ExitThread(
			ICorDebugAppDomain appDomain,
			ICorDebugThread thread)
		{
			try
			{
				ExitThreadCore(appDomain, thread);
			}
			catch (Exception ex)
			{
				ReportException(ex);
			}

			ProcessEndOfBatch(thread);
			Continue(appDomain.GetProcess());
		}

		public void LoadModule(
			ICorDebugAppDomain appDomain,
			ICorDebugModule module)
		{
			try
			{
				LoadModuleCore(appDomain, module);
			}
			catch (Exception ex)
			{
				ReportException(ex);
			}

			Continue(appDomain.GetProcess());
		}

		public void UnloadModule(
			ICorDebugAppDomain appDomain,
			ICorDebugModule module)
		{
			try
			{
				ReleaseModule(module);
			}
			catch (Exception ex)
			{
				ReportException(ex);
			}

			Continue(appDomain.GetProcess());
		}

		public void LoadClass(
			ICorDebugAppDomain appDomain,
			ICorDebugClass c)
		{
			Continue(appDomain.GetProcess());
		}

		public void UnloadClass(
			ICorDebugAppDomain appDomain,
			ICorDebugClass c)
		{
			Continue(appDomain.GetProcess());
		}

		public void DebuggerError(
			ICorDebugProcess process,
			int errorHR,
			int errorCode)
		{
			ReportException(Marshal.GetExceptionForHR(errorHR));
			ExitProcess(process);
		}

		public void LogMessage(
			ICorDebugAppDomain appDomain,
			ICorDebugThread thread,
			int level,
			string logSwitchName,
			string message)
		{
			try
			{
				LogMessageCore(appDomain, thread, level, logSwitchName, message);
			}
			catch (Exception ex)
			{
				ReportException(ex);
			}

			ProcessEndOfBatch(thread);
			Continue(appDomain.GetProcess());
		}

		public void LogSwitch(
			ICorDebugAppDomain appDomain,
			ICorDebugThread thread,
			int level,
			uint reason,
			string logSwitchName,
			string parentName)
		{
			ProcessEndOfBatch(thread);
			Continue(appDomain.GetProcess());
		}

		public void CreateAppDomain(
			ICorDebugProcess process,
			ICorDebugAppDomain appDomain)
		{
			appDomain.Attach();
			Continue(process);
		}

		public void ExitAppDomain(
			ICorDebugProcess process,
			ICorDebugAppDomain appDomain)
		{
			Continue(process);
		}

		public void LoadAssembly(
			ICorDebugAppDomain appDomain,
			ICorDebugAssembly assembly)
		{
			Continue(appDomain.GetProcess());
		}

		public void UnloadAssembly(
			ICorDebugAppDomain appDomain,
			ICorDebugAssembly assembly)
		{
			Continue(appDomain.GetProcess());
		}

		public void ControlCTrap(
			ICorDebugProcess process)
		{
			Continue(process);
		}

		public void NameChange(
			ICorDebugAppDomain appDomain,
			ICorDebugThread thread)
		{
			ProcessEndOfBatch(thread);

			if (appDomain != null)
			{
				Continue(appDomain.GetProcess());
			}
			else
			{
				Continue(thread.GetProcess());
			}
		}

		public void UpdateModuleSymbols(
			ICorDebugAppDomain appDomain,
			ICorDebugModule module,
			IStream symbolStream)
		{
			Continue(appDomain.GetProcess());
		}

		public void EditAndContinueRemap(
			ICorDebugAppDomain appDomain,
			ICorDebugThread thread,
			ICorDebugFunction function,
			bool accurate)
		{
			ProcessEndOfBatch(thread);
			Continue(appDomain.GetProcess());
		}

		public void BreakpointSetError(
			ICorDebugAppDomain appDomain,
			ICorDebugThread thread,
			ICorDebugBreakpoint breakpoint,
			int error)
		{
			try
			{
				BreakpointSetErrorCore(
					appDomain,
					thread,
					breakpoint,
					error);
			}
			catch (Exception ex)
			{
				ReportException(ex);
			}

			ProcessEndOfBatch(thread);
			Continue(appDomain.GetProcess());
		}

		#endregion

		#region ICorDebugManagedCallback2 Members

		public void FunctionRemapOpportunity(
			ICorDebugAppDomain appDomain,
			ICorDebugThread thread,
			ICorDebugFunction oldFunction,
			ICorDebugFunction newFunction,
			uint oldILOffset)
		{
			ProcessEndOfBatch(thread);
			Continue(appDomain.GetProcess());
		}

		public void CreateConnection(
			ICorDebugProcess process,
			int connectionId,
			string connName)
		{
			Continue(process);
		}

		public void ChangeConnection(
			ICorDebugProcess process,
			int connectionId)
		{
			Continue(process);
		}

		public void DestroyConnection(
			ICorDebugProcess process,
			int connectionId)
		{
			Continue(process);
		}

		public void Exception(
			ICorDebugAppDomain appDomain,
			ICorDebugThread thread,
			ICorDebugFrame frame,
			uint offset,
			CorDebugExceptionCallbackType eventType,
			CorDebugExceptionFlags flags)
		{
			try
			{
				ExceptionCore(appDomain, thread, frame, offset, eventType, flags);
			}
			catch (Exception ex)
			{
				ReportException(ex);
			}

			ProcessEndOfBatch(thread);
			Continue(appDomain.GetProcess());
		}

		public void ExceptionUnwind(
			ICorDebugAppDomain appDomain,
			ICorDebugThread thread,
			CorDebugExceptionCallbackType eventType,
			CorDebugExceptionFlags flags)
		{
			ProcessEndOfBatch(thread);
			Continue(appDomain.GetProcess());
		}

		public void FunctionRemapComplete(
			ICorDebugAppDomain appDomain,
			ICorDebugThread thread,
			ICorDebugFunction function)
		{
			ProcessEndOfBatch(thread);
			Continue(appDomain.GetProcess());
		}

		public void MDANotification(
			ICorDebugController controller,
			ICorDebugThread thread,
			IntPtr mda)
		{
			ProcessEndOfBatch(thread);

			var process = controller is ICorDebugAppDomain appDomain
				? appDomain.GetProcess()
				: (ICorDebugProcess)controller;

			Continue(process);
		}

		#endregion

		protected abstract void ReportException(Exception ex);

		void ProcessEndOfBatch(ICorDebugThread thread)
		{
			if (thread == null || thread.HasQueuedCallbacks())
			{
				return;
			}

			if (_schedule.TryGetValue(thread, out var list))
			{
				foreach (var action in list)
				{
					try
					{
						action();
					}
					catch (Exception ex)
					{
						ReportException(ex);
					}
				}

				_schedule.Remove(thread);
			}
		}

		static void Continue(ICorDebugProcess process)
		{
			process.Continue(false);
		}

		void ReleaseModule(ICorDebugModule module)
		{
			var forRemoval = new List<ICorDebugFunctionBreakpoint>();

			foreach (var breakpoint in _breakpointLookup.Keys)
			{
				if (breakpoint.GetFunction().GetModule() == module)
				{
					breakpoint.Activate(false);
					forRemoval.Add(breakpoint);
				}
			}

			foreach (var breakpoint in forRemoval)
			{
				_breakpointLookup.Remove(breakpoint);
				Marshal.FinalReleaseComObject(breakpoint);
			}
		}

		readonly Dictionary<ICorDebugThread, List<Action>> _schedule;
		readonly Dictionary<ICorDebugFunctionBreakpoint, Action<ICorDebugThread>> _breakpointLookup;
		readonly Dictionary<ICorDebugStepper, Action<ICorDebugThread, CorDebugStepReason>> _stepperLookup;
		readonly Dictionary<ICorDebugEval, Action<ICorDebugEval, bool>> _evalLookup;
	}
}
