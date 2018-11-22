// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;

namespace CausalityDbg.Core.CorDebugApi
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("CC7BCAEC-8A68-11D2-983C-0000F808342D")]
	interface ICorDebugStepper
	{
		// HRESULT IsActive(
		//     [out] BOOL   *pbActive
		// );
		void IsActive_();

		// HRESULT Deactivate();
		void Deactivate();

		// HRESULT SetInterceptMask(
		//     [in] CorDebugIntercept    mask
		// );
		void SetInterceptMask(
			CorDebugIntercept mask);

		// HRESULT SetUnmappedStopMask(
		//     [in] CorDebugUnmappedStop   mask
		// );
		void SetUnmappedStopMask_();

		// HRESULT Step(
		//     [in] BOOL   bStepIn
		// );
		void Step(
			[MarshalAs(UnmanagedType.Bool)] bool bStepIn);

		// HRESULT StepRange(
		//     [in] BOOL     bStepIn,
		//     [in, size_is(cRangeCount)] COR_DEBUG_STEP_RANGE ranges[],
		//     [in] ULONG32  cRangeCount
		// );
		void StepRange(
			[MarshalAs(UnmanagedType.Bool)] bool bStepIn,
			[MarshalAs(UnmanagedType.LPArray)] COR_DEBUG_STEP_RANGE[] ranges,
			uint cRangeCount);

		// HRESULT StepOut();
		void StepOut();

		// HRESULT SetRangeIL(
		//     [in] BOOL    bIL
		// );
		void SetRangeIL_();
	}
}
