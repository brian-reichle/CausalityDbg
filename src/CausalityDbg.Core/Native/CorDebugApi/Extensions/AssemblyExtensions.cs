// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Buffers;
using System.Runtime.InteropServices;
using CausalityDbg.Core.MetaDataApi;
using CausalityDbg.IL;

namespace CausalityDbg.Core.CorDebugApi
{
	static class AssemblyExtensions
	{
		public static string GetName(this ICorDebugAssembly assembly)
		{
			assembly.GetName(0, out var len, null);
			var buffer = ArrayPool<char>.Shared.Rent(len);
			assembly.GetName(len, out len, buffer);

			var name = len > 0 ? new string(buffer, 0, len - 1) : string.Empty;
			ArrayPool<char>.Shared.Return(buffer);

			return name;
		}

		public static ICorDebugClass FindClass(this ICorDebugAssembly assembly, string className)
		{
			var moduleEnum = assembly.EnumerateModules();

			while (moduleEnum.Next(1, out var module))
			{
				var cl = module.FindClass(className);

				if (cl != null)
				{
					return cl;
				}
			}

			return null;
		}

		public static IMetaDataAssemblyImport GetMetaDataAssemblyImport(this ICorDebugAssembly assembly, out MetaDataToken assemblyToken)
		{
			var modules = assembly.EnumerateModules();

			while (modules.Next(1, out var module))
			{
				var aImport = module.GetMetaDataAssemblyImport();
				var scopeToken = aImport.GetAssemblyFromScope();
				Marshal.ReleaseComObject(module);

				if (!scopeToken.IsNil)
				{
					assemblyToken = scopeToken;
					Marshal.ReleaseComObject(modules);
					return aImport;
				}

				Marshal.ReleaseComObject(aImport);
			}

			Marshal.ReleaseComObject(modules);
			assemblyToken = MetaDataToken.Nil;
			return null;
		}
	}
}
