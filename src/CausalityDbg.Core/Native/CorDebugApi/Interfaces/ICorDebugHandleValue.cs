// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;

namespace CausalityDbg.Core.CorDebugApi
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("029596E8-276B-46A1-9821-732E96BBB00B")]
	interface ICorDebugHandleValue : ICorDebugReferenceValue
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

		// HRESULT IsNull(
		//     [out] BOOL   *pbNull
		// );
		[PreserveSig]
		new int IsNull(
			[MarshalAs(UnmanagedType.Bool)] out bool pbNull);

		// HRESULT GetValue (
		//     [out] CORDB_ADDRESS   *pValue
		// );
		new CORDB_ADDRESS GetValue();

		// HRESULT SetValue(
		//     [in] CORDB_ADDRESS    value
		// );
		new void SetValue_();

		// HRESULT Dereference(
		//     [out] ICorDebugValue  **ppValue
		// );
		new ICorDebugValue Dereference();

		// HRESULT DereferenceStrong(
		//     [out] ICorDebugValue  **ppValue
		// );
		new void DereferenceStrong_();

		// HRESULT GetHandleType(
		//     [out] CorDebugHandleType  *pType
		// );
		void GetHandleType_();

		// HRESULT Dispose();
		[PreserveSig]
		int Dispose();
	}
}
