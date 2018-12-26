// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using CausalityDbg.IL;

namespace CausalityDbg.Core.MetaDataApi
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("FCE5EFA0-8BBA-4f8e-A036-8F2022B08466")]
	interface IMetaDataImport2 : IMetaDataImport
	{
		// void CloseEnum(
		//     [in] HCORENUM hEnum
		// );
		new void CloseEnum(
			IntPtr hEnum);

		// HRESULT CountEnum(
		//     [in]  HCORENUM    hEnum,
		//     [out] ULONG       *pulCount
		// );
		new int CountEnum(
			IntPtr hEnum);

		// HRESULT ResetEnum(
		//     [in] HCORENUM    hEnum,
		//     [in] ULONG       ulPos
		// );
		new void ResetEnum_();

		// HRESULT EnumTypeDefs(
		//     [out] HCORENUM   *phEnum,
		//     [in]  mdTypeDef  rTypeDefs[],
		//     [in]  ULONG      cMax,
		//     [out] ULONG      *pcTypeDefs
		// );
		new void EnumTypeDefs_();

		// HRESULT EnumInterfaceImpls(
		//     [in, out]  HCORENUM       *phEnum,
		//     [in]   mdTypeDef          td,
		//     [out]  mdInterfaceImpl    rImpls[],
		//     [in]   ULONG              cMax,
		//     [out]  ULONG*             pcImpls
		// );
		new void EnumInterfaceImpls_();

		// HRESULT EnumTypeRefs(
		//     [in, out] HCORENUM    *phEnum,
		//     [out] mdTypeRef       rTypeRefs[],
		//     [in]  ULONG           cMax,
		//     [out] ULONG           *pcTypeRefs
		// );
		new void EnumTypeRefs_();

		// HRESULT FindTypeDefByName(
		//     [in]  LPCWSTR       szTypeDef,
		//     [in]  mdToken       tkEnclosingClass,
		//     [out] mdTypeDef     *ptd
		// );
		new int FindTypeDefByName(
			[MarshalAs(UnmanagedType.LPWStr)] string szTypeDef,
			MetaDataToken tkEnclosingClass,
			out MetaDataToken ptd);

		// HRESULT GetScopeProps(
		//     [out] LPWSTR           szName,
		//     [in]  ULONG            cchName,
		//     [out] ULONG            *pchName,
		//     [out, optional] GUID   *pmvid
		// );
		new void GetScopeProps_();

		// HRESULT GetModuleFromScope(
		//     [out] mdModule    *pmd
		// );
		new void GetModuleFromScope_();

		// HRESULT GetTypeDefProps(
		//     [in]  mdTypeDef   td,
		//     [out] LPWSTR      szTypeDef,
		//     [in]  ULONG       cchTypeDef,
		//     [out] ULONG       *pchTypeDef,
		//     [out] DWORD       *pdwTypeDefFlags,
		//     [out] mdToken     *ptkExtends
		// );
		new void GetTypeDefProps(
			MetaDataToken td,
			[MarshalAs(UnmanagedType.LPArray)] char[] szTypeDef,
			int cchTypeDef,
			out int pchTypeDef,
			out TypeAttributes pdwTypeDefFlags,
			out MetaDataToken ptkExtends);

		// HRESULT GetInterfaceImplProps(
		//     [in]  mdInterfaceImpl        iiImpl,
		//     [out] mdTypeDef              *pClass,
		//     [out] mdToken                *ptkIface
		// );
		new void GetInterfaceImplProps_();

		// HRESULT GetTypeRefProps(
		//     [in]  mdTypeRef   tr,
		//     [out] mdToken     *ptkResolutionScope,
		//     [out] LPWSTR      szName,
		//     [in]  ULONG       cchName,
		//     [out] ULONG       *pchName
		// );
		new void GetTypeRefProps(
			MetaDataToken tr,
			out MetaDataToken ptkResolutionScope,
			[MarshalAs(UnmanagedType.LPArray)] char[] szName,
			int cchName,
			out int pchName);

		// HRESULT ResolveTypeRef(
		//     [in]  mdTypeRef       tr,
		//     [in]  REFIID          riid,
		//     [out] IUnknown        **ppIScope,
		//     [out] mdTypeDef       *ptd
		// );
		new void ResolveTypeRef_();

		// HRESULT EnumMembers(
		//     [in, out]  HCORENUM    *phEnum,
		//     [in]  mdTypeDef   cl,
		//     [out] mdToken     rMembers[],
		//     [in]  ULONG       cMax,
		//     [out] ULONG       *pcTokens
		// );
		new void EnumMembers_();

		// HRESULT EnumMembersWithName(
		//     [in, out] HCORENUM    *phEnum,
		//     [in]      mdTypeDef   cl,
		//     [in]      LPCWSTR     szName,
		//     [out]     mdToken     rMembers[],
		//     [in]      ULONG       cMax,
		//     [out]     ULONG       *pcTokens
		// );
		new void EnumMembersWithName_();

		// HRESULT EnumMethods(
		//     [in, out] HCORENUM   *phEnum,
		//     [in]  mdTypeDef      cl,
		//     [out] mdMethodDef    rMethods[],
		//     [in]  ULONG          cMax,
		//     [out] ULONG          *pcTokens
		// );
		new int EnumMethods(
			ref IntPtr phEnum,
			MetaDataToken cl,
			out MetaDataToken mdMethodDef,
			int cMax);

		// HRESULT EnumMethodsWithName(
		//     [in, out] HCORENUM    *phEnum,
		//     [in]  mdTypeDef       cl,
		//     [in]  LPCWSTR         szName,
		//     [out] mdMethodDef     rMethods[],
		//     [in]  ULONG           cMax,
		//     [out] ULONG           *pcTokens
		// );
		new void EnumMethodsWithName_();

		// HRESULT EnumFields(
		//     [in, out] HCORENUM    *phEnum,
		//     [in]      mdTypeDef   cl,
		//     [out]     mdFieldDef  rFields[],
		//     [in]      ULONG       cMax,
		//     [out]     ULONG       *pcTokens
		// );
		new void EnumFields_();

		// HRESULT EnumFieldsWithName(
		//     [in, out] HCORENUM    *phEnum,
		//     [in]  mdTypeDef       cl,
		//     [in]  LPCWSTR         szName,
		//     [out] mdFieldDef      rFields[],
		//     [in]  ULONG           cMax,
		//     [out] ULONG           *pcTokens
		// );
		new void EnumFieldsWithName_();

		// HRESULT EnumParams(
		//     [in, out] HCORENUM    *phEnum,
		//     [in]  mdMethodDef     mb,
		//     [out] mdParamDef      rParams[],
		//     [in]  ULONG           cMax,
		//     [out] ULONG          *pcTokens
		// );
		[return: MarshalAs(UnmanagedType.Bool)]
		new bool EnumParams(
			ref IntPtr phEnum,
			MetaDataToken mb,
			out MetaDataToken rParams,
			int cMax);

		// HRESULT EnumMemberRefs(
		//     [in, out] HCORENUM    *phEnum,
		//     [in]   mdToken        tkParent,
		//     [out]  mdMemberRef    rMemberRefs[],
		//     [in]   ULONG          cMax,
		//     [out]  ULONG          *pcTokens
		// );
		new void EnumMemberRefs_();

		// HRESULT EnumMethodImpls(
		//     [in, out] HCORENUM    *phEnum,
		//     [in]      mdTypeDef   td,
		//     [out]     mdToken     rMethodBody[],
		//     [out]     mdToken     rMethodDecl[],
		//     [in]      ULONG       cMax,
		//     [in]      ULONG       *pcTokens
		// );
		new void EnumMethodImpls_();

		// HRESULT EnumPermissionSets(
		//     [in, out] HCORENUM      *phEnum,
		//     [in]      mdToken       tk,
		//     [in]      DWORD         dwActions,
		//     [out]     mdPermission  rPermission[],
		//     [in]      ULONG         cMax,
		//     [out]     ULONG         *pcTokens
		// );
		new void EnumPermissionSets_();

		// HRESULT FindMember(
		//     [in]  mdTypeDef         td,
		//     [in]  LPCWSTR           szName,
		//     [in]  PCCOR_SIGNATURE   pvSigBlob,
		//     [in]  ULONG             cbSigBlob,
		//     [out] mdToken           *pmb
		// );
		new void FindMember_();

		// HRESULT FindMethod(
		//     [in]  mdTypeDef          td,
		//     [in]  LPCWSTR            szName,
		//     [in]  PCCOR_SIGNATURE    pvSigBlob,
		//     [in]  ULONG              cbSigBlob,
		//     [out] mdMethodDef        *pmb
		// );
		new void FindMethod_();

		// HRESULT FindField(
		//     [in]  mdTypeDef         td,
		//     [in]  LPCWSTR           szName,
		//     [in]  PCCOR_SIGNATURE   pvSigBlob,
		//     [in]  ULONG             cbSigBlob,
		//     [out] mdFieldDef        *pmb
		// );
		new void FindField_();

		// HRESULT FindMemberRef(
		//     [in]  mdTypeRef          td,
		//     [in]  LPCWSTR            szName,
		//     [in]  PCCOR_SIGNATURE    pvSigBlob,
		//     [in]  ULONG              cbSigBlob,
		//     [out] mdMemberRef        *pmr
		// );
		new void FindMemberRef_();

		// HRESULT GetMethodProps(
		//     [in]  mdMethodDef         mb,
		//     [out] mdTypeDef           *pClass,
		//     [out] LPWSTR              szMethod,
		//     [in]  ULONG               cchMethod,
		//     [out] ULONG               *pchMethod,
		//     [out] DWORD               *pdwAttr,
		//     [out] PCCOR_SIGNATURE     *ppvSigBlob,
		//     [out] ULONG               *pcbSigBlob,
		//     [out] ULONG               *pulCodeRVA,
		//     [out] DWORD               *pdwImplFlags
		// );
		new void GetMethodProps(
			MetaDataToken md,
			out MetaDataToken pClass,
			[MarshalAs(UnmanagedType.LPArray)] char[] szMethod,
			int cchMethod,
			out int pchMethod,
			IntPtr pdwAttr,
			out IntPtr ppvSigBlob,
			out int pcbSigBlob,
			out int pulCodeRVA,
			IntPtr pdwImplFlags);

		// HRESULT GetMemberRefProps(
		//     [in]  mdMemberRef       mr,
		//     [out] mdToken           *ptk,
		//     [out] LPWSTR            szMember,
		//     [in]  ULONG             cchMember,
		//     [out] ULONG             *pchMember,
		//     [out] PCCOR_SIGNATURE   *ppvSigBlob,
		//     [out] ULONG             *pbSig
		// );
		new void GetMemberRefProps_();

		// HRESULT EnumProperties(
		//     [in, out] HCORENUM    *phEnum,
		//     [in]      mdTypeDef   td,
		//     [out]     mdProperty  rProperties[],
		//     [in]      ULONG       cMax,
		//     [out]     ULONG       *pcProperties
		// );
		[return: MarshalAs(UnmanagedType.Bool)]
		new bool EnumProperties(
			ref IntPtr phEnum,
			MetaDataToken td,
			out MetaDataToken rProperties,
			int cMax);

		// HRESULT EnumEvents(
		//     [in, out] HCORENUM    *phEnum,
		//     [in]      mdTypeDef   td,
		//     [out]     mdEvent     rEvents[],
		//     [in]      ULONG       cMax,
		//     [out]    ULONG        *pcEvents
		// );
		new void EnumEvents_();

		// HRESULT GetEventProps(
		//     [in]  mdEvent       ev,
		//     [out] mdTypeDef     *pClass,
		//     [out] LPCWSTR       szEvent,
		//     [in]  ULONG         cchEvent,
		//     [out] ULONG         *pchEvent,
		//     [out] DWORD         *pdwEventFlags,
		//     [out] mdToken       *ptkEventType,
		//     [out] mdMethodDef   *pmdAddOn,
		//     [out] mdMethodDef   *pmdRemoveOn,
		//     [out] mdMethodDef   *pmdFire,
		//     [out] mdMethodDef   rmdOtherMethod[],
		//     [in]  ULONG         cMax,
		//     [out] ULONG         *pcOtherMethod
		// );
		new void GetEventProps_();

		// HRESULT EnumMethodSemantics(
		//     [in, out] HCORENUM    *phEnum,
		//     [in]  mdMethodDef     mb,
		//     [out] mdToken         rEventProp[],
		//     [in]  ULONG           cMax,
		//     [out] ULONG           *pcEventProp
		// );
		new void EnumMethodSemantics_();

		// HRESULT GetMethodSemantics(
		//     [in]  mdMethodDef   mb,
		//     [in]  mdToken       tkEventProp,
		//     [out] DWORD         *pdwSemanticsFlags
		// );
		new void GetMethodSemantics_();

		// HRESULT GetClassLayout(
		//     [in]  mdTypeDef          td,
		//     [out] DWORD              *pdwPackSize,
		//     [out] COR_FIELD_OFFSET   rFieldOffset[],
		//     [in]  ULONG              cMax,
		//     [out] ULONG              *pcFieldOffset,
		//     [out] ULONG              *pulClassSize
		// );
		new void GetClassLayout_();

		// HRESULT GetFieldMarshal(
		//     [in]  mdToken             tk,
		//     [out] PCCOR_SIGNATURE     *ppvNativeType,
		//     [out] ULONG               *pcbNativeType
		// );
		new void GetFieldMarshal_();

		// HRESULT GetRVA(
		//     [in]  mdToken     tk,
		//     [out] ULONG       *pulCodeRVA,
		//     [out]  DWORD      *pdwImplFlags
		// );
		new void GetRVA_();

		// HRESULT GetPermissionSetProps(
		//     [in]  mdPermission      pm,
		//     [out] DWORD             *pdwAction,
		//     [out] void const        **ppvPermission,
		//     [out] ULONG             *pcbPermission
		// );
		new void GetPermissionSetProps_();

		// HRESULT GetSigFromToken(
		//     [in]   mdSignature        mdSig,
		//     [out]  PCCOR_SIGNATURE    *ppvSig,
		//     [out]  ULONG              *pcbSig
		// );
		new void GetSigFromToken_();

		// HRESULT GetModuleRefProps(
		//     [in]  mdModuleRef         mur,
		//     [out] LPWSTR              szName,
		//     [in]  ULONG               cchName,
		//     [out] ULONG               *pchName
		// );
		new void GetModuleRefProps_();

		// HRESULT EnumModuleRefs(
		//     [in, out] HCORENUM     *phEnum,
		//     [out]     mdModuleRef  rModuleRefs[],
		//     [in]      ULONG        cMax,
		//     [out]     ULONG        *pcModuleRefs
		// );
		new void EnumModuleRefs_();

		// HRESULT GetTypeSpecFromToken(
		//     [in]  mdTypeSpec            typespec,
		//     [out] PCCOR_SIGNATURE       *ppvSig,
		//     [out] ULONG                 *pcbSig
		// );
		new void GetTypeSpecFromToken_();

		// HRESULT GetNameFromToken(
		//     [in] mdToken      tk,
		//     [out] MDUTF8CSTR  *pszUtf8NamePtr
		// );
		new void GetNameFromToken_();

		// HRESULT EnumUnresolvedMethods(
		//     [in, out] HCORENUM    *phEnum,
		//     [out]     mdToken     rMethods[],
		//     [in]      ULONG       cMax,
		//     [out]     ULONG       *pcTokens
		// );
		new void EnumUnresolvedMethods_();

		// HRESULT GetUserString(
		//     [in]   mdString    stk,
		//     [out]  LPWSTR      szString,
		//     [in]   ULONG       cchString,
		//     [out]  ULONG       *pchString
		// );
		new void GetUserString_();

		// HRESULT GetPinvokeMap(
		//     [in]  mdToken       tk,
		//     [out] DWORD         *pdwMappingFlags,
		//     [out] LPWSTR        szImportName,
		//     [in]  ULONG         cchImportName,
		//     [out] ULONG         *pchImportName,
		//     [out] mdModuleRef   *pmrImportDLL
		// );
		new void GetPinvokeMap_();

		// HRESULT EnumSignatures(
		//     [in, out] HCORENUM     *phEnum,
		//     [out]     mdSignature  rSignatures[],
		//     [in]      ULONG        cMax,
		//     [out]     ULONG        *pcSignatures
		// );
		new void EnumSignatures_();

		// HRESULT EnumTypeSpecs(
		//     [in, out] HCORENUM    *phEnum,
		//     [out] mdTypeSpec      rTypeSpecs[],
		//     [in]  ULONG           cMax,
		//     [out] ULONG           *pcTypeSpecs
		// );
		new void EnumTypeSpecs_();

		// HRESULT EnumUserStrings(
		//     [in, out]  HCORENUM    *phEnum,
		//     [out]  mdString        rStrings[],
		//     [in]   ULONG           cMax,
		//     [out]  ULONG           *pcStrings
		// );
		new void EnumUserStrings_();

		// HRESULT GetParamForMethodIndex(
		//     [in]  mdMethodDef      md,
		//     [in]  ULONG            ulParamSeq,
		//     [out] mdParamDef       *ppd
		// );
		[PreserveSig]
		new int GetParamForMethodIndex(
			MetaDataToken md,
			uint ulParamSeq,
			out MetaDataToken ppd);

		// HRESULT EnumCustomAttributes(
		//     [in, out] HCORENUM      *phEnum,
		//     [in]  mdToken            tk,
		//     [in]  mdToken            tkType,
		//     [out] mdCustomAttribute  rCustomAttributes[],
		//     [in]  ULONG              cMax,
		//     [out, optional] ULONG   *pcCustomAttributes
		// );
		new void EnumCustomAttributes_();

		// HRESULT GetCustomAttributeProps(
		//     [in]            mdCustomAttribute   cv,
		//     [out, optional] mdToken             *ptkObj,
		//     [out, optional] mdToken             *ptkType,
		//     [out, optional] void const          **ppBlob,
		//     [out, optional] ULONG               *pcbSize
		// );
		new void GetCustomAttributeProps_();

		// HRESULT FindTypeRef(
		//     [in] mdToken        tkResolutionScope,
		//     [in]  LPCWSTR       szName,
		//     [out] mdTypeRef     *ptr
		// );
		new void FindTypeRef_();

		// HRESULT GetMemberProps(
		//     [in]  mdToken           mb,
		//     [out] mdTypeDef         *pClass,
		//     [out] LPWSTR            szMember,
		//     [in]  ULONG             cchMember,
		//     [out] ULONG             *pchMember,
		//     [out] DWORD             *pdwAttr,
		//     [out] PCCOR_SIGNATURE   *ppvSigBlob,
		//     [out] ULONG             *pcbSigBlob,
		//     [out] ULONG             *pulCodeRVA,
		//     [out] DWORD             *pdwImplFlags,
		//     [out] DWORD             *pdwCPlusTypeFlag,
		//     [out] UVCP_CONSTANT     *ppValue,
		//     [out] ULONG             *pcchValue
		// );
		new void GetMemberProps_();

		// HRESULT GetFieldProps(
		//     [in]  mdFieldDef        mb,
		//     [out] mdTypeDef         *pClass,
		//     [out] LPWSTR            szField,
		//     [in]  ULONG             cchField,
		//     [out] ULONG             *pchField,
		//     [out] DWORD             *pdwAttr,
		//     [in]  PCCOR_SIGNATURE   *ppvSigBlob,
		//     [out] ULONG             *pcbSigBlob,
		//     [out] DWORD             *pdwCPlusTypeFlag,
		//     [out] UVCP_CONSTANT     *ppValue,
		//     [out] ULONG             *pcchValue
		// );
		new void GetFieldProps_();

		// HRESULT GetPropertyProps(
		//     [in]  mdProperty        prop,
		//     [out] mdTypeDef         *pClass,
		//     [out] LPCWSTR           szProperty,
		//     [in]  ULONG             cchProperty,
		//     [out] ULONG             *pchProperty,
		//     [out] DWORD             *pdwPropFlags,
		//     [out] PCCOR_SIGNATURE   *ppvSig,
		//     [out] ULONG             *pbSig,
		//     [out] DWORD             *pdwCPlusTypeFlag,
		//     [out] UVCP_CONSTANT     *ppDefaultValue,
		//     [out] ULONG             *pcchDefaultValue,
		//     [out] mdMethodDef       *pmdSetter,
		//     [out] mdMethodDef       *pmdGetter,
		//     [out] mdMethodDef       rmdOtherMethod[],
		//     [in]  ULONG             cMax,
		//     [out] ULONG             *pcOtherMethod
		// );
		new void GetPropertyProps(
			MetaDataToken prop,
			IntPtr pClass,
			[MarshalAs(UnmanagedType.LPArray)] char[] szProperty,
			int cchProperty,
			out int pchProperty,
			IntPtr pdwPropFlags,
			IntPtr ppvSig,
			IntPtr pbSig,
			IntPtr pdwCPlusTypeFlag,
			IntPtr ppDefaultValue,
			IntPtr pchDefaultValue,
			IntPtr pmdSetter,
			out MetaDataToken pmdGetter,
			IntPtr rmdOtherMethod,
			int cMax,
			IntPtr pcOtherMethod);

		// HRESULT GetParamProps(
		//     [in]  mdParamDef      tk,
		//     [out] mdMethodDef     *pmd,
		//     [out] ULONG           *pulSequence,
		//     [out] LPWSTR          szName,
		//     [in]  ULONG           cchName,
		//     [out] ULONG           *pchName,
		//     [out] DWORD           *pdwAttr,
		//     [out] DWORD           *pdwCPlusTypeFlag,
		//     [out] UVCP_CONSTANT   *ppValue,
		//     [out] ULONG           *pcchValue
		// );
		new void GetParamProps(
			MetaDataToken tk,
			IntPtr pmd,
			IntPtr pulSequence,
			[MarshalAs(UnmanagedType.LPArray)] char[] szName,
			int cchName,
			out int pchName,
			IntPtr pdwAttr,
			IntPtr pdwCPlusTypeFlag,
			IntPtr ppValue,
			IntPtr pcchValue);

		// HRESULT GetCustomAttributeByName(
		//     [in]  mdToken          tkObj,
		//     [in]  LPCWSTR          szName,
		//     [out] const void       **ppData,
		//     [out] ULONG            *pcbData
		// );
		new int GetCustomAttributeByName_();

		// BOOL IsValidToken(
		//     [in] mdToken     tk
		// );
		new bool IsValidToken_();

		// HRESULT GetNestedClassProps(
		//     [in]   mdTypeDef      tdNestedClass,
		//     [out]  mdTypeDef      *ptdEnclosingClass
		// );
		new void GetNestedClassProps(
			MetaDataToken tdNestedClass,
			out MetaDataToken tdEnclosingClass);

		// HRESULT GetNativeCallConvFromSig(
		//     [in]  void const  *pvSig,
		//     [in]  ULONG       cbSig,
		//     [out] ULONG       *pCallConv
		// );
		new void GetNativeCallConvFromSig_();

		// HRESULT IsGlobal(
		//     [in]  mdToken     pd,
		//     [out] int         *pbGlobal
		// );
		new void IsGlobal_();

		// HRESULT EnumGenericParams(
		//     [in, out] HCORENUM     *phEnum,
		//     [in]  mdToken          tk,
		//     [out] mdGenericParam   rGenericParams[],
		//     [in]  ULONG            cMax,
		//     [out] ULONG            *pcGenericParams
		// );
		[return: MarshalAs(UnmanagedType.Bool)]
		bool EnumGenericParams(
			ref IntPtr phEnum,
			MetaDataToken tk,
			out MetaDataToken rGenericParams,
			int cMax);

		// HRESULT GetGenericParamProps(
		//     [in]  mdGenericParam  gp,
		//     [out] ULONG           *pulParamSeq,
		//     [out] DWORD           *pdwParamFlags,
		//     [out] mdToken         *ptOwner,
		//     [out] DWORD           *reserved,
		//     [out] LPWSTR          wzName,
		//     [in]  ULONG           cchName,
		//     [out] ULONG           *pchName
		// );
		void GetGenericParamProps();

		// HRESULT GetMethodSpecProps(
		//     [in]  mdMethodSpec     mi,
		//     [out] mdToken          *tkParent,
		//     [out] PCCOR_SIGNATURE  *ppvSigBlob,
		//     [out] ULONG            *pcbSigBlob
		// );
		void GetMethodSpecProps_();

		// HRESULT EnumGenericParamConstraints(
		//     [in, out] HCORENUM                *phEnum,
		//     [in]  mdGenericParam              tk,
		//     [out] mdGenericParamConstraint    rGenericParamConstraints[],
		//     [in]  ULONG                       cMax,
		//     [out] ULONG                       *pcGenericParamConstraints
		// );
		void EnumGenericParamConstraints_();

		// HRESULT GetGenericParamConstraintProps(
		//     [in]  mdGenericParamConstraint  gpc,
		//     [out] mdGenericParam            *ptGenericParam,
		//     [out] mdToken                   *ptkConstraintType
		// );
		void GetGenericParamConstraintProps_();

		// HRESULT GetPEKind(
		//     [out] DWORD *pdwPEKind,
		//     [out] DWORD *pdwMachine
		// );
		void GetPEKind_();

		// HRESULT GetVersionString(
		//     [out] LPWSTR      pwzBuf,
		//     [in]  DWORD       ccBufSize,
		//     [out] DWORD       *pccBufSize
		// );
		void GetVersionString_();

		// HRESULT EnumMethodSpecs(
		//     [in, out] HCORENUM      *phEnum,
		//     [in]      mdToken       tk,
		//     [out]     mdMethodSpec  rMethodSpecs[],
		//     [in]      ULONG         cMax,
		//     [out]     ULONG         *pcMethodSpecs
		// );
		void EnumMethodSpecs_();
	}
}
