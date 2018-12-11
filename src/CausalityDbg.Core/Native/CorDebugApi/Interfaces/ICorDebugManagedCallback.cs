// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace CausalityDbg.Core.CorDebugApi
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("3D6F5F60-7538-11D3-8D5B-00104B35E7EF")]
	interface ICorDebugManagedCallback
	{
		// HRESULT Breakpoint(
		//     [in] ICorDebugAppDomain  *pAppDomain,
		//     [in] ICorDebugThread     *pThread,
		//     [in] ICorDebugBreakpoint *pBreakpoint
		// );
		void Breakpoint(
			ICorDebugAppDomain pAppDomain,
			ICorDebugThread pThread,
			ICorDebugBreakpoint pBreakpoint);

		// HRESULT StepComplete(
		//     [in] ICorDebugAppDomain  *pAppDomain,
		//     [in] ICorDebugThread     *pThread,
		//     [in] ICorDebugStepper    *pStepper,
		//     [in] CorDebugStepReason   reason
		// );
		void StepComplete(
			ICorDebugAppDomain pAppDomain,
			ICorDebugThread pThread,
			ICorDebugStepper pStepper,
			CorDebugStepReason reason);

		// HRESULT Break(
		//     [in] ICorDebugAppDomain *pAppDomain,
		//     [in] ICorDebugThread    *thread
		// );
		void Break(
			ICorDebugAppDomain pAppDomain,
			ICorDebugThread thread);

		// HRESULT Exception(
		//     [in] ICorDebugAppDomain *pAppDomain,
		//     [in] ICorDebugThread    *pThread,
		//     [in] BOOL                unhandled
		// );
		void Exception(
			ICorDebugAppDomain pAppDomain,
			ICorDebugThread pThread,
			[MarshalAs(UnmanagedType.Bool)] bool unhandled);

		// HRESULT EvalComplete(
		//     [in] ICorDebugAppDomain *pAppDomain,
		//     [in] ICorDebugThread    *pThread,
		//     [in] ICorDebugEval      *pEval
		// );
		void EvalComplete(
			ICorDebugAppDomain pAppDomain,
			ICorDebugThread pThread,
			ICorDebugEval pEval);

		// HRESULT EvalException(
		//     [in] ICorDebugAppDomain *pAppDomain,
		//     [in] ICorDebugThread    *pThread,
		//     [in] ICorDebugEval      *pEval
		// );
		void EvalException(
			ICorDebugAppDomain pAppDomain,
			ICorDebugThread pThread,
			ICorDebugEval pEval);

		// HRESULT CreateProcess(
		//     [in] ICorDebugProcess *pProcess
		// );
		void CreateProcess(
			ICorDebugProcess pProcess);

		// HRESULT ExitProcess(
		//     [in] ICorDebugProcess *pProcess
		// );
		void ExitProcess(
			ICorDebugProcess pProcess);

		// HRESULT CreateThread(
		//     [in] ICorDebugAppDomain *pAppDomain,
		//     [in] ICorDebugThread    *thread
		// );
		void CreateThread(
			ICorDebugAppDomain pAppDomain,
			ICorDebugThread thread);

		// HRESULT ExitThread(
		//     [in] ICorDebugAppDomain *pAppDomain,
		//     [in] ICorDebugThread    *thread
		// );
		void ExitThread(
			ICorDebugAppDomain pAppDomain,
			ICorDebugThread thread);

		// HRESULT LoadModule(
		//     [in] ICorDebugAppDomain *pAppDomain,
		//     [in] ICorDebugModule    *pModule
		// );
		void LoadModule(
			ICorDebugAppDomain pAppDomain,
			ICorDebugModule pModule);

		// HRESULT UnloadModule(
		//     [in] ICorDebugAppDomain  *pAppDomain,
		//     [in] ICorDebugModule     *pModule
		// );
		void UnloadModule(
			ICorDebugAppDomain pAppDomain,
			ICorDebugModule pModule);

		// HRESULT LoadClass(
		//     [in] ICorDebugAppDomain *pAppDomain,
		//     [in] ICorDebugClass     *c
		// );
		void LoadClass(
			ICorDebugAppDomain pAppDomain,
			ICorDebugClass c);

		// HRESULT UnloadClass(
		//     [in] ICorDebugAppDomain  *pAppDomain,
		//     [in] ICorDebugClass      *c
		// );
		void UnloadClass(
			ICorDebugAppDomain pAppDomain,
			ICorDebugClass c);

		// HRESULT DebuggerError(
		//     [in] ICorDebugProcess *pProcess,
		//     [in] HRESULT           errorHR,
		//     [in] DWORD             errorCode
		// );
		void DebuggerError(
			ICorDebugProcess pProcess,
			int errorHR,
			int errorCode);

		// HRESULT LogMessage(
		//     [in] ICorDebugAppDomain  *pAppDomain,
		//     [in] ICorDebugThread     *pThread,
		//     [in] LONG                 lLevel,
		//     [in] WCHAR               *pLogSwitchName,
		//     [in] WCHAR               *pMessage
		// );
		void LogMessage(
			ICorDebugAppDomain pAppDomain,
			ICorDebugThread pThread,
			int lLevel,
			[MarshalAs(UnmanagedType.LPWStr)] string pLogSwitchName,
			[MarshalAs(UnmanagedType.LPWStr)] string pMessage);

		// HRESULT LogSwitch(
		//     [in] ICorDebugAppDomain  *pAppDomain,
		//     [in] ICorDebugThread     *pThread,
		//     [in] LONG                 lLevel,
		//     [in] ULONG                ulReason,
		//     [in] WCHAR               *pLogSwitchName,
		//     [in] WCHAR               *pParentName);
		void LogSwitch(
			ICorDebugAppDomain pAppDomain,
			ICorDebugThread pThread,
			int lLevel,
			uint ulReason,
			[MarshalAs(UnmanagedType.LPWStr)] string pLogSwitchName,
			[MarshalAs(UnmanagedType.LPWStr)] string pParentName);

		// HRESULT CreateAppDomain(
		//     [in] ICorDebugProcess   *pProcess,
		//     [in] ICorDebugAppDomain *pAppDomain
		// );
		void CreateAppDomain(
			ICorDebugProcess pProcess,
			ICorDebugAppDomain pAppDomain);

		// HRESULT ExitAppDomain(
		//     [in] ICorDebugProcess   *pProcess,
		//     [in] ICorDebugAppDomain *pAppDomain
		// );
		void ExitAppDomain(
			ICorDebugProcess pProcess,
			ICorDebugAppDomain pAppDomain);

		// HRESULT LoadAssembly(
		//     [in] ICorDebugAppDomain *pAppDomain,
		//     [in] ICorDebugAssembly  *pAssembly
		// );
		void LoadAssembly(
			ICorDebugAppDomain pAppDomain,
			ICorDebugAssembly pAssembly);

		// HRESULT UnloadAssembly(
		//     [in] IcorDebugAppDomain  *pAppDomain,
		//     [in] ICorDebugAssembly   *pAssembly
		// );
		void UnloadAssembly(
			ICorDebugAppDomain pAppDomain,
			ICorDebugAssembly pAssembly);

		// HRESULT ControlCTrap(
		//     [in] ICorDebugProcess *pProcess
		// );
		void ControlCTrap(
			ICorDebugProcess pProcess);

		// HRESULT NameChange(
		//     [in] ICorDebugAppDomain *pAppDomain,
		//     [in] ICorDebugThread    *pThread
		// );
		void NameChange(
			ICorDebugAppDomain pAppDomain,
			ICorDebugThread pThread);

		// HRESULT UpdateModuleSymbols(
		//     [in] ICorDebugAppDomain *pAppDomain,
		//     [in] ICorDebugModule    *pModule,
		//     [in] IStream            *pSymbolStream
		// );
		void UpdateModuleSymbols(
			ICorDebugAppDomain pAppDomain,
			ICorDebugModule pMmodule,
			IStream pSymbolStream);

		// HRESULT EditAndContinueRemap(
		//     [in] ICorDebugAppDomain *pAppDomain,
		//     [in] ICorDebugThread *pThread,
		//     [in] ICorDebugFunction *pFunction,
		//     [in] BOOL fAccurate
		// );
		void EditAndContinueRemap(
			ICorDebugAppDomain pAppDomain,
			ICorDebugThread pThread,
			ICorDebugFunction pFunction,
			[MarshalAs(UnmanagedType.Bool)] bool fAccurate);

		// HRESULT BreakpointSetError(
		//     [in] ICorDebugAppDomain  *pAppDomain,
		//     [in] ICorDebugThread     *pThread,
		//     [in] ICorDebugBreakpoint *pBreakpoint,
		//     [in] DWORD                dwError
		// );
		void BreakpointSetError(
			ICorDebugAppDomain pAppDomain,
			ICorDebugThread pThread,
			ICorDebugBreakpoint pBreakpoint,
			int dwError);
	}
}
