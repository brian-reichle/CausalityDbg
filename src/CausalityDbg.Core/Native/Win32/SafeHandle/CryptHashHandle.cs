// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

namespace CausalityDbg.Core
{
	sealed class CryptHashHandle : SafeHandle
	{
		CryptHashHandle()
			: base(IntPtr.Zero, true)
		{
		}

		public override bool IsInvalid => handle == IntPtr.Zero;

		[SuppressUnmanagedCodeSecurity]
		[PrePrepareMethod]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		protected override bool ReleaseHandle() => NativeMethods.CryptDestroyHash(handle);
	}
}
