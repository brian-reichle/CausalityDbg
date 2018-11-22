// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using CausalityDbg.IL;

namespace CausalityDbg.Core.CorDebugApi
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("D613F0BB-ACE1-4C19-BD72-E4C08D5DA7F5")]
	interface ICorDebugType
	{
		// HRESULT GetType(
		//     [out] CorElementType   *ty
		// );
		CorElementType GetType();

		// HRESULT GetClass(
		//     [out] ICorDebugClass   **ppClass
		// );
		ICorDebugClass GetClass();

		// HRESULT EnumerateTypeParameters(
		//     [out] ICorDebugTypeEnum   **ppTyParEnum
		// );
		ICorDebugTypeEnum EnumerateTypeParameters();

		// HRESULT GetFirstTypeParameter(
		//     [out] ICorDebugType   **value
		// );
		ICorDebugType GetFirstTypeParameter();

		// HRESULT GetBase(
		//     [out] ICorDebugType   **pBase
		// );
		void GetBase_();

		// HRESULT GetStaticFieldValue(
		//     [in]  mdFieldDef        fieldDef,
		//     [in]  ICorDebugFrame    *pFrame,
		//     [out] ICorDebugValue    **ppValue
		// );
		void GetStaticFieldValue_();

		// HRESULT GetRank(
		//     [out] ULONG32   *pnRank
		// );
		int GetRank();
	}
}
