// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using CausalityDbg.IL;

namespace CausalityDbg.Core
{
	static class ElementTypeExtensions
	{
		public static string GetFullName(this CorElementType type)
		{
			return type switch
			{
				CorElementType.ELEMENT_TYPE_BOOLEAN => "System.Boolean",
				CorElementType.ELEMENT_TYPE_CHAR => "System.Char",
				CorElementType.ELEMENT_TYPE_I1 => "System.SByte",
				CorElementType.ELEMENT_TYPE_U1 => "System.Byte",
				CorElementType.ELEMENT_TYPE_I2 => "System.Int16",
				CorElementType.ELEMENT_TYPE_U2 => "System.UInt16",
				CorElementType.ELEMENT_TYPE_I4 => "System.Int32",
				CorElementType.ELEMENT_TYPE_U4 => "System.UInt32",
				CorElementType.ELEMENT_TYPE_I8 => "System.Int64",
				CorElementType.ELEMENT_TYPE_U8 => "System.UInt64",
				CorElementType.ELEMENT_TYPE_R4 => "System.Single",
				CorElementType.ELEMENT_TYPE_R8 => "System.Double",
				CorElementType.ELEMENT_TYPE_I => "System.IntPtr",
				CorElementType.ELEMENT_TYPE_U => "System.UIntPtr",
				CorElementType.ELEMENT_TYPE_OBJECT => "System.Object",
				CorElementType.ELEMENT_TYPE_STRING => "System.String",
				CorElementType.ELEMENT_TYPE_VOID => "System.Void",
				CorElementType.ELEMENT_TYPE_TYPEDBYREF => "System.TypedReference",
				_ => throw new TypeResolutionException(),
			};
		}
	}
}
