// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

namespace CausalityDbg.Core
{
	static partial class NativeMethods
	{
		// BOOL CloseHandle(
		//     [in] HANDLE hObject
		// );
		[DllImport("kernel32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CloseHandle(
			IntPtr handle);

		// HANDLE OpenProcess(
		//     [in]  DWORD dwDesiredAccess,
		//     [in]  BOOL  bInheritHandle,
		//     [in]  DWORD dwProcessId
		// );
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern ProcessHandle OpenProcess(
			ProcessAccessOptions dwDesiredAccess,
			[MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,
			int dwProcessId);

		// BOOL WINAPI IsWow64Process(
		//     [in]   HANDLE      hProcess,
		//     [out]  PBOOL       Wow64Process
		// );
		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsWow64Process(
			IntPtr hProcess,
			[MarshalAs(UnmanagedType.Bool)] out bool Wow64Process);

		// BOOL WINAPI GetBinaryType(
		//     [in]   LPCTSTR     lpApplicationName,
		//     [out]  LPDWORD     lpBinaryType
		// );
		[DllImport("kernel32.dll", PreserveSig = true, SetLastError = true, CharSet = CharSet.Auto, BestFitMapping = false, ThrowOnUnmappableChar = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetBinaryType(
			[MarshalAs(UnmanagedType.LPTStr)] string lpApplicationName,
			out BinaryType lpBinaryType);

		// HANDLE WINAPI GetCurrentProcess();
		[DllImport("kernel32.dll")]
		public static extern IntPtr GetCurrentProcess();
	}
}
