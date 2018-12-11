// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using CausalityDbg.Core.MetaDataApi;
using CausalityDbg.IL;

namespace CausalityDbg.Core.CorDebugApi
{
	static class FrameExtensions
	{
		public static void GetGenArgs(this ICorDebugILFrame frame, out ICorDebugType[] typeGenArgs, out ICorDebugType[] methodGenArgs)
		{
			var function = frame.GetFunction();
			var mToken = function.GetToken();
			var declClass = function.GetClass();
			var tToken = declClass.GetToken();
			var module = function.GetModule();
			var import = (IMetaDataImport2)module.GetMetaDataImport();

			var genArgs = ((ICorDebugILFrame2)frame).EnumerateTypeParameters();

			typeGenArgs = import.GetCorTypes(genArgs, tToken);
			methodGenArgs = import.GetCorTypes(genArgs, mToken);
		}

		public static int? GetIP(this ICorDebugILFrame frame)
		{
			const CorDebugMappingResult mask =
				CorDebugMappingResult.MAPPING_APPROXIMATE |
				CorDebugMappingResult.MAPPING_EXACT;

			if ((frame.GetIP(out var ip) & mask) != 0)
			{
				return (int)ip;
			}
			else
			{
				return null;
			}
		}

		static ICorDebugType[] GetCorTypes(this IMetaDataImport2 import, ICorDebugTypeEnum genArgs, MetaDataToken token)
		{
			var count = import.CountGenericParams(token);

			if (count == 0)
			{
				return null;
			}
			else
			{
				var typeGenArgs = new ICorDebugType[count];

				for (var i = 0; i < typeGenArgs.Length; i++)
				{
					genArgs.Next(1, out var type);
					typeGenArgs[i] = type;
				}

				return typeGenArgs;
			}
		}
	}
}
