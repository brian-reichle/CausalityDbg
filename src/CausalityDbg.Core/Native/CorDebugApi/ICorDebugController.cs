// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;

namespace CausalityDbg.Core.CorDebugApi
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("3D6F5F62-7538-11D3-8D5B-00104B35E7EF")]
	interface ICorDebugController
	{
		// HRESULT Stop(
		//     [in] DWORD dwTimeoutIgnored
		// );
		void Stop(
			int pwTimeoutIgnored);

		// HRESULT Continue(
		//     [in] BOOL fIsOutOfBand
		// );
		void Continue(
			[MarshalAs(UnmanagedType.Bool)] bool fIsOutOfBand);

		// HRESULT IsRunning(
		//     [out] BOOL *pbRunning
		// );
		void IsRunning_();

		// HRESULT HasQueuedCallbacks(
		//     [in] ICorDebugThread *pThread,
		//     [out] BOOL *pbQueued
		// );
		[return: MarshalAs(UnmanagedType.Bool)]
		bool HasQueuedCallbacks(
			ICorDebugThread pThread);

		// HRESULT EnumerateThreads(
		//     [out] ICorDebugThreadEnum **ppThreads
		// );
		void EnumerateThreads_();

		// HRESULT SetAllThreadsDebugState(
		//     [in] CorDebugThreadState state,
		//     [in] ICorDebugThread *pExceptThisThread
		// );
		void SetAllThreadsDebugState_();

		// HRESULT Detach();
		void Detach();

		// HRESULT Terminate(
		//     [in] UINT exitCode
		// );
		void Terminate_();

		// /*  OBSOLETE  */
		// HRESULT CanCommitChanges(
		//     [in] ULONG cSnapshots,
		//     [in, size_is(cSnapshots)] ICorDebugEditAndContinueSnapshot *pSnapshots[],
		//     [out] ICorDebugErrorInfoEnum **pError
		// );
		void CanCommitChanges_();

		// /*  OBSOLETE  */
		// HRESULT CommitChanges(
		//     [in] ULONG cSnapshots,
		//     [in, size_is(cSnapshots)] ICorDebugEditAndContinueSnapshot *pSnapshots[],
		//     [out] ICorDebugErrorInfoEnum **pError
		// );
		void CommitChanges_();
	}
}
