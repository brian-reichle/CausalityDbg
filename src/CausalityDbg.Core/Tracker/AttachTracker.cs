// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using CausalityDbg.Core.CorDebugApi;

namespace CausalityDbg.Core
{
	sealed class AttachTracker : Tracker
	{
		public AttachTracker(Config config, ITrackerCallback callback, int pid)
			: base(config, callback)
		{
			if (pid == CorDebuggerHelper.GetCurrentPID()) throw new AttachException(AttachErrorType.AttachToSelf);
			if (BitnessHelper.IsProcess64Bit(pid) != Environment.Is64BitProcess) throw new AttachException(AttachErrorType.IncompatiblePlatforms);

			_pid = pid;
		}

		protected override ICorDebug CreateDebugger()
			=> CorDebuggerHelper.CreateDebuggingInterfaceForProcess(_pid);

		protected override ICorDebugProcess CreateProcess(ICorDebug debugger)
		{
			debugger.DebugActiveProcess((uint)_pid, false, out var process);
			return process;
		}

		readonly int _pid;
	}
}
