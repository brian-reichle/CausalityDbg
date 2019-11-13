// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;

namespace CausalityDbg.Core
{
#pragma warning disable SA1401 // Fields should be private
	static partial class NativeMethods
	{
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		public sealed class PROCESS_INFORMATION
		{
			/// <summary>
			/// A handle to the newly created process.
			/// The handle is used to specify the process in all functions that perform operations on the process object.
			/// </summary>
			public IntPtr hProcess;

			/// <summary>
			/// A handle to the primary thread of the newly created process.
			/// The handle is used to specify the thread in all functions that perform operations on the thread object.
			/// </summary>
			public IntPtr hThread;

			/// <summary>
			/// A value that can be used to identify a process.
			/// The value is valid from the time the process is created until all handles to the process are closed and the
			/// process object is freed; at this point, the identifier may be reused.
			/// </summary>
			public int dwProcessId;

			/// <summary>
			/// A value that can be used to identify a thread.
			/// The value is valid from the time the thread is created until all handles to the thread are closed and the
			/// thread object is freed; at this point, the identifier may be reused.
			/// </summary>
			public int dwThreadId;
		}
	}
#pragma warning restore SA1401 // Fields should be private
}
