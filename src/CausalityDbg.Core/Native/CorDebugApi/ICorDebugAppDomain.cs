// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;

namespace CausalityDbg.Core.CorDebugApi
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("3D6F5F63-7538-11D3-8D5B-00104B35E7EF")]
	interface ICorDebugAppDomain : ICorDebugController
	{
		// HRESULT Stop(
		//     [in] DWORD dwTimeoutIgnored
		// );
		new void Stop(
			int dwTimeoutIgnored);

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
		new void HasQueuedCallbacks(
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

		// HRESULT GetProcess(
		//     [out] ICorDebugProcess   **ppProcess
		// );
		ICorDebugProcess GetProcess();

		// HRESULT EnumerateAssemblies(
		//     [out] ICorDebugAssemblyEnum  **ppAssemblies
		// );
		ICorDebugAssemblyEnum EnumerateAssemblies();

		// HRESULT GetModuleFromMetaDataInterface(
		//     [in] IUnknown           *pIMetaData,
		//     [out] ICorDebugModule  **ppModule
		// );
		void GetModuleFromMetaDataInterface_();

		// HRESULT EnumerateBreakpoints(
		//     [out] ICorDebugBreakpointEnum   **ppBreakpoints
		// );
		void EnumerateBreakpoints_();

		// HRESULT EnumerateSteppers(
		//     [out] ICorDebugStepperEnum   **ppSteppers
		// );
		void EnumerateSteppers_();

		// HRESULT IsAttached(
		//     [out] BOOL  *pbAttached
		// );
		void IsAttached_();

		// HRESULT GetName(
		//     [in]  ULONG32           cchName,
		//     [out] ULONG32           *pcchName,
		//     [out, size_is(cchName), length_is(*pcchName)] WCHAR              szName[]
		// );
		void GetName_();

		// HRESULT GetObject(
		//     [out] ICorDebugValue   **ppObject
		// );
		void GetObject_();

		// HRESULT Attach();
		void Attach();

		// HRESULT GetID(
		//     [out] ULONG32   *pId
		// );
		void GetID_();
	}
}
