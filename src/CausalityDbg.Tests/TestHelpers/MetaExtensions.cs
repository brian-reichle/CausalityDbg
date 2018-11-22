// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Immutable;
using System.Reflection;
using CausalityDbg.Core.MetaCache;
using CausalityDbg.IL;

namespace CausalityDbg.Tests
{
	static class MetaExtensions
	{
		public static MetaType NewType(this MetaModule module, string name)
		{
			return module.NewType(name, 0);
		}

		public static MetaType NewType(this MetaModule module, string name, int genTypeArgs)
		{
			return new MetaType(module, null, name, genTypeArgs);
		}

		public static MetaType NewType(this MetaType declaringType, string name)
		{
			return declaringType.NewType(name, 0);
		}

		public static MetaType NewType(this MetaType declaringType, string name, int genTypeArgs)
		{
			return new MetaType(declaringType.Module, declaringType, name, declaringType.GenTypeArgs + genTypeArgs);
		}

		public static MetaFunction NewFunction(this MetaModule module, string name, params MetaParameter[] parameters)
		{
			return module.NewFunction(name, 0, parameters);
		}

		public static MetaFunction NewFunction(this MetaModule module, string name, int genTypeArgs, params MetaParameter[] parameters)
		{
			return new MetaFunction(module, null, MetaDataToken.Nil, name, genTypeArgs, CallingConventions.Standard, ImmutableArray.Create(parameters));
		}

		public static MetaFunction NewFunction(this MetaType declaringType, string name, params MetaParameter[] parameters)
		{
			return declaringType.NewFunction(name, 0, parameters);
		}

		public static MetaFunction NewFunction(this MetaType declaringType, string name, int genTypeArgs, params MetaParameter[] parameters)
		{
			return new MetaFunction(declaringType.Module, declaringType, MetaDataToken.Nil, name, genTypeArgs, CallingConventions.Standard, ImmutableArray.Create(parameters));
		}

		public static MetaCompoundClass Init(this MetaType targetType, params MetaCompound[] typeParameters)
		{
			return new MetaCompoundClass(targetType, ImmutableArray.Create(typeParameters));
		}

		public static MetaCompoundArray ToArray(this MetaCompound targetType, int rank)
		{
			return new MetaCompoundArray(targetType, rank);
		}

		public static MetaCompoundByRef ByRef(this MetaCompound targetType)
		{
			return new MetaCompoundByRef(targetType);
		}

		public static MetaCompoundPointer Ptr(this MetaCompound targetType)
		{
			return new MetaCompoundPointer(targetType);
		}

		public static MetaParameter ToParam(this MetaCompound paramType)
		{
			return paramType.ToParam(null);
		}

		public static MetaParameter ToParam(this MetaCompound paramType, string name)
		{
			return new MetaParameter(name, paramType);
		}

		public static MetaFrame ToFrame(this MetaFunction function, params MetaCompound[] typeParameters)
		{
			return function.ToFrame(null, typeParameters);
		}

		public static MetaFrame ToFrame(this MetaFunction function, int? ilOffset, params MetaCompound[] typeParameters)
		{
			return new MetaFrame(function, ilOffset, ImmutableArray.Create(typeParameters));
		}
	}
}
