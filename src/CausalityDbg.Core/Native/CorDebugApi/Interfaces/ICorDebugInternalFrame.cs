// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;

namespace CausalityDbg.Core.CorDebugApi
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("B92CC7F7-9D2D-45C4-BC2B-621FCC9DFBF4")]
	interface ICorDebugInternalFrame : ICorDebugFrame
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

		// HRESULT GetFrameType(
		//     [out] CorDebugInternalFrameType  *pType
		// );
		CorDebugInternalFrameType GetFrameType();
	}
}
