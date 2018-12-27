// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
				out var size,
				out var att,
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
			var length = name.Length + 1;
			var buffer = ArrayPool<char>.Shared.Rent(length);

			try
			{
				while (import.EnumMethods(ref hEnum, cl.GetToken(), out var mToken, 1))
				{
					import.GetMethodProps(
						mToken,
						out var classToken,
						buffer,
						length,
						out var size,
						IntPtr.Zero,
						out var sigBlob,
						out var sigSize,
						out var rva,
						IntPtr.Zero);

					if (size != length) continue;

					var funcName = new string(buffer, 0, size - 1);

					if (funcName != name) continue;

					if (method.SpecifiesArgTypes)
					{
						var blob = ArrayPool<byte>.Shared.Rent(sigSize);
						Marshal.Copy(sigBlob, blob, 0, sigSize);
						var sig = SignatureReader.ReadMethodDefSig(new ArraySegment<byte>(blob, 0, sigSize));
						ArrayPool<byte>.Shared.Return(blob);
						if (!IsMatch(module, sig, method)) continue;
					}

					yield return module.GetFunctionFromToken(mToken);
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
			var length = name.Length + 1;
			var buffer = ArrayPool<char>.Shared.Rent(length);

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

					if (size != length) continue;

					var propertyName = new string(buffer, 0, size - 1);

					if (propertyName != name) continue;

					ArrayPool<char>.Shared.Return(buffer);
					return module.GetFunctionFromToken(mToken);
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
				case CorElementType.ELEMENT_TYPE_BOOLEAN: return typeName == "System.Boolean";
				case CorElementType.ELEMENT_TYPE_CHAR: return typeName == "System.Char";
				case CorElementType.ELEMENT_TYPE_I: return typeName == "System.IntPtr";
				case CorElementType.ELEMENT_TYPE_I1: return typeName == "System.SByte";
				case CorElementType.ELEMENT_TYPE_I2: return typeName == "System.Int16";
				case CorElementType.ELEMENT_TYPE_I4: return typeName == "System.Int32";
				case CorElementType.ELEMENT_TYPE_I8: return typeName == "System.Int64";
				case CorElementType.ELEMENT_TYPE_OBJECT: return typeName == "System.Object";
				case CorElementType.ELEMENT_TYPE_R4: return typeName == "System.Single";
				case CorElementType.ELEMENT_TYPE_R8: return typeName == "System.Double";
				case CorElementType.ELEMENT_TYPE_STRING: return typeName == "System.String";
				case CorElementType.ELEMENT_TYPE_U: return typeName == "System.UIntPtr";
				case CorElementType.ELEMENT_TYPE_U1: return typeName == "System.Byte";
				case CorElementType.ELEMENT_TYPE_U2: return typeName == "System.UInt16";
				case CorElementType.ELEMENT_TYPE_U4: return typeName == "System.UInt32";
				case CorElementType.ELEMENT_TYPE_U8: return typeName == "System.UInt64";

				case CorElementType.ELEMENT_TYPE_CLASS:
				case CorElementType.ELEMENT_TYPE_VALUETYPE:
					var token = ((SigTypeUserType)sigType).Token;

					if (token.TokenType == TokenType.TypeDef)
					{
						return IsMatch_TypeDef(module, token, typeName, typeName.Length);
					}
					else if (token.TokenType == TokenType.TypeRef)
					{
						return IsMatch_TypeRef(module, token, typeName, typeName.Length);
					}
					else
					{
						return false;
					}

				default:
					return false;
			}
		}

		static bool IsMatch_TypeRef(ICorDebugModule module, MetaDataToken token, string typeName, int len)
		{
			var length = len + 1;
			var buffer = ArrayPool<char>.Shared.Rent(length);

			var import = module.GetMetaDataImport();
			import.GetTypeRefProps(
				token,
				out var scope,
				buffer,
				length,
				out var size);

			size--;

			bool result;

			if (scope.TokenType == TokenType.TypeRef)
			{
				var start = len - size - 2;

				result = start > 0
					&& IsMatch(typeName, buffer, start + 2, size)
					&& typeName[start] == ':'
					&& typeName[start + 1] == ':'
					&& IsMatch_TypeRef(module, scope, typeName, start);
			}
			else
			{
				result = size == len
					&& IsMatch(typeName, buffer, 0, len);
			}

			ArrayPool<char>.Shared.Return(buffer);

			return result;
		}

		static bool IsMatch_TypeDef(ICorDebugModule module, MetaDataToken token, string typeName, int len)
		{
			var length = len + 1;
			var buffer = ArrayPool<char>.Shared.Rent(length);

			var import = module.GetMetaDataImport();
			import.GetTypeDefProps(
				token,
				buffer,
				length,
				out var size,
				out var att,
				out var parent);

			size--;

			bool result;

			if (att.IsNested())
			{
				var start = len - size - 2;

				import.GetNestedClassProps(token, out var declaringToken);

				result = start > 0
					&& IsMatch(typeName, buffer, start + 2, size)
					&& typeName[start] == ':'
					&& typeName[start + 1] == ':'
					&& IsMatch_TypeDef(module, declaringToken, typeName, start);
			}
			else
			{
				result = size == len
					&& IsMatch(typeName, buffer, 0, size);
			}

			ArrayPool<char>.Shared.Return(buffer);

			return result;
		}

		static bool IsMatch(string typeName, char[] segment, int start, int length)
		{
			for (var i = 0; i < length; i++)
			{
				if (typeName[start + i] != segment[i])
				{
					return false;
				}
			}

			return true;
		}
	}
}
