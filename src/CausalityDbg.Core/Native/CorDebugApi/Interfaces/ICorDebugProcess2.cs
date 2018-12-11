// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;

namespace CausalityDbg.Core.CorDebugApi
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("AD1B3588-0EF0-4744-A496-AA09A9F80371")]
	interface ICorDebugProcess2
	{
		// HRESULT GetThreadForTaskID(
		//     [in] TASKID taskid,
		//     [out] ICorDebugThread2 **ppThread
		// );
		void GetThreadForTaskID_();

		// HRESULT GetVersion(
		//     [out] COR_VERSION *version
		// );
		void GetVersion_();

		// HRESULT SetUnmanagedBreakpoint(
		//     [in] CORDB_ADDRESS address,
		//     [in] ULONG32 bufsize,
		//     [length_is][size_is][out] BYTE buffer[  ],
		//     [out]  ULONG32 *bufLen) = 0;
		void SetUnmanagedBreakpoint_();

		// HRESULT ClearUnmanagedBreakpoint(
		//     [in] CORDB_ADDRESS address
		// );
		void ClearUnmanagedBreakpoint_();

		// HRESULT SetDesiredNGENCompilerFlags(
		//     [in] DWORD pdwFlags
		// );
		[PreserveSig]
		int SetDesiredNGENCompilerFlags(
			CorDebugJITCompilerFlags flags);

		// HRESULT GetDesiredNGENCompilerFlags(
		//     [out] DWORD *pdwFlags
		// );
		void GetDesiredNGENCompilerFlags_();

		// HRESULT GetReferenceValueFromGCHandle(
		//     [in] UINT_PTR handle,
		//     [out] ICorDebugReferenceValue **pOutValue
		// );
		void GetReferenceValueFromGCHandle_();
	}
}
