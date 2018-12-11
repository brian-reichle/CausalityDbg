// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using CausalityDbg.IL;

namespace CausalityDbg.Core.SymbolStoreApi
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("B4CE6286-2A6B-3712-A3B7-1EE1DAD467B5")]
	interface ISymUnmanagedReader
	{
		// HRESULT GetDocument(
		//     [in]  WCHAR  *url,
		//     [in]  GUID   language,
		//     [in]  GUID   languageVendor,
		//     [in]  GUID   documentType,
		//     [out, retval] ISymUnmanagedDocument** pRetVal
		// );
		void GetDocument_();

		// HRESULT GetDocuments(
		//     [in]  ULONG32  cDocs,
		//     [out] ULONG32  *pcDocs,
		//     [out, size_is (cDocs), length_is (*pcDocs)] ISymUnmanagedDocument *pDocs[]
		// );
		void GetDocuments_();

		// HRESULT GetUserEntryPoint(
		//     [out, retval]  mdMethodDef  *pToken
		// );
		void GetUserEntryPoint_();

		// HRESULT GetMethod(
		//     [in]  mdMethodDef  token,
		//     [out, retval] ISymUnmanagedMethod**  pRetVal
		// );
		ISymUnmanagedMethod GetMethod(
			MetaDataToken methodToken);

		// HRESULT GetMethodByVersion(
		//     [in]  mdMethodDef  token,
		//     [in]  int  version,
		//     [out, retval] ISymUnmanagedMethod** pRetVal
		// );
		void GetMethodByVersion_();

		// HRESULT GetVariables(
		//     [in]  mdToken  parent,
		//     [in]  ULONG32  cVars,
		//     [out] ULONG32  *pcVars,
		//     [out, size_is (cVars), length_is (*pcVars)] ISymUnmanagedVariable *pVars[]
		// );
		void GetVariables_();

		// HRESULT GetGlobalVariables(
		//     [in]  ULONG32  cVars,
		//     [out] ULONG32  *pcVars,
		//     [out, size_is(cVars), length_is(*pcVars)] ISymUnmanagedVariable *pVars[]
		// );
		void GetGlobalVariables_();

		// HRESULT GetMethodFromDocumentPosition(
		//     [in]  ISymUnmanagedDocument*  document,
		//     [in]  ULONG32  line,
		//     [in]  ULONG32  column,
		//     [out, retval] ISymUnmanagedMethod**  pRetVal
		// );
		void GetMethodFromDocumentPosition_();

		// HRESULT GetSymAttribute(
		//     [in]  mdToken  parent,
		//     [in]  WCHAR    *name,
		//     [in]  ULONG32  cBuffer,
		//     [out] ULONG32  *pcBuffer,
		//     [out, size_is (cBuffer), length_is (*pcBuffer)] BYTE buffer[]
		// );
		void GetSymAttribute_();

		// HRESULT GetNamespaces(
		//     [in]  ULONG32  cNameSpaces,
		//     [out] ULONG32  *pcNameSpaces,
		//     [out, size_is (cNameSpaces), length_is (*pcNameSpaces)] ISymUnmanagedNamespace*  namespaces[]
		// );
		void GetNamespaces_();

		// HRESULT Initialize(
		//     [in]  IUnknown     *importer,
		//     [in]  const WCHAR  *filename,
		//     [in]  const WCHAR  *searchPath,
		//     [in]  IStream      *pIStream
		// );
		void Initialize_();

		// HRESULT UpdateSymbolStore(
		//     [in] const WCHAR *filename,
		//     [in] IStream *pIStream
		// );
		void UpdateSymbolStore_();

		// HRESULT ReplaceSymbolStore(
		//     [in] const WCHAR *filename,
		//     [in] IStream *pIStream
		// );
		void ReplaceSymbolStore_();

		// HRESULT GetSymbolStoreFileName(
		//     [in]  ULONG32 cchName,
		//     [out] ULONG32 *pcchName,
		//     [out, size_is (cchName), length_is (*pcchName)] WCHAR szName[]
		// );
		void GetSymbolStoreFileName_();

		// HRESULT GetMethodsFromDocumentPosition(
		//     [in]  ISymUnmanagedDocument* document,
		//     [in]  ULONG32 line,
		//     [in]  ULONG32 column,
		//     [in]  ULONG32 cMethod,
		//     [out] ULONG32* pcMethod,
		//     [out, size_is (cMethod), length_is (*pcMethod)] ISymUnmanagedMethod* pRetVal[]
		// );
		void GetMethodsFromDocumentPosition_();

		// HRESULT GetDocumentVersion(
		//     [in]  ISymUnmanagedDocument *pDoc,
		//     [out] int* version,
		//     [out] BOOL* pbCurrent
		// );
		void GetDocumentVersion_();

		// HRESULT GetMethodVersion(
		//     [in]  ISymUnmanagedMethod* pMethod,
		//     [out] int* version
		// );
		void GetMethodVersion_();
	}
}
