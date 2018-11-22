// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Threading;
using CausalityDbg.Core;

namespace CausalityDbg.Main
{
	sealed class TrackerWrapper : IDisposable
	{
		public TrackerWrapper(Dispatcher dispatcher, Config config)
		{
			_dispatcher = dispatcher;
			_config = config;
			_queue = new Queue<Action>();

			// * The callback needs to be registered on an MTA thread while the GUI
			//   thread needs to be an STA thread.
			// * The registering thread is not used for the actual callbacks, but
			//   terminating the thread while connected seems to break the ICorDebug
			//   objects.
			_thread = new Thread(ThreadBody);
			_thread.SetApartmentState(ApartmentState.MTA);
			_thread.IsBackground = true;
			_thread.Start();
		}

		public ITrackerStatus Status { get; set; }

		public void StartProcess(string process, string arguments, string directory, DataProvider store, NGenMode mode)
		{
			lock (_queue)
			{
				if (_disposed) throw new ObjectDisposedException(nameof(TrackerWrapper));
				if (Status == null) throw new InvalidOperationException("Status not set yet.");

				var callback = new Callback(_dispatcher, store, Status, this);

				_queue.Enqueue(delegate
				{
					Close(ref _tracker);
					_tracker = TrackerFactory.New(_config);

					IDictionary environment = null;

					if (mode == NGenMode.Dissable)
					{
						environment = Environment.GetEnvironmentVariables();
						environment["COMPLUS_ZapDisable"] = "1";
					}

					_tracker.Attach(process, arguments, directory, callback, environment, mode == NGenMode.Targeted);
				});

				Monitor.Pulse(_queue);
			}
		}

		public void SetProcess(int processID, DataProvider store)
		{
			lock (_queue)
			{
				if (_disposed) throw new ObjectDisposedException(nameof(TrackerWrapper));
				if (Status == null) throw new InvalidOperationException("Status not set yet.");

				var callback = new Callback(_dispatcher, store, Status, this);

				_queue.Enqueue(delegate
				{
					Close(ref _tracker);
					_tracker = TrackerFactory.New(_config);
					_tracker.Attach(processID, callback);
				});

				Monitor.Pulse(_queue);
			}
		}

		public void Detach()
		{
			lock (_queue)
			{
				if (_disposed) throw new ObjectDisposedException(nameof(TrackerWrapper));

				_queue.Enqueue(delegate { Close(ref _tracker); });
				Monitor.Pulse(_queue);
			}
		}

		public void Dispose()
		{
			lock (_queue)
			{
				if (_disposed) return;

				_disposed = true;
				_queue.Clear();
				_queue.Enqueue(delegate { Close(ref _tracker); });
				Monitor.Pulse(_queue);
			}

			var thread = _thread;
			_thread = null;
			thread.Join();
		}

		public event EventHandler<ExceptionThrownEventArgs> ExceptionThrown;

		void OnExceptionThrown(Exception ex)
		{
			ExceptionThrown?.Invoke(this, new ExceptionThrownEventArgs(ex));
		}

		static void Close(ref ITracker tracker)
		{
			if (tracker != null)
			{
				tracker.Dispose();
				tracker = null;
			}
		}

		void ReportException(Exception ex)
		{
			_dispatcher.BeginInvoke(
				DispatcherPriority.Normal,
				(Action<Exception>)OnExceptionThrown,
				ex);
		}

		void ThreadBody()
		{
			while (!_disposed)
			{
				Action action;

				lock (_queue)
				{
					while (_queue.Count == 0)
					{
						Monitor.Wait(_queue);
					}

					action = _queue.Dequeue();
				}

				try
				{
					action();
				}
				catch (Exception ex)
				{
					ReportException(ex);
				}
			}
		}

		class Callback : ITrackerCallback
		{
			public Callback(Dispatcher dispatcher, DataProvider provider, ITrackerStatus status, TrackerWrapper tracker)
			{
				_dispatcher = dispatcher;
				_setStatus = status.SetStatus;
				_notify = status.Notify;
				_newScope = provider.NewScope;
				_newEvent = provider.NewEvent;
				_closeScope = provider.CloseScope;
				_reportException = tracker.ReportException;
			}

			public void StartTracking(int pid)
			{
				_dispatcher.BeginInvoke(DispatcherPriority.Normal, _setStatus, pid);
			}

			public void EndTracking()
			{
				_dispatcher.BeginInvoke(DispatcherPriority.Normal, _setStatus, 0);
			}

			public void Notify(TrackerNotificationLevel level, string modulePath, string text)
			{
				_dispatcher.BeginInvoke(DispatcherPriority.Normal, _notify, level, modulePath, text);
			}

			public void NewScope(int scopeId, int? hostScopeId, int? triggerId, long fromTimestamp, DataItem item)
			{
				_dispatcher.BeginInvoke(DispatcherPriority.Normal, _newScope, scopeId, hostScopeId, triggerId, fromTimestamp, item);
			}

			public void NewEvent(int eventId, int hostScopeId, long timestamp, DataItem item)
			{
				_dispatcher.BeginInvoke(DispatcherPriority.Normal, _newEvent, eventId, hostScopeId, timestamp, item);
			}

			public void CloseScope(int scopeId, long toTimestamp)
			{
				_dispatcher.BeginInvoke(DispatcherPriority.Normal, _closeScope, scopeId, toTimestamp);
			}

			public void ReportException(Exception ex)
			{
				_dispatcher.BeginInvoke(DispatcherPriority.Normal, _reportException, ex);
			}

			readonly Dispatcher _dispatcher;
			readonly SetStatusDelegate _setStatus;
			readonly NotifyDelegate _notify;
			readonly NewScopeDelegate _newScope;
			readonly NewEventDelegate _newEvent;
			readonly CloseScopeDelegate _closeScope;
			readonly ReportExceptionDelegate _reportException;

			delegate void SetStatusDelegate(int pid);
			delegate void NotifyDelegate(TrackerNotificationLevel level, string modulePath, string text);
			delegate void NewScopeDelegate(int scopeId, int? hostScopeId, int? triggerId, long fromTimestamp, DataItem item);
			delegate void NewEventDelegate(int eventId, int hostScopeId, long timestamp, DataItem item);
			delegate void CloseScopeDelegate(int scopeId, long toTimestamp);
			delegate void ReportExceptionDelegate(Exception ex);
		}

		Thread _thread;
		bool _disposed;
		ITracker _tracker;
		readonly Config _config;
		readonly Dispatcher _dispatcher;
		readonly Queue<Action> _queue;
	}
}
