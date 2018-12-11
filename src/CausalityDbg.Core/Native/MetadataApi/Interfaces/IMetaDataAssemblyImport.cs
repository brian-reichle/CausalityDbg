// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using CausalityDbg.IL;

namespace CausalityDbg.Core.MetaDataApi
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("EE62470B-E94B-424e-9B7C-2F00C9249F93")]
	interface IMetaDataAssemblyImport
	{
		// HRESULT GetAssemblyProps(
		//     [in]  mdAssembly          mda,
		//     [out] const void          **ppbPublicKey,
		//     [out] ULONG               *pcbPublicKey,
		//     [out] ULONG               *pulHashAlgId,
		//     [out] LPWSTR              szName,
		//     [in] ULONG                cchName,
		//     [out] ULONG               *pchName,
		//     [out] ASSEMBLYMETADATA    *pMetaData,
		//     [out] DWORD               *pdwAssemblyFlags
		// );
		void GetAssemblyProps(
			MetaDataToken mda,
			out IntPtr ppbPublicKey,
			out int pcbPublicKey,
			IntPtr pulHashAlgId,
			[MarshalAs(UnmanagedType.LPArray)] char[] szName,
			int cchName,
			out int pchName,
			IntPtr pMetaData,
			IntPtr pdwAssemblyFlags);

		// HRESULT GetAssemblyRefProps(
		//     [in]  mdAssemblyRef        mdar,
		//     [out] const void          **ppbPublicKeyOrToken,
		//     [out] ULONG                *pcbPublicKeyOrToken,
		//     [out] LPWSTR               szName,
		//     [in]  ULONG                cchName,
		//     [out] ULONG                *pchName,
		//     [out] ASSEMBLYMETADATA     *pMetaData,
		//     [out] const void           **ppbHashValue,
		//     [out] ULONG                *pcbHashValue,
		//     [out] DWORD                *pdwAssemblyRefFlags
		// );
		void GetAssemblyRefProps_();

		// HRESULT GetFileProps(
		//     [in]  mdFile      mdf,
		//     [out] LPWSTR      szName,
		//     [in]  ULONG       cchName,
		//     [out] ULONG       *pchName,
		//     [out] const void  **ppbHashValue,
		//     [out] ULONG       *pcbHashValue,
		//     [out] DWORD       *pdwFileFlags
		// );
		void GetFileProps_();

		// HRESULT GetExportedTypeProps(
		//     [in]  mdExportedType    mdct,
		//     [out] LPWSTR            szName,
		//     [in]  ULONG             cchName,
		//     [out] ULONG             *pchName,
		//     [out] mdToken           *ptkImplementation,
		//     [out] mdTypeDef         *ptkTypeDef,
		//     [out] DWORD             *pdwExportedTypeFlags
		// );
		void GetExportedTypeProps(
			MetaDataToken mdct,
			[MarshalAs(UnmanagedType.LPArray)] char[] szName,
			int cchName,
			out int pchName,
			out MetaDataToken ptkImplementation,
			out MetaDataToken ptkTypeDef,
			out TypeAttributes pdwExportedTypeFlags);

		// HRESULT GetManifestResourceProps(
		//     [in]  mdManifestResource   mdmr,
		//     [out] LPWSTR               szName,
		//     [in]  ULONG                cchName,
		//     [out] ULONG                *pchName,
		//     [out] mdToken              *ptkImplementation,
		//     [out] DWORD                *pdwOffset,
		//     [out] DWORD                *pdwResourceFlags
		// );
		void GetManifestResourceProps_();

		// HRESULT EnumAssemblyRefs(
		//     [in, out] HCORENUM        *phEnum,
		//     [out]     mdAssemblyRef   rAssemblyRefs[],
		//     [in]      ULONG           cMax,
		//     [out]     ULONG           *pcTokens
		// );
		void EnumAssemblyRefs_();

		// HRESULT EnumFiles(
		//     [in, out] HCORENUM    *phEnum,
		//     [out] mdFile          rFiles[],
		//     [in]  ULONG           cMax,
		//     [out] ULONG           *pcTokens
		// );
		void EnumFiles_();

		// HRESULT EnumExportedTypes(
		//     [in, out] HCORENUM     *phEnum,
		//     [out] mdExportedType   rExportedTypes[],
		//     [in]  ULONG            cMax,
		//     [out] ULONG            *pcTokens
		// );
		void EnumExportedTypes_();

		// HRESULT EnumManifestResources(
		//     [in, out] HCORENUM         *phEnum,
		//     [out] mdManifestResource   rManifestResources[],
		//     [in]  ULONG                cMax,
		//     [out] ULONG                *pcTokens
		// );
		void EnumManifestResources_();

		// HRESULT GetAssemblyFromScope(
		//     [out] mdAssembly  *ptkAssembly
		// );
		[PreserveSig]
		int GetAssemblyFromScope(out MetaDataToken ptkAssembly);

		// HRESULT FindExportedTypeByName(
		//     [in]  LPCWSTR           szName,
		//     [in]  mdToken           mdtExportedType,
		//     [out] mdExportedType    *ptkExportedType
		// );
		[PreserveSig]
		int FindExportedTypeByName(
			[MarshalAs(UnmanagedType.LPWStr)] string szName,
			MetaDataToken mdToken,
			out MetaDataToken ptkExportedType);

		// HRESULT FindManifestResourceByName(
		//     [in]  LPCWSTR                szName,
		//     [out] mdManifestResource     *ptkManifestResource
		// );
		void FindManifestResourceByName_();

		// void CloseEnum(
		//     [in] HCORENUM     hEnum
		// );
		void CloseEnum_();

		// HRESULT FindAssembliesByName(
		//     [in]  LPCWSTR     szAppBase,
		//     [in]  LPCWSTR     szPrivateBin,
		//     [in]  LPCWSTR     szAssemblyName,
		//     [out] IUnknown    *ppIUnk[],
		//     [in]  ULONG       cMax,
		//     [out] ULONG       *pcAssemblies
		// );
		void FindAssembliesByName_();
	}
}
