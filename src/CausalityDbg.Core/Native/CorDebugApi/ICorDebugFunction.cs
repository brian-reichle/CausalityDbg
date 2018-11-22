// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using CausalityDbg.IL;

namespace CausalityDbg.Core.CorDebugApi
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("CC7BCAF3-8A68-11D2-983C-0000F808342D")]
	interface ICorDebugFunction
	{
		// HRESULT GetModule(
		//     [out] ICorDebugModule **ppModule
		// );
		ICorDebugModule GetModule();

		// HRESULT GetClass(
		//     [out] ICorDebugClass **ppClass
		// );
		ICorDebugClass GetClass();

		// HRESULT GetToken(
		//     [out] mdMethodDef *pMethodDef
		// );
		MetaDataToken GetToken();

		// HRESULT GetILCode(
		//     [out] ICorDebugCode **ppCode
		// );
		void GetILCode_();

		// HRESULT GetNativeCode(
		//     [out] ICorDebugCode **ppCode
		// );
		void GetNativeCode_();

		// HRESULT CreateBreakpoint(
		//     [out] ICorDebugFunctionBreakpoint **ppBreakpoint
		// );
		ICorDebugFunctionBreakpoint CreateBreakpoint();

		// HRESULT GetLocalVarSigToken(
		//     [out] mdSignature *pmdSig
		// );
		void GetLocalVarSigToken_();

		// HRESULT GetCurrentVersionNumber(
		//     [out] ULONG32 *pnCurrentVersion
		// );
		void GetCurrentVersionNumber_();
	}
}
