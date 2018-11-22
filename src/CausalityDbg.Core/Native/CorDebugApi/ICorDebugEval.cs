// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;

namespace CausalityDbg.Core.CorDebugApi
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("CC7BCAF6-8A68-11D2-983C-0000F808342D")]
	interface ICorDebugEval
	{
		// HRESULT CallFunction(
		//     [in] ICorDebugFunction  *pFunction,
		//     [in] ULONG32            nArgs,
		//     [in, size_is(nArgs)] ICorDebugValue *ppArgs[]
		// );
		void CallFunction(
			ICorDebugFunction pFunction,
			uint nArgs,
			[MarshalAs(UnmanagedType.LPArray)] ICorDebugValue[] ppArgs);

		// HRESULT NewObject(
		//     [in] ICorDebugFunction  *pConstructor,
		//     [in] ULONG32            nArgs,
		//     [in, size_is(nArgs)] ICorDebugValue *ppArgs[]
		// );
		void NewObject_();

		// HRESULT NewObjectNoConstructor(
		//     [in] ICorDebugClass     *pClass
		// );
		void NewObjectNoConstructor_();

		// HRESULT NewString(
		//     [in] LPCWSTR   string
		// );
		void NewString_();

		// HRESULT NewArray(
		//     [in] CorElementType     elementType,
		//     [in] ICorDebugClass     *pElementClass,
		//     [in] ULONG32            rank,
		//     [in, size_is(rank)] ULONG32 dims[],
		//     [in, size_is(rank)] ULONG32 lowBounds[]
		// );
		void NewArray_();

		// HRESULT IsActive(
		//     [out] BOOL              *pbActive
		// );
		void IsActive_();

		// HRESULT Abort();
		void Abort_();

		// HRESULT GetResult(
		//     [out] ICorDebugValue    **ppResult
		// );
		ICorDebugValue GetResult();

		// HRESULT GetThread(
		//     [out] ICorDebugThread   **ppThread
		// );
		void GetThread_();

		// HRESULT CreateValue(
		//     [in] CorElementType     elementType,
		//     [in] ICorDebugClass     *pElementClass,
		//     [out] ICorDebugValue    **ppValue
		// );
		void CreateValue_();
	}
}
