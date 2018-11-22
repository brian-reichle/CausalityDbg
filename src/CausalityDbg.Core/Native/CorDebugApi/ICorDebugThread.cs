// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;

namespace CausalityDbg.Core.CorDebugApi
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("938C6D66-7FB6-4F69-B389-425B8987329B")]
	interface ICorDebugThread
	{
		// HRESULT GetProcess(
		//     [out] ICorDebugProcess   **ppProcess
		// );
		ICorDebugProcess GetProcess();

		// HRESULT GetID(
		//     [out] DWORD *pdwThreadId
		// );
		uint GetID();

		// HRESULT GetHandle(
		//     [out] HTHREAD *phThreadHandle
		// );
		void GetHandle_();

		// HRESULT GetAppDomain(
		//     [out] ICorDebugAppDomain  **ppAppDomain
		// );
		void GetAppDomain_();

		// HRESULT SetDebugState(
		//     [in] CorDebugThreadState state
		// );
		void SetDebugState_();

		// HRESULT GetDebugState(
		//     [out] CorDebugThreadState   *pState
		// );
		void GetDebugState_();

		// HRESULT GetUserState(
		//     [out] CorDebugUserState   *pState
		// );
		void GetUserState_();

		// HRESULT GetCurrentException(
		//     [out] ICorDebugValue **ppExceptionObject
		// );
		ICorDebugValue GetCurrentException();

		// HRESULT ClearCurrentException();
		void ClearCurrentException_();

		// HRESULT CreateStepper(
		//     [out] ICorDebugStepper **ppStepper
		// );
		ICorDebugStepper CreateStepper();

		// HRESULT EnumerateChains(
		//     [out] ICorDebugChainEnum **ppChains
		// );
		void EnumerateChains_();

		// HRESULT GetActiveChain(
		//     [out] ICorDebugChain **ppChain
		// );
		void GetActiveChain_();

		// HRESULT GetActiveFrame(
		//     [out] ICorDebugFrame   **ppFrame
		// );
		ICorDebugFrame GetActiveFrame();

		// HRESULT GetRegisterSet(
		//     [out] ICorDebugRegisterSet **ppRegisters
		// );
		void GetRegisterSet_();

		// HRESULT CreateEval(
		//     [out] ICorDebugEval   **ppEval
		// );
		ICorDebugEval CreateEval();

		// HRESULT GetObject(
		//     [out] ICorDebugValue   **ppObject
		// );
		ICorDebugValue GetObject();
	}
}
