// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using CausalityDbg.Core.CorDebugApi;
using CausalityDbg.Core.MetaDataApi;
using CausalityDbg.IL;

namespace CausalityDbg.Core
{
	static class FunctionExtensions
	{
		public static List<byte[]> GetDataSections(this ICorDebugFunction function)
		{
			var result = new List<byte[]>();
			var addr = function.GetHeaderAddress();
			var process = function.GetModule().GetProcess();

			var header = process.ReadLong(addr);

			if ((header & 0x03) == (int)CorILMethodFlags.CorILMethod_TinyFormat &&
				(header & (int)CorILMethodFlags.CorILMethod_MoreSects) == 0)
			{
				return result;
			}

			var headerLen = (int)((header >> 10) & 0x3C);
			var codeLen = (int)((header >> 32) & 0xFFFFFFFF);

			addr = (addr + headerLen + codeLen).AlignToWord();

			do
			{
				var sectHeader = process.ReadInt(addr);
				int sectLen;

				if ((sectHeader & (int)CorILMethodSectFlags.CorILMethod_Sect_FatFormat) == 0)
				{
					sectLen = (sectHeader >> 8) & 0xFF;
				}
				else
				{
					sectLen = (sectHeader >> 8) & 0xFFFFFF;
				}

				result.Add(process.ReadBytes(addr, sectLen));

				if ((sectHeader & (int)CorILMethodSectFlags.CorILMethod_Sect_MoreSects) == 0)
				{
					break;
				}

				addr = (addr + sectLen).AlignToWord();
			}
			while (true);

			return result;
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

		public static CorExceptionClauseCollection GetExceptionClauses(this ICorDebugFunction function)
		{
			foreach (var blob in function.GetDataSections())
			{
				var collection = CorExceptionClauseCollection.New(blob);

				if (collection != null)
				{
					return collection;
				}
			}

			return null;
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
						buffer = new char[paramName.Length + 1];
					}

					import.GetParamProps(
						paramToken,
						IntPtr.Zero,
						IntPtr.Zero,
						buffer,
						(uint)paramName.Length,
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

							return index;
						}
					}

					index++;
				}
			}
			finally
			{
				import.CloseEnum(hEnum);
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
