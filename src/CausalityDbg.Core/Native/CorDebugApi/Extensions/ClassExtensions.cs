// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Buffers;
using System.Collections.Generic;
using CausalityDbg.Configuration;
using CausalityDbg.IL;

namespace CausalityDbg.Core.CorDebugApi
{
	static class ClassExtensions
	{
		public static ICorDebugClass GetBase(this ICorDebugClass cl)
		{
			var module = cl.GetModule();
			var import = module.GetMetaDataImport();

			import.GetTypeDefProps(
				cl.GetToken(),
				null,
				0,
				out var _,
				out var _,
				out var bToken);

			if (bToken.IsNil)
			{
				return null;
			}
			else if (bToken.TokenType == TokenType.TypeDef)
			{
				return module.GetClassFromToken(bToken);
			}
			else if (bToken.TokenType == TokenType.TypeRef)
			{
				return module.ResolveClassRef(bToken);
			}
			else
			{
				throw new TypeResolutionException();
			}
		}

		public static ICorDebugClass FindClass(this ICorDebugClass cl, string className)
		{
			return cl.GetModule().FindClass(cl.GetToken(), className);
		}

		public static IEnumerable<ICorDebugFunction> FindFunctions(this ICorDebugClass cl, MethodRef method)
		{
			var module = cl.GetModule();
			var import = module.GetMetaDataImport();
			var hEnum = IntPtr.Zero;
			var name = method.Name;
			var buffer = ArrayPool<char>.Shared.Rent(name.Length + 1);

			try
			{
				while (import.EnumMethods(ref hEnum, cl.GetToken(), out var mToken, 1))
				{
					import.GetMethodProps(
						mToken,
						out var classToken,
						buffer,
						buffer.Length,
						out var size,
						IntPtr.Zero,
						out var sigBlob,
						out var sigSize,
						out var rva,
						IntPtr.Zero);

					if (size == name.Length + 1 &&
						name.AsSpan().Equals(buffer.AsSpan(0, size - 1), StringComparison.Ordinal))
					{
						if (method.SpecifiesArgTypes)
						{
							var sig = SignatureReader.ReadMethodDefSig(SpanUtils.Create<byte>(sigBlob, sigSize));
							if (!IsMatch(module, sig, method)) continue;
						}

						yield return module.GetFunctionFromToken(mToken);
					}
				}
			}
			finally
			{
				if (hEnum != IntPtr.Zero)
				{
					import.CloseEnum(hEnum);
				}
			}

			ArrayPool<char>.Shared.Return(buffer);
		}

		public static ICorDebugFunction FindPropertyGetterInherit(this ICorDebugClass cl, string name)
		{
			do
			{
				var func = cl.FindPropertyGetter(name);

				if (func != null)
				{
					return func;
				}

				cl = cl.GetBase();
			}
			while (cl != null);

			return null;
		}

		public static ICorDebugFunction FindPropertyGetter(this ICorDebugClass cl, string name)
		{
			var module = cl.GetModule();
			var import = module.GetMetaDataImport();
			var hEnum = IntPtr.Zero;
			var cToken = cl.GetToken();
			var buffer = ArrayPool<char>.Shared.Rent(name.Length + 1);

			try
			{
				while (import.EnumProperties(ref hEnum, cToken, out var pToken, 1))
				{
					import.GetPropertyProps(
						pToken,
						IntPtr.Zero,
						buffer,
						buffer.Length,
						out var size,
						IntPtr.Zero,
						IntPtr.Zero,
						IntPtr.Zero,
						IntPtr.Zero,
						IntPtr.Zero,
						IntPtr.Zero,
						IntPtr.Zero,
						out var mToken,
						IntPtr.Zero,
						0,
						IntPtr.Zero);

					if (size == name.Length + 1 &&
						name.AsSpan().Equals(buffer.AsSpan(0, size - 1), StringComparison.Ordinal))
					{
						ArrayPool<char>.Shared.Return(buffer);
						return module.GetFunctionFromToken(mToken);
					}
				}
			}
			finally
			{
				if (hEnum != IntPtr.Zero)
				{
					import.CloseEnum(hEnum);
				}
			}

			ArrayPool<char>.Shared.Return(buffer);
			return null;
		}

		static bool IsMatch(ICorDebugModule module, SigMethod sig, MethodRef method)
		{
			if (method.ArgTypes.Length != sig.Parameters.Length) return false;

			for (var i = 0; i < method.ArgTypes.Length; i++)
			{
				var sigArg = sig.Parameters[i];
				var argType = method.ArgTypes[i];

				if (sigArg.ByRef != argType.ByRef) return false;
				if (!IsMatch(module, sigArg.ValueType, argType.Name)) return false;
			}

			return true;
		}

