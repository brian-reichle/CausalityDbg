// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;

namespace CausalityDbg.Core.CorDebugApi
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("03E26311-4F76-11D3-88C6-006097945418")]
	interface ICorDebugILFrame : ICorDebugFrame
	{
		// HRESULT GetChain(
		//     [out] ICorDebugChain     **ppChain
		// );
		new ICorDebugChain GetChain();

		// HRESULT GetCode(
		//     [out] ICorDebugCode      **ppCode
		// );
		new void GetCode_();

		// HRESULT GetFunction(
		//     [out] ICorDebugFunction  **ppFunction
		// );
		new ICorDebugFunction GetFunction();

		// HRESULT GetFunctionToken(
		//     [out] mdMethodDef        *pToken
		// );
		new void GetFunctionToken_();

		// HRESULT GetStackRange(
		//     [out] CORDB_ADDRESS      *pStart,
		//     [out] CORDB_ADDRESS      *pEnd
		// );
		new void GetStackRange(
			out CORDB_ADDRESS pStart,
			out CORDB_ADDRESS pEnd);

		// HRESULT GetCaller(
		//     [out] ICorDebugFrame     **ppFrame
		// );
		new ICorDebugFrame GetCaller();

		// HRESULT GetCallee(
		//     [out] ICorDebugFrame     **ppFrame
		// );
		new ICorDebugFrame GetCallee();

		// HRESULT CreateStepper(
		//     [out] ICorDebugStepper   **ppStepper
		// );
		new ICorDebugStepper CreateStepper();

		// HRESULT GetIP(
		//     [out] ULONG32               *pnOffset,
		//     [out] CorDebugMappingResult *pMappingResult
		// );
		CorDebugMappingResult GetIP(
			out uint pnOffset);

		// HRESULT SetIP(
		//     [in] ULONG32 nOffset
		// );
		void SetIP_();

		// HRESULT EnumerateLocalVariables(
		//     [out] ICorDebugValueEnum    **ppValueEnum
		// );
		void EnumerateLocalVariables_();

		// HRESULT GetLocalVariable(
		//     [in] DWORD                  dwIndex,
		//     [out] ICorDebugValue        **ppValue
		// );
		void GetLocalVariable_();

		// HRESULT EnumerateArguments(
		//     [out] ICorDebugValueEnum    **ppValueEnum
		// );
		void EnumerateArguments_();

		// HRESULT GetArgument(
		//     [in] DWORD                  dwIndex,
		//     [out] ICorDebugValue        **ppValue
		// );
		[PreserveSig]
		int GetArgument(
			int dwIndex,
			out ICorDebugValue ppValue);

		// HRESULT GetStackDepth(
		//     [out] ULONG32               *pDepth
		// );
		void GetStackDepth_();

		// HRESULT GetStackValue(
		//     [in] DWORD                  dwIndex,
		//     [out] ICorDebugValue        **ppValue
		// );
		void GetStackValue_();

		// HRESULT CanSetIP(
		//     [in] ULONG32   nOffset
		// );
		void CanSetIP_();
	}
}
