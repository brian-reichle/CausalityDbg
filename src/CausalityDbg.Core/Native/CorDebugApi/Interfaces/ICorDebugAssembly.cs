// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;

namespace CausalityDbg.Core.CorDebugApi
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("DF59507C-D47A-459E-BCE2-6427EAC8FD06")]
	interface ICorDebugAssembly
	{
		// HRESULT GetProcess(
		//     [out] ICorDebugProcess **ppProcess
		// );
		void GetProcess_();

		// HRESULT GetAppDomain(
		//     [out] ICorDebugAppDomain  **ppAppDomain
		// );
		ICorDebugAppDomain GetAppDomain();

		// HRESULT EnumerateModules(
		//     [out] ICorDebugModuleEnum **ppModules
		// );
		ICorDebugModuleEnum EnumerateModules();

		// HRESULT GetCodeBase(
		//     [in] ULONG32  cchName,
		//     [out] ULONG32 *pcchName,
		//     [out, size_is(cchName), length_is(*pcchName)] WCHAR szName[]
		// );
		void GetCodeBase_();

		// HRESULT GetName(
		//     [in] ULONG32  cchName,
		//     [out] ULONG32 *pcchName,
		//     [out, size_is(cchName), length_is(*pcchName)] WCHAR szName[]
		// );
		void GetName(
			int cchName,
			out int pcchName,
			[MarshalAs(UnmanagedType.LPArray)] char[] szName);
	}
}
