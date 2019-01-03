// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;

namespace CausalityDbg.Source.MetaDataApi
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("809c652e-7396-11d2-9771-00a0c9b4d50c")]
	interface IMetaDataDispenser
	{
		// HRESULT DefineScope(
		//     [in]  REFCLSID    rclsid,
		//     [in]  DWORD       dwCreateFlags,
		//     [in]  REFIID      riid,
		//     [out] IUnknown    **ppIUnk
		// );
		void DefineScope_();

		// HRESULT OpenScope(
		//     [in]  LPCWSTR     szScope,
		//     [in]  DWORD       dwOpenFlags,
		//     [in]  REFIID      riid,
		//     [out] IUnknown    **ppIUnk
		// );
		[return: MarshalAs(UnmanagedType.IUnknown)]
		object OpenScope(
			[MarshalAs(UnmanagedType.LPWStr)] string szScope,
			CorOpenFlags dwOpenFlags,
			[MarshalAs(UnmanagedType.LPStruct)] Guid riid);

		// HRESULT OpenScopeOnMemory(
		//     [in]  LPCVOID     pData,
		//     [in]  ULONG       cbData,
		//     [in]  DWORD       dwOpenFlags,
		//     [in]  REFIID      riid,
		//     [out] IUnknown    **ppIUnk
		// );
		void OpenScopeOnMemory_();
	}
}
