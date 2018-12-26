// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;

namespace CausalityDbg.Core.CorDebugApi
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("CC7BCAFD-8A68-11D2-983C-0000F808342D")]
	interface ICorDebugStringValue : ICorDebugHeapValue
	{
		// HRESULT GetType(
		//     [out] CorElementType   *pType
		// );
		new void GetType_();

		// HRESULT GetSize(
		//     [out] ULONG32   *pSize
		// );
		new void GetSize_();

		// HRESULT GetAddress(
		//     [out] CORDB_ADDRESS   *pAddress
		// );
		new void GetAddress_();

		// HRESULT CreateBreakpoint(
		//     [out] ICorDebugValueBreakpoint **ppBreakpoint
		// );
		new void CreateBreakpoint_();

		// HRESULT IsValid(
		//     [out] BOOL    *pbValid
		// );
		new void IsValid_();

		// HRESULT CreateRelocBreakpoint(
		//     [out] ICorDebugValueBreakpoint **ppBreakpoint
		// );
		new void CreateRelocBreakpoint_();

		// HRESULT GetLength(
		//     [out] ULONG32   *pcchString
		// );
		int GetLength();

		// HRESULT GetString(
		//     [in] ULONG32    cchString,
		//     [out] ULONG32   *pcchString,
		//     [out, size_is(cchString), length_is(*pcchString)] WCHAR szString[]
		// );
		void GetString(
			int cchString,
			out int pcchString,
			[MarshalAs(UnmanagedType.LPArray)] char[] szString);
	}
}
