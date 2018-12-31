// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Immutable;
using System.Reflection;
using CausalityDbg.IL;
using CausalityDbg.Metadata;

namespace CausalityDbg.Tests
{
	static class MetaExtensions
	{
		public static MetaType NewType(this MetaModule module, string name)
			=> module.NewType(name, 0);

		public static MetaType NewType(this MetaModule module, string name, int genTypeArgs)
			=> module.CreateType(name, genTypeArgs);

		public static MetaType NewType(this MetaType declaringType, string name)
			=> declaringType.NewType(name, 0);

		public static MetaType NewType(this MetaType declaringType, string name, int genTypeArgs)
			=> declaringType.CreateType(name, declaringType.GenTypeArgs + genTypeArgs);

		public static MetaFunction NewFunction(this MetaModule module, string name, params MetaParameter[] parameters)
			=> module.NewFunction(name, 0, parameters);

		public static MetaFunction NewFunction(this MetaModule module, string name, int genTypeArgs, params MetaParameter[] parameters)
			=> module.CreateFunction(MetaDataToken.Nil, name, genTypeArgs, CallingConventions.Standard, ImmutableArray.Create(parameters));

		public static MetaFunction NewFunction(this MetaType declaringType, string name, params MetaParameter[] parameters)
			=> declaringType.NewFunction(name, 0, parameters);

		public static MetaFunction NewFunction(this MetaType declaringType, string name, int genTypeArgs, params MetaParameter[] parameters)
			=> declaringType.CreateFunction(MetaDataToken.Nil, name, genTypeArgs, CallingConventions.Standard, ImmutableArray.Create(parameters));

		public static MetaCompoundClass CreateCompound(this MetaType targetType, params MetaCompound[] typeParameters)
			=> targetType.CreateCompound(ImmutableArray.Create(typeParameters));

		public static MetaParameter ToParam(this MetaCompound paramType) => paramType.ToParam(null);
	}
}
