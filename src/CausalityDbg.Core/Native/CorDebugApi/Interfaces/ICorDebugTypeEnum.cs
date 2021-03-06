// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;

namespace CausalityDbg.Core.CorDebugApi
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("10F27499-9DF2-43CE-8333-A321D7C99CB4")]
	interface ICorDebugTypeEnum : ICorDebugEnum
	{
		// HRESULT Skip(
		//     [in] ULONG celt
		// );
		new void Skip_();

		// HRESULT Reset();
		new void Reset_();

		// HRESULT Clone(
		//     [out] ICorDebugEnum **ppEnum
		// );
		new void Clone_();

		// HRESULT GetCount(
		//     [out] ULONG *pcelt
		// );
		new int GetCount();

		// HRESULT Next(
		//     [in]  ULONG  celt,
		//     [out, size_is(celt), length_is(*pceltFetched)] ICorDebugType *values[],
		//     [out] ULONG *pceltFetched
		// );
		[return: MarshalAs(UnmanagedType.Bool)]
		bool Next(
			uint celt,
			out ICorDebugType values);
	}
}