		static bool IsMatch(ICorDebugModule module, SigType sigType, string typeName)
		{
			switch (sigType.ElementType)
			{
				case CorElementType.ELEMENT_TYPE_BOOLEAN: return typeName.Equals("System.Boolean", StringComparison.Ordinal);
				case CorElementType.ELEMENT_TYPE_CHAR: return typeName.Equals("System.Char", StringComparison.Ordinal);
				case CorElementType.ELEMENT_TYPE_I: return typeName.Equals("System.IntPtr", StringComparison.Ordinal);
				case CorElementType.ELEMENT_TYPE_I1: return typeName.Equals("System.SByte", StringComparison.Ordinal);
				case CorElementType.ELEMENT_TYPE_I2: return typeName.Equals("System.Int16", StringComparison.Ordinal);
				case CorElementType.ELEMENT_TYPE_I4: return typeName.Equals("System.Int32", StringComparison.Ordinal);
				case CorElementType.ELEMENT_TYPE_I8: return typeName.Equals("System.Int64", StringComparison.Ordinal);
				case CorElementType.ELEMENT_TYPE_OBJECT: return typeName.Equals("System.Object", StringComparison.Ordinal);
				case CorElementType.ELEMENT_TYPE_R4: return typeName.Equals("System.Single", StringComparison.Ordinal);
				case CorElementType.ELEMENT_TYPE_R8: return typeName.Equals("System.Double", StringComparison.Ordinal);
				case CorElementType.ELEMENT_TYPE_STRING: return typeName.Equals("System.String", StringComparison.Ordinal);
				case CorElementType.ELEMENT_TYPE_U: return typeName.Equals("System.UIntPtr", StringComparison.Ordinal);
				case CorElementType.ELEMENT_TYPE_U1: return typeName.Equals("System.Byte", StringComparison.Ordinal);
				case CorElementType.ELEMENT_TYPE_U2: return typeName.Equals("System.UInt16", StringComparison.Ordinal);
				case CorElementType.ELEMENT_TYPE_U4: return typeName.Equals("System.UInt32", StringComparison.Ordinal);
				case CorElementType.ELEMENT_TYPE_U8: return typeName.Equals("System.UInt64", StringComparison.Ordinal);

				case CorElementType.ELEMENT_TYPE_CLASS:
				case CorElementType.ELEMENT_TYPE_VALUETYPE:
					var token = ((SigTypeUserType)sigType).Token;

					if (token.TokenType == TokenType.TypeDef)
					{
						return IsMatch_TypeDef(module, token, typeName.AsSpan());
					}
					else if (token.TokenType == TokenType.TypeRef)
					{
						return IsMatch_TypeRef(module, token, typeName.AsSpan());
					}
					else
					{
						return false;
					}

				default:
					return false;
			}
		}

		static bool IsMatch_TypeRef(ICorDebugModule module, MetaDataToken token, ReadOnlySpan<char> typeName)
		{
			var buffer = ArrayPool<char>.Shared.Rent(typeName.Length + 1);

			var import = module.GetMetaDataImport();
			import.GetTypeRefProps(
				token,
				out var scope,
				buffer,
				buffer.Length,
				out var size);

			size--;

			bool result;

			if (scope.TokenType == TokenType.TypeRef)
			{
				var start = typeName.Length - size - 2;

				result = start > 0
					&& typeName.Slice(start + 2).Equals(buffer.AsSpan(0, size), StringComparison.Ordinal)
					&& typeName[start] == ':'
					&& typeName[start + 1] == ':'
					&& IsMatch_TypeRef(module, scope, typeName.Slice(0, start));
			}
			else
			{
				result = typeName.Equals(buffer.AsSpan(0, size), StringComparison.Ordinal);
			}

			ArrayPool<char>.Shared.Return(buffer);

			return result;
		}

		static bool IsMatch_TypeDef(ICorDebugModule module, MetaDataToken token, ReadOnlySpan<char> typeName)
		{
			var buffer = ArrayPool<char>.Shared.Rent(typeName.Length + 1);

			var import = module.GetMetaDataImport();
			import.GetTypeDefProps(
				token,
				buffer,
				buffer.Length,
				out var size,
				out var att,
				out var _);

			size--;

			bool result;

			if (att.IsNested())
			{
				var start = typeName.Length - size - 2;

				import.GetNestedClassProps(token, out var declaringToken);

				result = start > 0
					&& typeName.Slice(start + 2, size).Equals(buffer.AsSpan(0, size), StringComparison.Ordinal)
					&& typeName[start] == ':'
					&& typeName[start + 1] == ':'
					&& IsMatch_TypeDef(module, declaringToken, typeName.Slice(0, start));
			}
			else
			{
				result = typeName.Equals(buffer.AsSpan(0, size), StringComparison.Ordinal);
			}

			ArrayPool<char>.Shared.Return(buffer);

			return result;
		}
	}
}
