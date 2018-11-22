// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

namespace CausalityDbg.Core
{
	sealed class ProcessHandle : SafeHandle
	{
		static readonly IntPtr NegativeOne = (IntPtr)(-1);

		ProcessHandle()
			: base(NegativeOne, true)
		{
		}

		public override bool IsInvalid => handle == NegativeOne || handle == IntPtr.Zero;

		[SuppressUnmanagedCodeSecurity]
		[PrePrepareMethod]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		protected override bool ReleaseHandle() => NativeMethods.CloseHandle(handle);
	}
}
