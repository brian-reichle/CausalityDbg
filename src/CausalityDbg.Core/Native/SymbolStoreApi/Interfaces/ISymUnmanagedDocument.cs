// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;

namespace CausalityDbg.Core.SymbolStoreApi
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("40DE4037-7C81-3E1E-B022-AE1ABFF2CA08")]
	interface ISymUnmanagedDocument
	{
		// HRESULT GetURL(
		//     [in]  ULONG32  cchUrl,
		//     [out] ULONG32  *pcchUrl,
		//     [out, size_is(cchUrl), length_is(*pcchUrl)] WCHAR szUrl[]
		// );
		void GetURL(
			int cchUrl,
			out int pcchUrl,
			[MarshalAs(UnmanagedType.LPArray)] char[] szUrl);

		// HRESULT GetDocumentType(
		//     [out, retval] GUID*  pRetVal
		// );
		void GetDocumentType_();

		// HRESULT GetLanguage(
		//     [out, retval]  GUID*  pRetVal
		// );
		void GetLanguage_();

		// HRESULT GetLanguageVendor(
		//     [out, retval]  GUID*  pRetVal
		// );
		void GetLanguageVendor_();

		// HRESULT GetCheckSumAlgorithmId(
		//     [out, retval] GUID*  pRetVal
		// );
		void GetCheckSumAlgorithmId_();

		// HRESULT GetCheckSum(
		//     [in]  ULONG32  cData,
		//     [out] ULONG32  *pcData,
		//     [out, size_is(cData), length_is(*pcData)] BYTE data[]
		// );
		void GetCheckSum_();

		// HRESULT FindClosestLine(
		//     [in]  ULONG32  line,
		//     [out, retval] ULONG32*  pRetVal
		// );
		void FindClosestLine_();

		// HRESULT HasEmbeddedSource(
		//    [out, retval]  BOOL  *pRetVal
		// );
		void HasEmbeddedSource_();

		// HRESULT GetSourceLength(
		//     [out, retval]  ULONG32*  pRetVal
		// );
		void GetSourceLength_();

		// HRESULT GetSourceRange(
		//     [in]  ULONG32  startLine,
		//     [in]  ULONG32  startColumn,
		//     [in]  ULONG32  endLine,
		//     [in]  ULONG32  endColumn,
		//     [in]  ULONG32  cSourceBytes,
		//     [out] ULONG32  *pcSourceBytes,
		//     [out, size_is(cSourceBytes), length_is(*pcSourceBytes)] BYTE source[]
		// );
		void GetSourceRange_();
	}
}
