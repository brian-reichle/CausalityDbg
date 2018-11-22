// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;

namespace CausalityDbg.Core.CorDebugApi
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("CC7BCAEF-8A68-11D2-983C-0000F808342D")]
	interface ICorDebugFrame
	{
		// HRESULT GetChain(
		//     [out] ICorDebugChain     **ppChain
		// );
		ICorDebugChain GetChain();

		// HRESULT GetCode(
		//     [out] ICorDebugCode      **ppCode
		// );
		void GetCode_();

		// HRESULT GetFunction(
		//     [out] ICorDebugFunction  **ppFunction
		// );
		ICorDebugFunction GetFunction();

		// HRESULT GetFunctionToken(
		//     [out] mdMethodDef        *pToken
		// );
		void GetFunctionToken_();

		// HRESULT GetStackRange(
		//     [out] CORDB_ADDRESS      *pStart,
		//     [out] CORDB_ADDRESS      *pEnd
		// );
		void GetStackRange(
			out CORDB_ADDRESS pStart,
			out CORDB_ADDRESS pEnd);

		// HRESULT GetCaller(
		//     [out] ICorDebugFrame     **ppFrame
		// );
		ICorDebugFrame GetCaller();

		// HRESULT GetCallee(
		//     [out] ICorDebugFrame     **ppFrame
		// );
		ICorDebugFrame GetCallee();

		// HRESULT CreateStepper(
		//     [out] ICorDebugStepper   **ppStepper
		// );
		ICorDebugStepper CreateStepper();
	}
}
