// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;

namespace CausalityDbg.Core.CorDebugApi
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("E3AC4D6C-9CB7-43E6-96CC-B21540E5083C")]
	interface ICorDebugHeapValue2
	{
		// HRESULT CreateHandle(
		//     [in] CorDebugHandleType      type,
		//     [out] ICorDebugHandleValue   **ppHandle
		// );
		ICorDebugHandleValue CreateHandle(
			CorDebugHandleType type);
	}
}
