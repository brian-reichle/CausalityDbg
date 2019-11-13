// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Immutable;
using System.Reflection;
using CausalityDbg.IL;

namespace CausalityDbg.Metadata
{
	public static class MetaFactory
	{
		public static MetaCompoundArray CreateArray(this MetaCompound elementType, int rank)
			=> new MetaCompoundArray(elementType, rank);

		public static MetaCompoundPointer CreatePointer(this MetaCompound baseType)
			=> new MetaCompoundPointer(baseType);

		public static MetaCompoundByRef CreateByRef(this MetaCompound baseType)
			=> new MetaCompoundByRef(baseType);

		public static MetaCompoundClass CreateCompound(this MetaType type)
			=> new MetaCompoundClass(type, ImmutableArray<MetaCompound>.Empty);

		public static MetaCompoundClass CreateCompound(this MetaType type, ImmutableArray<MetaCompound> genericTypeArgs)
			=> new MetaCompoundClass(type, genericTypeArgs);

		public static MetaCompoundGenArg CreateTypeArg(bool method, int index)
			=> new MetaCompoundGenArg(method, index);

		public static MetaModule CreateModule(string name, MetaModuleFlags flags)
			=> new MetaModule(name, flags);

		public static MetaType CreateType(this MetaModule module, string name, int genTypeArgs)
			=> new MetaType(module, null, name, genTypeArgs);

		public static MetaType CreateType(this MetaType declaringType, string name, int genTypeArgs)
		{
			if (declaringType == null)
			{
				throw new ArgumentNullException(nameof(declaringType));
			}

			return new MetaType(declaringType.Module, declaringType, name, genTypeArgs);
		}

		public static MetaFunction CreateFunction(this MetaModule module, MetaDataToken token, string name, int genTypeArgs, CallingConventions callingConvention, ImmutableArray<MetaParameter> parameters)
			=> new MetaFunction(module, null, token, name, genTypeArgs, callingConvention, parameters);

		public static MetaFunction CreateFunction(this MetaType declaringType, MetaDataToken token, string name, int genTypeArgs, CallingConventions callingConvention, ImmutableArray<MetaParameter> parameters)
		{
			if (declaringType == null)
			{
				throw new ArgumentNullException(nameof(declaringType));
			}

			return new MetaFunction(declaringType.Module, declaringType, token, name, genTypeArgs, callingConvention, parameters);
		}

		public static MetaParameter ToParam(this MetaCompound parameterType, string name)
			=> new MetaParameter(name, parameterType);
	}
}
