// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

namespace CausalityDbg.Core
{
	sealed class UnmanagedBuffer : SafeBuffer
	{
		public UnmanagedBuffer(int size)
			: base(true)
		{
			if (size < 0) throw new ArgumentOutOfRangeException(nameof(size));
			SetHandle(Marshal.AllocCoTaskMem(size));
			Initialize(unchecked((ulong)size));
		}

		[SuppressUnmanagedCodeSecurity]
		[PrePrepareMethod]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		protected override bool ReleaseHandle()
		{
			Marshal.FreeCoTaskMem(handle);
			return true;
		}
	}
}
