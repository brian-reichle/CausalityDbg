// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;

namespace CausalityDbg.Core.CorDebugApi
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("CC7BCB01-8A68-11D2-983C-0000F808342D")]
	interface ICorDebugEnum
	{
		// HRESULT Skip(
		//     [in] ULONG celt
		// );
		void Skip_();

		// HRESULT Reset();
		void Reset_();

		// HRESULT Clone(
		//     [out] ICorDebugEnum **ppEnum
		// );
		void Clone_();

		// HRESULT GetCount(
		//     [out] ULONG *pcelt
		// );
		int GetCount();
	}
}
