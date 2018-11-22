// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;

namespace CausalityDbg.Core.CorDebugApi
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("5D88A994-6C30-479B-890F-BCEF88B129A5")]
	interface ICorDebugILFrame2
	{
		// HRESULT RemapFunction(
		//     [in] ULONG32      newILOffset
		// );
		void RemapFunction_();

		// HRESULT EnumerateTypeParameters(
		//     [out] ICorDebugTypeEnum    **ppTyParEnum
		// );
		ICorDebugTypeEnum EnumerateTypeParameters();
	}
}
