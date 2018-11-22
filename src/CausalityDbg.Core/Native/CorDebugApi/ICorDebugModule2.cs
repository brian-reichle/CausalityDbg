// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using CausalityDbg.IL;

namespace CausalityDbg.Core.CorDebugApi
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("7FCC5FB5-49C0-41DE-9938-3B88B5B9ADD7")]
	interface ICorDebugModule2
	{
		// HRESULT SetJMCStatus(
		//     [in] BOOL                        bIsJustMyCode,
		//     [in] ULONG32                     cTokens,
		//     [in, size_is(cTokens)] mdToken   pTokens[]
		// );
		void SetJMCStatus_();

		// HRESULT ApplyChanges(
		//     [in] ULONG                       cbMetadata,
		//     [in, size_is(cbMetadata)] BYTE   pbMetadata[],
		//     [in] ULONG                       cbIL,
		//     [in, size_is(cbIL)] BYTE         pbIL[]
		// );
		void ApplyChanges_();

		// HRESULT SetJITCompilerFlags(
		//     [in] DWORD dwFlags
		// );
		[PreserveSig]
		int SetJITCompilerFlags(
			CorDebugJITCompilerFlags dwFlags);

		// HRESULT GetJITCompilerFlags(
		//     [out] DWORD   *pdwFlags
		// );
		void GetJITCompilerFlags_();

		// HRESULT ResolveAssembly(
		//     [in]  mdToken             tkAssemblyRef,
		//     [out] ICorDebugAssembly   **ppAssembly
		// );
		ICorDebugAssembly ResolveAssembly(
			MetaDataToken tkAssemblyRef);
	}
}
