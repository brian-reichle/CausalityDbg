// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace CausalityDbg.Core
{
	static class CryptFunctions
	{
		const uint PROV_RSA_FULL = 1;
		const uint CALG_SHA1 = 0x00008004;
		const uint HP_HASHVAL = 0x02;
		const uint CRYPT_VERIFYCONTEXT = 0xF0000000;

		public static long GetPublicKeyToken(IntPtr publicKey, int publicKeySize)
		{
			long result;

			using (var context = AquireContext())
			using (var hash = CreateHash(context))
			{
				CryptHashData(hash, publicKey, publicKeySize);

				using var buffer = GetHash(hash);
				result = buffer.Read<long>(buffer.ByteLength - sizeof(long));
			}

			return result;
		}

		static CryptContextHandle AquireContext()
		{
			if (!NativeMethods.CryptAcquireContext(
				out var hCryptProv,
				null,
				null,
				PROV_RSA_FULL,
				CRYPT_VERIFYCONTEXT))
			{
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}

			return hCryptProv;
		}

		static CryptHashHandle CreateHash(CryptContextHandle hCryptProv)
		{
			if (!NativeMethods.CryptCreateHash(
				hCryptProv,
				CALG_SHA1,
				IntPtr.Zero,
				0,
				out var hHash))
			{
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}

			return hHash;
		}

		static void CryptHashData(CryptHashHandle hHash, IntPtr publicKey, int publicKeySize)
		{
			if (!NativeMethods.CryptHashData(hHash, publicKey, publicKeySize, 0))
			{
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}
		}

		static UnmanagedBuffer GetHash(CryptHashHandle hHash)
		{
			var hashSize = 0;

			if (!NativeMethods.CryptGetHashParam(
				hHash,
				HP_HASHVAL,
				IntPtr.Zero,
				ref hashSize,
				0))
			{
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}

			var buffer = new UnmanagedBuffer(hashSize);

			try
			{
				if (!NativeMethods.CryptGetHashParam(
					hHash,
					HP_HASHVAL,
					buffer.DangerousGetHandle(),
					ref hashSize,
					0))
				{
					throw new Win32Exception(Marshal.GetLastWin32Error());
				}
			}
			catch
			{
				buffer.Close();
				throw;
			}

			return buffer;
		}
	}
}
