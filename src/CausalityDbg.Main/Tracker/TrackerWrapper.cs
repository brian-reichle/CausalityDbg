// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Windows.Threading;
using CausalityDbg.Configuration;
using CausalityDbg.Core;
using CausalityDbg.DataStore;

namespace CausalityDbg.Main
{
	sealed class TrackerWrapper : IDisposable
	{
		public TrackerWrapper(Dispatcher dispatcher, Config config)
		{
			_dispatcher = dispatcher;
			_config = config;
		}

		public ITrackerStatus Status { get; set; }

		public void StartProcess(LaunchArguments args, DataProvider store)
		{
			if (_disposed) throw new ObjectDisposedException(nameof(TrackerWrapper));
			if (Status == null) throw new InvalidOperationException("Status not set yet.");

			var callback = new Callback(_dispatcher, store, Status, this);

			Close(ref _tracker);

			try
			{
				_tracker = TrackerFactory.New(_config, callback, args);
				_tracker.Attach();
			}
			catch (AttachException ex)
			{
				_tracker?.Dispose();
				_tracker = null;
				ReportException(ex);
			}
		}

		public void SetProcess(int processID, DataProvider store)
		{
			if (_disposed) throw new ObjectDisposedException(nameof(TrackerWrapper));
			if (Status == null) throw new InvalidOperationException("Status not set yet.");

			var callback = new Callback(_dispatcher, store, Status, this);

			Close(ref _tracker);

			try
			{
				_tracker = TrackerFactory.New(_config, callback, processID);
				_tracker.Attach();
			}
			catch (AttachException ex)
			{
				_tracker?.Dispose();
				_tracker = null;
				ReportException(ex);
			}
		}

		public void Detach()
		{
			Close(ref _tracker);
		}

		public void Dispose()
		{
			_disposed = true;
			Close(ref _tracker);
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

		sealed class Callback : ITrackerCallback
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

		bool _disposed;
		ITracker _tracker;
		readonly Config _config;
		readonly Dispatcher _dispatcher;
	}
}
