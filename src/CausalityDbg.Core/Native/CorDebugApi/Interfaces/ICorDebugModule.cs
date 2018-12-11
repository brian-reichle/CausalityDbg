// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using CausalityDbg.IL;

namespace CausalityDbg.Core.CorDebugApi
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("DBA2D8C1-E5C5-4069-8C13-10A7C6ABF43D")]
	interface ICorDebugModule
	{
		// HRESULT GetProcess(
		//     [out] ICorDebugProcess **ppProcess
		// );
		ICorDebugProcess GetProcess();

		// HRESULT GetBaseAddress(
		//     [out] CORDB_ADDRESS *pAddress
		// );
		CORDB_ADDRESS GetBaseAddress();

		// HRESULT GetAssembly(
		//     [out] ICorDebugAssembly **ppAssembly
		// );
		ICorDebugAssembly GetAssembly();

		// HRESULT GetName(
		//     [in] ULONG32 cchName,
		//     [out] ULONG32 *pcchName,
		//     [out, size_is(cchName), length_is(*pcchName)] WCHAR szName[]
		// );
		void GetName(
			uint cchName,
			out uint pcchName,
			[MarshalAs(UnmanagedType.LPArray)] char[] szName);

		// HRESULT EnableJITDebugging(
		//     [in] BOOL bTrackJITInfo,
		//     [in] BOOL bAllowJitOpts
		// );
		void EnableJITDebugging_();

		// HRESULT EnableClassLoadCallbacks(
		//     [in] BOOL bClassLoadCallbacks
		// );
		void EnableClassLoadCallbacks_();

		// HRESULT GetFunctionFromToken(
		//     [in] mdMethodDef methodDef,
		//     [out] ICorDebugFunction **ppFunction
		// );
		ICorDebugFunction GetFunctionFromToken(
			MetaDataToken methodDef);

		// HRESULT GetFunctionFromRVA(
		//     [in]  CORDB_ADDRESS      rva,
		//     [out] ICorDebugFunction  **ppFunction
		// );
		void GetFunctionFromRVA_();

		// HRESULT GetClassFromToken(
		//     [in]  mdTypeDef        typeDef,
		//     [out] ICorDebugClass **ppClass
		// );
		ICorDebugClass GetClassFromToken(
			MetaDataToken typeDef);

		// HRESULT CreateBreakpoint(
		//     [out] ICorDebugModuleBreakpoint **ppBreakpoint
		// );
		void CreateBreakpoint_();

		// HRESULT GetEditAndContinueSnapshot(
		//     [out] ICorDebugEditAndContinueSnapshot **ppEditAndContinueSnapshot
		// );
		void GetEditAndContinueSnapshot_();

		// HRESULT GetMetaDataInterface(
		//     [in] REFIID      riid,
		//     [out] IUnknown **ppObj
		// );
		[return: MarshalAs(UnmanagedType.Interface)]
		object GetMetaDataInterface(
			[MarshalAs(UnmanagedType.LPStruct)] Guid riid);

		// HRESULT GetToken(
		//     [out] mdModule *pToken
		// );
		void GetToken_();

		// HRESULT IsDynamic(
		//     [out] BOOL *pDynamic
		// );
		[return: MarshalAs(UnmanagedType.Bool)]
		bool IsDynamic();

		// HRESULT GetGlobalVariableValue(
		//     [in]  mdFieldDef      fieldDef,
		//     [out] ICorDebugValue  **ppValue
		// );
		void GetGlobalVariableValue_();

		// HRESULT GetSize(
		//     [out] ULONG32 *pcBytes
		// );
		void GetSize_();

		// HRESULT IsInMemory(
		//     [out] BOOL *pInMemory
		// );
		[return: MarshalAs(UnmanagedType.Bool)]
		bool IsInMemory();
	}
}
