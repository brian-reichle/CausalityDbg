// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

namespace CausalityDbg.Core
{
	static partial class NativeMethods
	{
		// BOOL CryptAcquireContext(
		//     [out]  HCRYPTPROV *phProv,
		//     [in]   LPCTSTR     pszContainer,
		//     [in]   LPCTSTR     pszProvider,
		//     [in]   DWORD       dwProvType,
		//     [in]   DWORD       dwFlags
		// );
		[DllImport("advapi32.dll", PreserveSig = true, BestFitMapping = false, ThrowOnUnmappableChar = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CryptAcquireContext(
			out CryptContextHandle phProv,
			[MarshalAs(UnmanagedType.LPTStr)] string pszContainer,
			[MarshalAs(UnmanagedType.LPTStr)] string pszProvider,
			uint dwProvType,
			uint dwFlags);

		// BOOL CryptCreateHash(
		//     [in]   HCRYPTPROV  hProv,
		//     [in]   ALG_ID      Algid,
		//     [in]   HCRYPTKEY   hKey,
		//     [in]   DWORD       dwFlags,
		//     [out]  HCRYPTHASH *phHash
		// );
		[DllImport("advapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CryptCreateHash(
			CryptContextHandle hProv,
			uint algid,
			IntPtr hKey,
			uint dwFlags,
			out CryptHashHandle phHash);

		// BOOL CryptDestroyHash(
		//     [in]  HCRYPTHASH hHash
		// );
		[DllImport("advapi32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CryptDestroyHash(
			IntPtr hHash);

		// BOOL CryptReleaseContext(
		//     [in]  HCRYPTPROV hProv,
		//     [in]  DWORD      dwFlags
		// );
		[DllImport("advapi32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CryptReleaseContext(
			IntPtr hProv,
			uint dwFlags);

		// BOOL CryptHashData(
		//     [in]  HCRYPTHASH  hHash,
		//     [in]  BYTE       *pbData,
		//     [in]  DWORD       dwDataLen,
		//     [in]  DWORD       dwFlags
		// );
		[DllImport("advapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CryptHashData(
			CryptHashHandle hHash,
			IntPtr pbData,
			int dwDataLen,
			uint dwFlags);

		// BOOL CryptGetHashParam(
		//     [in]      HCRYPTHASH  hHash,
		//     [in]      DWORD       dwParam,
		//     [out]     BYTE       *pbData,
		//     [in, out] DWORD      *pdwDataLen,
		//     [in]      DWORD       dwFlags
		// );
		[DllImport("advapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CryptGetHashParam(
			CryptHashHandle hHash,
			uint dwParam,
			IntPtr pbData,
			ref int pdwDataLen,
			uint dwFlags);
	}
}
