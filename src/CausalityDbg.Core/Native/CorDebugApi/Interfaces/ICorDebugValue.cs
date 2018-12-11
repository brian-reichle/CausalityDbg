// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;

namespace CausalityDbg.Core.CorDebugApi
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("CC7BCAF7-8A68-11D2-983C-0000F808342D")]
	interface ICorDebugValue
	{
		// HRESULT GetType(
		//     [out] CorElementType   *pType
		// );
		void GetType_();

		// HRESULT GetSize(
		//     [out] ULONG32   *pSize
		// );
		void GetSize_();

		// HRESULT GetAddress(
		//     [out] CORDB_ADDRESS   *pAddress
		// );
		void GetAddress_();

		// HRESULT CreateBreakpoint(
		//     [out] ICorDebugValueBreakpoint **ppBreakpoint
		// );
		void CreateBreakpoint_();
	}
}
