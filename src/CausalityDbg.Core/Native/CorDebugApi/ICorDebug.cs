// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using static CausalityDbg.Core.NativeMethods;

namespace CausalityDbg.Core.CorDebugApi
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("3D6F5F61-7538-11D3-8D5B-00104B35E7EF")]
	interface ICorDebug
	{
		// HRESULT Initialize();
		void Initialize();

		// HRESULT Terminate();
		void Terminate();

		// HRESULT SetManagedHandler(
		//     [in] ICorDebugManagedCallback     *pCallback
		// );
		void SetManagedHandler(
			ICorDebugManagedCallback pCallback);

		// HRESULT SetUnmanagedHandler(
		//     [in] ICorDebugUnmanagedCallback  *pCallback
		// );
		void SetUnmanagedHandler_();

		// HRESULT CreateProcess(
		//     [in]  LPCWSTR                     lpApplicationName,
		//     [in]  LPWSTR                      lpCommandLine,
		//     [in]  LPSECURITY_ATTRIBUTES       lpProcessAttributes,
		//     [in]  LPSECURITY_ATTRIBUTES       lpThreadAttributes,
		//     [in]  BOOL                        bInheritHandles,
		//     [in]  DWORD                       dwCreationFlags,
		//     [in]  PVOID                       lpEnvironment,
		//     [in]  LPCWSTR                     lpCurrentDirectory,
		//     [in]  LPSTARTUPINFOW              lpStartupInfo,
		//     [in]  LPPROCESS_INFORMATION       lpProcessInformation,
		//     [in]  CorDebugCreateProcessFlags  debuggingFlags,
		//     [out] ICorDebugProcess            **ppProcess
		// );
		void CreateProcess(
			[MarshalAs(UnmanagedType.LPWStr)] string lpApplicationName,
			[MarshalAs(UnmanagedType.LPWStr)] string lpCommandLine,
			IntPtr lpProcessAttributes,
			IntPtr lpThreadAttributes,
			[MarshalAs(UnmanagedType.Bool)] bool bInheritHandles,
			ProcessCreationFlags dwCreationFlags,
			IntPtr lpEnvironment,
			[MarshalAs(UnmanagedType.LPWStr)] string lpCurrentDirectory,
			[MarshalAs(UnmanagedType.LPStruct)] STARTUPINFO lpStartupInfo,
			PROCESS_INFORMATION lpProcessInformation,
			int debuggingFlags,
			out ICorDebugProcess ppProcess);

		// HRESULT DebugActiveProcess(
		//     [in]  DWORD               id,
		//     [in]  BOOL                win32Attach,
		//     [out] ICorDebugProcess    **ppProcess
		// );
		void DebugActiveProcess(
			uint id,
			[MarshalAs(UnmanagedType.Bool)] bool win32Attach,
			out ICorDebugProcess ppProcess);

		// HRESULT EnumerateProcesses(
		//     [out] ICorDebugProcessEnum      **ppProcess
		// );
		void EnumerateProcess_();

		// HRESULT GetProcess(
		//     [in] DWORD               dwProcessId,
		//     [out] ICorDebugProcess   **ppProcess
		// );
		void GetProcess_();

		// HRESULT CanLaunchOrAttach(
		//     [in] DWORD      dwProcessId,
		//     [in] BOOL       win32DebuggingEnabled
		// );
		void CanLaunchOrAttach_();
	}
}
