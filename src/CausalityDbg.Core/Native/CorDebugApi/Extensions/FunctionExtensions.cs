// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Buffers;
using System.Reflection;
using System.Runtime.InteropServices;
using CausalityDbg.Core.MetaDataApi;
using CausalityDbg.IL;

namespace CausalityDbg.Core.CorDebugApi
{
	static class FunctionExtensions
	{
		public static CorExceptionClauseCollection GetExceptionClauses(this ICorDebugFunction function)
		{
			var addr = function.GetHeaderAddress();
			var process = function.GetModule().GetProcess();

			var header = process.ReadValue<long>(addr);

			if ((header & 0x03) == (int)CorILMethodFlags.CorILMethod_TinyFormat &&
				(header & (int)CorILMethodFlags.CorILMethod_MoreSects) == 0)
			{
				return null;
			}

			var headerLen = (int)((header >> 10) & 0x3C);
			var codeLen = (int)((header >> 32) & 0xFFFFFFFF);

			addr = (addr + headerLen + codeLen).AlignToWord();

			do
			{
				var sectHeader = process.ReadValue<int>(addr);
				int sectLen;

				if ((sectHeader & (int)CorILMethodSectFlags.CorILMethod_Sect_FatFormat) == 0)
				{
					sectLen = (sectHeader >> 8) & 0xFF;
				}
				else
				{
					sectLen = (sectHeader >> 8) & 0xFFFFFF;
				}

				var blob = ArrayPool<byte>.Shared.Rent(sectLen);
				process.ReadBytes(addr, blob, 0, sectLen);
				var collection = CorExceptionClauseCollection.New(blob);
				ArrayPool<byte>.Shared.Return(blob);

				if (collection != null)
				{
					return collection;
				}

				if ((sectHeader & (int)CorILMethodSectFlags.CorILMethod_Sect_MoreSects) == 0)
				{
					break;
				}

				addr = (addr + sectLen).AlignToWord();
			}
			while (true);

			return null;
		}

		public static CORDB_ADDRESS GetHeaderAddress(this ICorDebugFunction function)
		{
			var module = function.GetModule();
			var import = module.GetMetaDataImport();

			import.GetMethodProps(
				function.GetToken(),
				out var classTok,
				null,
				0,
				out var nameSize,
				IntPtr.Zero,
				out var blobPtr,
				out var blobSize,
				out var rva,
				IntPtr.Zero);

			return module.GetBaseAddress() + rva;
		}

		public static int GetParamIndex(this ICorDebugFunction function, string paramName)
		{
			if (string.IsNullOrEmpty(paramName)) return -1;

			var module = function.GetModule();
			var import = module.GetMetaDataImport();
			var functionToken = function.GetToken();
			var hEnum = IntPtr.Zero;
			char[] buffer = null;
			var index = 0;

			try
			{
				while (import.EnumParams(ref hEnum, functionToken, out var paramToken, 1))
				{
					if (buffer == null)
					{
						buffer = ArrayPool<char>.Shared.Rent(paramName.Length + 1);
					}

					import.GetParamProps(
						paramToken,
						IntPtr.Zero,
						IntPtr.Zero,
						buffer,
						paramName.Length,
						out var size,
						IntPtr.Zero,
						IntPtr.Zero,
						IntPtr.Zero,
						IntPtr.Zero);

					if (size == paramName.Length + 1)
					{
						if (new string(buffer, 0, paramName.Length) == paramName)
						{
							if ((GetCallingConventionLight(import, functionToken) & CallingConventionThisMask) == CallingConventions.HasThis)
							{
								index++;
							}

							ArrayPool<char>.Shared.Return(buffer);
							return index;
						}
					}

					index++;
				}
			}
			finally
			{
				if (hEnum != IntPtr.Zero)
				{
					import.CloseEnum(hEnum);
				}
			}

			if (buffer != null)
			{
				ArrayPool<char>.Shared.Return(buffer);
			}

			return -1;
		}

		public static CallingConventions GetCallingConventionLight(this ICorDebugFunction function)
		{
			var module = function.GetModule();
			var import = module.GetMetaDataImport();
			return GetCallingConventionLight(import, function.GetToken());
		}

		static CallingConventions GetCallingConventionLight(IMetaDataImport import, MetaDataToken token)
		{
			import.GetMethodProps(
				token,
				out var classToken,
				null,
				0,
				out var nameSize,
				IntPtr.Zero,
				out var blobPtr,
				out var blobSize,
				out var rva,
				IntPtr.Zero);

			var preamble = Marshal.ReadByte(blobPtr);

			const byte VarArgs = 0x05;

			var result = ((CallingConventions)preamble) & CallingConventionThisMask;

			if ((preamble & ~unchecked((byte)CallingConventionThisMask)) == VarArgs)
			{
				result |= CallingConventions.VarArgs;
			}
			else
			{
				result |= CallingConventions.Standard;
			}

			return result;
		}

		const CallingConventions CallingConventionThisMask = CallingConventions.HasThis | CallingConventions.ExplicitThis;
	}
}
