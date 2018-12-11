// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;

namespace CausalityDbg.Core.CorDebugApi
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("250E5EEA-DB5C-4C76-B6F3-8C46F12E3203")]
	interface ICorDebugManagedCallback2
	{
		// HRESULT FunctionRemapOpportunity(
		//     [in] ICorDebugAppDomain   *pAppDomain,
		//     [in] ICorDebugThread      *pThread,
		//     [in] ICorDebugFunction    *pOldFunction,
		//     [in] ICorDebugFunction    *pNewFunction,
		//     [in] ULONG32              oldILOffset
		// );
		void FunctionRemapOpportunity(
			ICorDebugAppDomain pAppDomain,
			ICorDebugThread pThread,
			ICorDebugFunction pOldFunction,
			ICorDebugFunction pNewFunction,
			uint oldILOffset);

		// HRESULT CreateConnection(
		//     [in] ICorDebugProcess     *pProcess,
		//     [in] CONNID               dwConnectionId,
		//     [in] WCHAR                *pConnName
		// );
		void CreateConnection(
			ICorDebugProcess pProcess,
			int dwConnectionId,
			[MarshalAs(UnmanagedType.LPWStr)] string pConnName);

		// HRESULT ChangeConnection(
		//     [in] ICorDebugProcess     *pProcess,
		//     [in] CONNID               dwConnectionId
		// );
		void ChangeConnection(
			ICorDebugProcess pProcess,
			int dwConnectionId);

		// HRESULT DestroyConnection(
		//     [in] ICorDebugProcess     *pProcess,
		//     [in] CONNID               dwConnectionId
		// );
		void DestroyConnection(
			ICorDebugProcess pProcess,
			int dwConnectionId);

		// HRESULT Exception(
		//     [in] ICorDebugAppDomain   *pAppDomain,
		//     [in] ICorDebugThread      *pThread,
		//     [in] ICorDebugFrame       *pFrame,
		//     [in] ULONG32              nOffset,
		//     [in] CorDebugExceptionCallbackType dwEventType,
		//     [in] DWORD                dwFlags
		// );
		void Exception(
			ICorDebugAppDomain pAppDomain,
			ICorDebugThread pThread,
			ICorDebugFrame pFrame,
			uint nOffset,
			CorDebugExceptionCallbackType dwEventType,
			CorDebugExceptionFlags dwFlags);

		// HRESULT ExceptionUnwind(
		//     [in] ICorDebugAppDomain                  *pAppDomain,
		//     [in] ICorDebugThread                     *pThread,
		//     [in] CorDebugExceptionUnwindCallbackType  dwEventType,
		//     [in] DWORD                                dwFlags
		// );
		void ExceptionUnwind(
			ICorDebugAppDomain pAppDomain,
			ICorDebugThread pThread,
			CorDebugExceptionCallbackType dwEventType,
			CorDebugExceptionFlags dwFlags);

		// HRESULT FunctionRemapComplete(
		//     [in] ICorDebugAppDomain   *pAppDomain,
		//     [in] ICorDebugThread      *pThread,
		//     [in] ICorDebugFunction    *pFunction
		// );
		void FunctionRemapComplete(
			ICorDebugAppDomain pAppDomain,
			ICorDebugThread pThread,
			ICorDebugFunction pFunction);

		// HRESULT MDANotification(
		//     [in] ICorDebugController  *pController,
		//     [in] ICorDebugThread      *pThread,
		//     [in] ICorDebugMDA         *pMDA
		// );
		void MDANotification(
			ICorDebugController pController,
			ICorDebugThread pThread,
			IntPtr pMDA);
	}
}
