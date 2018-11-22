// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;

namespace CausalityDbg.Core.CLRHostApi
{
	[ComImport]
	[InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("BD39D1D2-BA2F-486A-89B0-B4B0CB466891")]
	interface ICLRRuntimeInfo
	{
		// HRESULT GetVersionString(
		//     [out, size_is(*pcchBuffer)] LPWSTR pwzBuffer,
		//     [in, out]  DWORD *pcchBuffer
		// );
		[PreserveSig]
		int GetVersionString(
			[MarshalAs(UnmanagedType.LPArray)] char[] pwzBuffer,
			ref int pcchBuffer);

		// HRESULT GetRuntimeDirectory(
		//     [out, size_is(*pcchBuffer)] LPWSTR pwzBuffer,
		//     [in, out]  DWORD *pcchBuffer
		// );
		void GetRuntimeDirectory(
			[MarshalAs(UnmanagedType.LPArray)] char[] pwzBuffer,
			ref int pcchBuffer);

		// HRESULT IsLoaded(
		//     [in]  HANDLE hndProcess,
		//     [out, retval] BOOL *pbLoaded
		// );
		void IsLoaded(
			ProcessHandle hndProcess,
			[MarshalAs(UnmanagedType.Bool)] out bool pbLoaded);

		// HRESULT LoadErrorString(
		//     [in] UINT iResourceID,
		//     [out, size_is(*pcchBuffer)] LPWSTR pwzBuffer,
		//     [in, out]  DWORD *pcchBuffer,
		//     [in, lcid] LONG iLocaleID
		// );
		void LoadErrorString(
			int iResourceID,
			[MarshalAs(UnmanagedType.LPWStr)] char[] pwzBuffer,
			ref int pcchBuffer,
			int iLocaleID);

		// HRESULT LoadLibrary(
		//     [in]  LPCWSTR pwzDllName,
		//     [out, retval] HMODULE *phndModule
		// );
		void LoadLibrary(
			[MarshalAs(UnmanagedType.LPWStr)] string pwzDllName,
			out IntPtr phndModule);

		// HRESULT GetProcAddress(
		//     [in]  LPCSTR pszProcName,
		//     [out, retval] LPVOID *ppProc
		// );
		void GetProcAddress(
			[MarshalAs(UnmanagedType.LPStr)] string pszProcName,
			IntPtr ppProc);

		// HRESULT GetInterface(
		//     [in]  REFCLSID rclsid,
		//     [in]  REFIID   riid,
		//     [out, iid_is(riid), retval] LPVOID *ppUnk
		// );
		[return: MarshalAs(UnmanagedType.IUnknown)]
		object GetInterface(
			[MarshalAs(UnmanagedType.LPStruct)] Guid rclsid,
			[MarshalAs(UnmanagedType.LPStruct)] Guid riid);

		// HRESULT IsLoadable(
		//     [out, retval] BOOL *pbLoadable
		// );
		[return: MarshalAs(UnmanagedType.Bool)]
		bool IsLoadable();

		// HRESULT SetDefaultStartupFlags(
		//     [in]  DWORD dwStartupFlags,
		//     [in]  LPCWSTR pwzHostConfigFile
		// );
		void SetDefaultStartupFlags(
			int dwStartupFlags,
			[MarshalAs(UnmanagedType.LPWStr)] string pwzHostConfigFile);

		// HRESULT GetDefaultStartupFlags(
		//     [out]  DWORD *pdwStartupFlags,
		//     [out, size_is(*pcchHostConfigFile)] LPWSTR pwzHostConfigFile,
		//     [in, out]  DWORD *pcchHostConfigFile
		// );
		void GetDefaultStartupFlags(
			out int pdwStartupFlags,
			[MarshalAs(UnmanagedType.LPArray)] char[] pwzHostConfigFile,
			ref int pcchHostConfigFile);

		// HRESULT BindAsLegacyV2Runtime();
		void BindAsLegacyV2Runtime();

		// HRESULT IsStarted(
		//     [out] BOOL     *pbStarted,
		//     [out] DWORD    *pdwStartupFlags
		// );
		void IsStarted(
			[MarshalAs(UnmanagedType.Bool)] out bool pbStarted,
			out int pdwStartupFlags);
	}
}
