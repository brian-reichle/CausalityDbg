// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;

namespace CausalityDbg.Core.CorDebugApi
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("CC7BCAE8-8A68-11D2-983C-0000F808342D")]
	interface ICorDebugBreakpoint
	{
		// HRESULT Activate(
		//     [in] BOOL bActive
		// );
		void Activate(
			[MarshalAs(UnmanagedType.Bool)] bool bActive);

		// HRESULT IsActive(
		//     [out] BOOL *pbActive
		// );
		[return: MarshalAs(UnmanagedType.Bool)]
		bool IsActive();
	}
}
