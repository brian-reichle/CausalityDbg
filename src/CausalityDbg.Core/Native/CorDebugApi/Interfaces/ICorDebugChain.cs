// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;

namespace CausalityDbg.Core.CorDebugApi
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("CC7BCAEE-8A68-11D2-983C-0000F808342D")]
	interface ICorDebugChain
	{
		// HRESULT GetThread(
		//     [out] ICorDebugThread    **ppThread
		// );
		void GetThread_();

		// HRESULT GetStackRange(
		//     [out] CORDB_ADDRESS      *pStart,
		//     [out] CORDB_ADDRESS      *pEnd
		// );
		void GetStackRange_();

		// HRESULT GetContext(
		//     [out] ICorDebugContext   **ppContext
		// );
		void GetContext_();

		// HRESULT GetCaller(
		//     [out] ICorDebugChain      **ppChain
		// );
		ICorDebugChain GetCaller();

		// HRESULT GetCallee(
		//     [out] ICorDebugChain     **ppChain
		// );
		void GetCallee_();

		// HRESULT GetPrevious(
		//     [out] ICorDebugChain     **ppChain
		// );
		ICorDebugChain GetPrevious();

		// HRESULT GetNext(
		//     [out] ICorDebugChain     **ppChain
		// );
		void GetNext_();

		// HRESULT IsManaged(
		//     [out] BOOL               *pManaged
		// );
		void IsManaged_();

		// HRESULT EnumerateFrames(
		//     [out] ICorDebugFrameEnum **ppFrames
		// );
		void EnumerateFrames_();

		// HRESULT GetActiveFrame(
		//     [out] ICorDebugFrame   **ppFrame
		// );
		ICorDebugFrame GetActiveFrame();

		// HRESULT GetRegisterSet(
		//     [out] ICorDebugRegisterSet **ppRegisters
		// );
		void GetRegisterSet_();

		// HRESULT GetReason(
		//     [out] CorDebugChainReason *pReason
		// );
		CorDebugChainReason GetReason();
	}
}
