// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;

namespace CausalityDbg.Source.SymbolStoreApi
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("B62B923C-B500-3158-A543-24F307A8B7E1")]
	interface ISymUnmanagedMethod
	{
		// HRESULT GetToken(
		//     [out, retval]  mdMethodDef  *pToken
		// );
		void GetToken_();

		// HRESULT GetSequencePointCount(
		//     [out, retval] ULONG32* pRetVal
		// );
		int GetSequencePointCount();

		// HRESULT GetRootScope(
		//     [out, retval] ISymUnmanagedScope** pRetVal
		// );
		void GetRootScope_();

		// HRESULT GetScopeFromOffset(
		//     [in]  ULONG32 offset,
		//     [out, retval] ISymUnmanagedScope**  pRetVal
		// );
		void GetScopeFromOffset_();

		// HRESULT GetOffset(
		//     [in]  ISymUnmanagedDocument*  document,
		//     [in]  ULONG32                 line,
		//     [in]  ULONG32                 column,
		//     [out, retval] ULONG32*        pRetVal
		// );
		void GetOffset_();

		// HRESULT GetRanges(
		//     [in]  ISymUnmanagedDocument* document,
		//     [in]  ULONG32                line,
		//     [in]  ULONG32                column,
		//     [in]  ULONG32                cRanges,
		//     [out] ULONG32                *pcRanges,
		//     [out, size_is(cRanges), length_is(*pcRanges)] ULONG32 ranges[]
		// );
		void GetRanges_();

		// HRESULT GetParameters(
		//     [in]  ULONG32  cParams,
		//     [out] ULONG32  *pcParams,
		//     [out, size_is(cParams), length_is(*pcParams)] ISymUnmanagedVariable*  params[]
		// );
		void GetParameters_();

		// HRESULT GetNamespace(
		//     [out] ISymUnmanagedNamespace  **pRetVal
		// );
		void GetNamespace_();

		// HRESULT GetSourceStartEnd(
		//     [in]  ISymUnmanagedDocument  *docs[2],
		//     [in]  ULONG32                lines[2],
		//     [in]  ULONG32                columns[2],
		//     [out] BOOL                   *pRetVal
		// );
		void GetSourceStartEnd_();

		// HRESULT GetSequencePoints(
		//     [in]  ULONG32  cPoints,
		//     [out] ULONG32  *pcPoints,
		//     [in, size_is(cPoints)] ULONG32  offsets[],
		//     [in, size_is(cPoints)] ISymUnmanagedDocument* documents[],
		//     [in, size_is(cPoints)] ULONG32  lines[],
		//     [in, size_is(cPoints)] ULONG32  columns[],
		//     [in, size_is(cPoints)] ULONG32  endLines[],
		//     [in, size_is(cPoints)] ULONG32  endColumns[]
		// );
		void GetSequencePoints(
			int cPoints,
			out int pcPoints,
			[MarshalAs(UnmanagedType.LPArray)] int[] offsets,
			[Out, MarshalAs(UnmanagedType.LPArray)] ISymUnmanagedDocument[] documents,
			[MarshalAs(UnmanagedType.LPArray)] int[] lines,
			[MarshalAs(UnmanagedType.LPArray)] int[] columns,
			[MarshalAs(UnmanagedType.LPArray)] int[] endLines,
			[MarshalAs(UnmanagedType.LPArray)] int[] endColumns);
	}
}
