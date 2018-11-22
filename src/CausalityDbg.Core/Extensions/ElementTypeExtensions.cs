// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using CausalityDbg.IL;

namespace CausalityDbg.Core
{
	static class ElementTypeExtensions
	{
		public static string GetShortName(this CorElementType type)
		{
			switch (type)
			{
				case CorElementType.ELEMENT_TYPE_BOOLEAN: return "bool";
				case CorElementType.ELEMENT_TYPE_CHAR: return "char";
				case CorElementType.ELEMENT_TYPE_I1: return "sbyte";
				case CorElementType.ELEMENT_TYPE_U1: return "byte";
				case CorElementType.ELEMENT_TYPE_I2: return "short";
				case CorElementType.ELEMENT_TYPE_U2: return "ushort";
				case CorElementType.ELEMENT_TYPE_I4: return "int";
				case CorElementType.ELEMENT_TYPE_U4: return "uint";
				case CorElementType.ELEMENT_TYPE_I8: return "long";
				case CorElementType.ELEMENT_TYPE_U8: return "ulong";
				case CorElementType.ELEMENT_TYPE_R4: return "float";
				case CorElementType.ELEMENT_TYPE_R8: return "double";
				case CorElementType.ELEMENT_TYPE_I: return "System.IntPtr";
				case CorElementType.ELEMENT_TYPE_U: return "System.UIntPtr";
				case CorElementType.ELEMENT_TYPE_OBJECT: return "object";
				case CorElementType.ELEMENT_TYPE_STRING: return "string";
				case CorElementType.ELEMENT_TYPE_VOID: return "void";
				case CorElementType.ELEMENT_TYPE_TYPEDBYREF: return "System.TypedReference";
				default: throw new TypeResolutionException();
			}
		}

		public static string GetFullName(this CorElementType type)
		{
			switch (type)
			{
				case CorElementType.ELEMENT_TYPE_BOOLEAN: return "System.Boolean";
				case CorElementType.ELEMENT_TYPE_CHAR: return "System.Char";
				case CorElementType.ELEMENT_TYPE_I1: return "System.SByte";
				case CorElementType.ELEMENT_TYPE_U1: return "System.Byte";
				case CorElementType.ELEMENT_TYPE_I2: return "System.Int16";
				case CorElementType.ELEMENT_TYPE_U2: return "System.UInt16";
				case CorElementType.ELEMENT_TYPE_I4: return "System.Int32";
				case CorElementType.ELEMENT_TYPE_U4: return "System.UInt32";
				case CorElementType.ELEMENT_TYPE_I8: return "System.Int64";
				case CorElementType.ELEMENT_TYPE_U8: return "System.UInt64";
				case CorElementType.ELEMENT_TYPE_R4: return "System.Single";
				case CorElementType.ELEMENT_TYPE_R8: return "System.Double";
				case CorElementType.ELEMENT_TYPE_I: return "System.IntPtr";
				case CorElementType.ELEMENT_TYPE_U: return "System.UIntPtr";
				case CorElementType.ELEMENT_TYPE_OBJECT: return "System.Object";
				case CorElementType.ELEMENT_TYPE_STRING: return "System.String";
				case CorElementType.ELEMENT_TYPE_VOID: return "System.Void";
				case CorElementType.ELEMENT_TYPE_TYPEDBYREF: return "System.TypedReference";
				default: throw new TypeResolutionException();
			}
		}
	}
}
