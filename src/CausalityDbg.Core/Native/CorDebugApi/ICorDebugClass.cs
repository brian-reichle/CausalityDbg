// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using CausalityDbg.IL;

namespace CausalityDbg.Core.CorDebugApi
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("CC7BCAF5-8A68-11D2-983C-0000F808342D")]
	interface ICorDebugClass
	{
		// HRESULT GetModule(
		//     [out] ICorDebugModule    **pModule
		// );
		ICorDebugModule GetModule();

		// HRESULT GetToken(
		//     [out] mdTypeDef          *pTypeDef
		// );
		MetaDataToken GetToken();

		// HRESULT GetStaticFieldValue(
		//     [in]  mdFieldDef         fieldDef,
		//     [in]  ICorDebugFrame     *pFrame,
		//     [out] ICorDebugValue     **ppValue
		// );
		void GetStaticFieldValue_();
	}
}
