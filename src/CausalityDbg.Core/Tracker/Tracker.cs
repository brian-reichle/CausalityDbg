// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Threading;
using CausalityDbg.Configuration;
using CausalityDbg.Core.CorDebugApi;

namespace CausalityDbg.Core
{
	abstract partial class Tracker : ITracker
	{
		protected Tracker(Config config, ITrackerCallback callback)
		{
			_config = config ?? throw new ArgumentNullException(nameof(config));
			_trackerCallback = callback ?? throw new ArgumentNullException(nameof(callback));
			_shutdown = new ManualResetEvent(false);
		}

		public void Attach()
		{
			if (_thread != null)
			{
				throw new InvalidOperationException("Already attached.");
			}

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

		public void Dispose()
		{
			_shutdown.Set();
			_thread.Join();
			_shutdown.Dispose();
		}

		protected abstract ICorDebug CreateDebugger();
		protected abstract ICorDebugProcess CreateProcess(ICorDebug debugger);

		protected virtual void Start(ICorDebugProcess process)
		{
			_trackerCallback.StartTracking(process.GetID());
		}

		void ThreadBody()
		{
			try
			{
				ThreadBodyCore();
			}
			catch (Exception ex)
			{
				_trackerCallback.ReportException(ex);
			}
		}

		void ThreadBodyCore()
		{
			var callback = new ManagedCallback(this);

			var debugger = CreateDebugger();
			debugger.Initialize();
			debugger.SetManagedHandler(callback);

			ICorDebugProcess process;

			try
			{
				process = CreateProcess(debugger);
			}
			catch (Exception ex)
			{
				debugger.Terminate();

				if (ex is COMException comEx &&
					comEx.ErrorCode == (int)HResults.CORDBG_E_DEBUGGER_ALREADY_ATTACHED)
				{
					throw new AttachException(AttachErrorType.AlreadyAttached, ex);
				}

				throw;
			}

			_shutdown.WaitOne();

			try
			{
				process.Stop();

				callback.ClearRegistrations();

				while (process.HasQueuedCallbacks(null))
				{
					process.Continue(false);
					Thread.Sleep(0);
					process.Stop();
				}

				process.Detach();
			}
			catch (COMException ex) when (ex.HResult == (int)HResults.CORDBG_E_PROCESS_TERMINATED
				|| ex.HResult == (int)HResults.CORDBG_E_OBJECT_NEUTERED)
			{
			}

			debugger.Terminate();

			_trackerCallback.EndTracking();
		}

		void Terminated()
		{
			_shutdown.Set();
		}

		Thread _thread;

		readonly Config _config;
		readonly ITrackerCallback _trackerCallback;
		readonly ManualResetEvent _shutdown;
	}
}
