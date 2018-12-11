// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;

namespace CausalityDbg.Core.CorDebugApi
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("3D6F5F64-7538-11D3-8D5B-00104B35E7EF")]
	interface ICorDebugProcess : ICorDebugController
	{
		// HRESULT Stop(
		//     [in] DWORD dwTimeoutIgnored
		// );
		new void Stop(
			int dwTimeoutIgnored = 0);

		// HRESULT Continue(
		//     [in] BOOL fIsOutOfBand
		// );
		new void Continue(
			[MarshalAs(UnmanagedType.Bool)] bool fIsOutOfBand);

		// HRESULT IsRunning(
		//     [out] BOOL *pbRunning
		// );
		new void IsRunning_();

		// HRESULT HasQueuedCallbacks(
		//     [in] ICorDebugThread *pThread,
		//     [out] BOOL *pbQueued
		// );
		[return: MarshalAs(UnmanagedType.Bool)]
		new bool HasQueuedCallbacks(
			ICorDebugThread pThread);

		// HRESULT EnumerateThreads(
		//     [out] ICorDebugThreadEnum **ppThreads
		// );
		new void EnumerateThreads_();

		// HRESULT SetAllThreadsDebugState(
		//     [in] CorDebugThreadState state,
		//     [in] ICorDebugThread *pExceptThisThread
		// );
		new void SetAllThreadsDebugState_();

		// HRESULT Detach();
		new void Detach();

		// HRESULT Terminate(
		//     [in] UINT exitCode
		// );
		new void Terminate_();

		// /*  OBSOLETE  */
		// HRESULT CanCommitChanges(
		//     [in] ULONG cSnapshots,
		//     [in, size_is(cSnapshots)] ICorDebugEditAndContinueSnapshot *pSnapshots[],
		//     [out] ICorDebugErrorInfoEnum **pError
		// );
		new void CanCommitChanges_();

		// /*  OBSOLETE  */
		// HRESULT CommitChanges(
		//     [in] ULONG cSnapshots,
		//     [in, size_is(cSnapshots)] ICorDebugEditAndContinueSnapshot *pSnapshots[],
		//     [out] ICorDebugErrorInfoEnum **pError
		// );
		new void CommitChanges_();

		// HRESULT GetID(
		//     [out] DWORD *pdwProcessId
		// );
		int GetID();

		// HRESULT GetHandle(
		//     [out] HPROCESS *phProcessHandle
		// );
		void GetHandle_();

		// HRESULT GetThread(
		//     [in] DWORD dwThreadId,
		//     [out] ICorDebugThread **ppThread
		// );
		void GetThread_();

		// HRESULT EnumerateObjects(
		//     [out] ICorDebugObjectEnum **ppObjects
		// );
		void EnumerateObjects_();

		// HRESULT IsTransitionStub(
		//     [in]  CORDB_ADDRESS address,
		//     [out] BOOL *pbTransitionStub
		// );
		void IsTransitionStub_();

		// HRESULT IsOSSuspended(
		//     [in]  DWORD threadID,
		//     [out] BOOL  *pbSuspended
		// );
		void IsOSSuspended_();

		// HRESULT GetThreadContext(
		//     [in] DWORD threadID,
		//     [in] ULONG32 contextSize,
		//     [in, out, length_is(contextSize), size_is(contextSize)] BYTE context[]
		// );
		void GetThreadContext_();

		// HRESULT SetThreadContext(
		//     [in] DWORD threadID,
		//     [in] ULONG32 contextSize,
		//     [in, length_is(contextSize), size_is(contextSize)] BYTE context[]
		// );
		void SetThreadContext_();

		// HRESULT ReadMemory(
		//     [in]  CORDB_ADDRESS address,
		//     [in]  DWORD size,
		//     [out, size_is(size), length_is(size)] BYTE buffer[],
		//     [out] SIZE_T *read
		// );
		void ReadMemory(
			CORDB_ADDRESS address,
			int size,
			IntPtr buffer,
			out IntPtr read);

		// HRESULT WriteMemory(
		//     [in]  CORDB_ADDRESS address,
		//     [in]  DWORD size,
		//     [in, size_is(size)] BYTE buffer[],
		//     [out] SIZE_T *written
		// );
		void WriteMemory_();

		// HRESULT ClearCurrentException(
		//     [in] DWORD threadID
		// );
		void ClearCurrentException_();

		// HRESULT EnableLogMessages(
		//     [in]BOOL fOnOff
		// );
		void EnableLogMessages_();

		// HRESULT ModifyLogSwitch(
		//     [in] WCHAR *pLogSwitchName,
		//     [in] LONG  lLevel
		// );
		void ModifyLogSwitch_();

		// HRESULT EnumerateAppDomains(
		//     [out] ICorDebugAppDomainEnum **ppAppDomains
		// );
		void EnumerateAppDomains_();

		// HRESULT GetObject(
		//     [out] ICorDebugValue **ppObject
		// );
		void GetObject_();

		// HRESULT ThreadForFiberCookie(
		//     [in] DWORD fiberCookie,
		//     [out] ICorDebugThread **ppThread
		// );
		void ThreadForFiberCookie_();

		// HRESULT GetHelperThreadID(
		//     [out] DWORD *pThreadID
		// );
		void GetHelperThreadID_();
	}
}
