// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;

namespace CausalityDbg.Core.CorDebugApi
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("FB0D9CE7-BE66-4683-9D32-A42A04E2FD91")]
	interface ICorDebugEval2
	{
		// HRESULT CallParameterizedFunction(
		//     [in] ICorDebugFunction     *pFunction,
		//     [in] ULONG32               nTypeArgs,
		//     [in, size_is(nTypeArgs)] ICorDebugType *ppTypeArgs[],
		//     [in] ULONG32               nArgs,
		//     [in, size_is(nArgs)] ICorDebugValue *ppArgs[]
		// );
		void CallParameterizedFunction_();

		// HRESULT CreateValueForType(
		//     [in] ICorDebugType         *pType,
		//     [out] ICorDebugValue       **ppValue
		// );
		void CreateValueForType_();

		// HRESULT NewParameterizedObject(
		//     [in] ICorDebugFunction     *pConstructor,
		//     [in] ULONG32               nTypeArgs,
		//     [in, size_is(nTypeArgs)] ICorDebugType *ppTypeArgs[],
		//     [in] ULONG32               nArgs,
		//     [in, size_is(nArgs)] ICorDebugValue *ppArgs[]
		// );
		void NewParameterizedObject_();

		// HRESULT NewParameterizedObjectNoConstructor(
		//     [in] ICorDebugClass        *pClass,
		//     [in] ULONG32               nTypeArgs,
		//     [in, size_is(nTypeArgs)] ICorDebugType *ppTypeArgs[]
		// );
		void NewParameterizedObjectNoConstructor_();

		// HRESULT NewParameterizedArray(
		//     [in] ICorDebugType          *pElementType,
		//     [in] ULONG32                rank,
		//     [in, size_is(rank)] ULONG32 dims[],
		//     [in, size_is(rank)] ULONG32 lowBounds[]
		// );
		void NewParameterizedArray_();

		// HRESULT NewStringWithLength(
		//     [in] LPCWSTR               string,
		//     [in] UINT                  uiLength
		// );
		void NewStringWithLength_();

		// HRESULT RudeAbort();
		void RudeAbort_();
	}
}
