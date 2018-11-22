// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using CausalityDbg.Core.CorDebugApi;
using CausalityDbg.Core.MetaDataApi;
using CausalityDbg.IL;

namespace CausalityDbg.Core
{
	static class ModuleExtensions
	{
		public static string GetName(this ICorDebugModule module)
		{
			module.GetName(0, out var len, null);
			var buffer = new char[len];
			module.GetName((uint)buffer.Length, out len, buffer);
			return len > 0 ? new string(buffer, 0, (int)(len - 1)) : string.Empty;
		}

		public static IMetaDataImport GetMetaDataImport(this ICorDebugModule module)
		{
			return (IMetaDataImport)module.GetMetaDataInterface(typeof(IMetaDataImport).GUID);
		}

		public static IMetaDataAssemblyImport GetMetaDataAssemblyImport(this ICorDebugModule module)
		{
			return (IMetaDataAssemblyImport)module.GetMetaDataInterface(typeof(IMetaDataAssemblyImport).GUID);
		}

		public static ICorDebugClass FindClass(this ICorDebugModule module, string className)
		{
			return FindClass(module, MetaDataToken.Nil, className);
		}

		public static ICorDebugClass FindClass(this ICorDebugModule module, MetaDataToken scopeToken, string className)
		{
			var import = module.GetMetaDataImport();
			var cToken = MetaDataToken.Nil;
			var hr = import.FindTypeDefByName(className, scopeToken, out cToken);

			if (hr == (int)HResults.CLDB_E_RECORD_NOTFOUND)
			{
				return null;
			}
			else if (hr < 0)
			{
				throw Marshal.GetExceptionForHR(hr);
			}

			return module.GetClassFromToken(cToken);
		}

		public static ICorDebugClass ResolveClassRef(this ICorDebugModule module, MetaDataToken typeRefToken)
		{
			if (TryResolve(ref module, ref typeRefToken))
			{
				return module.GetClassFromToken(typeRefToken);
			}
			else
			{
				return null;
			}
		}

		static bool TryResolve(ref ICorDebugModule module, ref MetaDataToken token)
		{
			GetTargetScopeAndName(module, token, out var scope, out var className);

			if (scope.TokenType == TokenType.TypeRef)
			{
				if (!TryResolve(ref module, ref scope))
				{
					return false;
				}

				var import = module.GetMetaDataImport();
				var hr = import.FindTypeDefByName(className, scope, out token);

				if (hr == (int)HResults.CLDB_E_RECORD_NOTFOUND)
				{
					module = null;
					return false;
				}
				else if (hr < 0)
				{
					throw Marshal.GetExceptionForHR(hr);
				}
				else if (token.IsNil)
				{
					module = null;
					return false;
				}

				return true;
			}
			else if (scope == new MetaDataToken(0u))
			{
				return TryResolveExportedType(ref module, className, out token);
			}
			else if (scope == new MetaDataToken(1u))
			{
				return TryResolveTypeByName(module, className, out token);
			}
			else
			{
				var assembly = ((ICorDebugModule2)module).ResolveAssembly(scope);
				return TryResolveTypeByName(assembly, className, out module, out token);
			}
		}

		static bool TryResolveTypeByName(ICorDebugAssembly assembly, string className, out ICorDebugModule module, out MetaDataToken token)
		{
			var moduleEnum = assembly.EnumerateModules();

			ICorDebugModule fallbackModule = null;

			while (moduleEnum.Next(1, out var otherModule))
			{
				if (fallbackModule == null)
				{
					fallbackModule = otherModule;
				}

				if (TryResolveTypeByName(otherModule, className, out token))
				{
					module = otherModule;
					return true;
				}
			}

			if (fallbackModule == null)
			{
				throw new InvalidOperationException("no module?");
			}
			else if (TryResolveExportedType(ref fallbackModule, className, out token))
			{
				module = fallbackModule;
				return true;
			}

			module = null;
			token = MetaDataToken.Nil;
			return false;
		}

		static bool TryResolveTypeByName(ICorDebugModule module, string className, out MetaDataToken token)
		{
			var importRef = module.GetMetaDataImport();
			var hr = importRef.FindTypeDefByName(className, MetaDataToken.Nil, out token);

			if (hr == (int)HResults.CLDB_E_RECORD_NOTFOUND)
			{
				return false;
			}
			else if (hr < 0)
			{
				throw Marshal.GetExceptionForHR(hr);
			}
			else
			{
				return !token.IsNil;
			}
		}

		static bool TryResolveExportedType(ref ICorDebugModule module, string className, out MetaDataToken token)
		{
			var aImport = module.GetMetaDataAssemblyImport();

			var hr = aImport.FindExportedTypeByName(className, MetaDataToken.Nil, out var mdExportedType);

			if (hr == (int)HResults.CLDB_E_RECORD_NOTFOUND)
			{
				module = null;
				token = MetaDataToken.Nil;
				return false;
			}
			else if (hr < 0)
			{
				throw Marshal.GetExceptionForHR(hr);
			}

			aImport.GetExportedTypeProps(
				mdExportedType,
				null,
				0,
				out var size,
				out var implementation,
				out var hint,
				out var att);

			if (hr < 0)
			{
				throw Marshal.GetExceptionForHR(hr);
			}

			if (implementation.TokenType == TokenType.AssemblyRef)
			{
				var assembly = ((ICorDebugModule2)module).ResolveAssembly(implementation);
				return TryResolveTypeByName(assembly, className, out module, out token);
			}

			module = null;
			token = MetaDataToken.Nil;
			return false;
		}

		static void GetTargetScopeAndName(ICorDebugModule module, MetaDataToken typeRefToken, out MetaDataToken scope, out string className)
		{
			var import = module.GetMetaDataImport();

			import.GetTypeRefProps(
				typeRefToken,
				out scope,
				null,
				0,
				out var size);

			var buffer = new char[size];

			import.GetTypeRefProps(
				typeRefToken,
				out scope,
				buffer,
				buffer.Length,
				out size);

			className = new string(buffer, 0, size - 1);
		}
	}
}
