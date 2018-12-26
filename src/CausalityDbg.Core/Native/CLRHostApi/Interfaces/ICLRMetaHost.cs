// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;

namespace CausalityDbg.Core.CLRHostApi
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("D332DB9E-B9B3-4125-8207-A14884F53216")]
	interface ICLRMetaHost
	{
		// HRESULT GetRuntime(
		//     [in] LPCWSTR pwzVersion,
		//     [in, REFIID riid,
		//     [out,iid_is(riid), retval] LPVOID *ppRuntime
		// );
		[PreserveSig]
		int GetRuntime(
			[MarshalAs(UnmanagedType.LPWStr)] string pwzVersion,
			[MarshalAs(UnmanagedType.LPStruct)] Guid riid,
			[MarshalAs(UnmanagedType.Interface)] out object ppRuntime);

		// HRESULT GetVersionFromFile(
		//     [in] LPCWSTR pwzFilePath,
		//     [out, size_is(*pcchBuffer)] LPWSTR pwzBuffer,
		//     [in, out] DWORD *pcchBuffer
		// );
		[PreserveSig]
		int GetVersionFromFile(
			[MarshalAs(UnmanagedType.LPWStr)] string filePath,
			[MarshalAs(UnmanagedType.LPArray)] char[] buffer,
			ref int bufferLength);

		// HRESULT EnumerateInstalledRuntimes(
		//     [out, retval] IEnumUnknown **ppEnumerator
		// );
		void EnumerateInstalledRuntimes_();

		// HRESULT EnumerateLoadedRuntimes(
		//     [in] HANDLE hndProcess,
		//     [out, retval] IEnumUnknown **ppEnumerator
		// );
		IEnumUnknown EnumerateLoadedRuntimes(
			ProcessHandle hndProcess);

		// HRESULT RequestRuntimeLoadedNotification(
		//     [in] RuntimeLoadedCallbackFnPtr pCallbackFunction
		// );
		void RequestRuntimeLoadedNotification_();

		// HRESULT QueryLegacyV2RuntimeBinding(
		//     [in] REFIID riid,
		//     [out, iid_is(riid), retval] LPVOID *ppUnk
		// );
		void QueryLegacyV2RuntimeBinding_();

		// HRESULT ExitProcess(
		//     [in] INT32 iExitCode
		// );
		void ExitProcess(
			int iExitCode);
	}
}
