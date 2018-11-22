// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using CausalityDbg.IL;

namespace CausalityDbg.Core.MetaCache
{
	[DebuggerDisplay("Function: {Name}")]
	sealed class MetaFunction
	{
		public MetaFunction(MetaModule module, MetaType declaringType, MetaDataToken token, string name, int genTypeArgs, CallingConventions callingConvention, ImmutableArray<MetaParameter> parameters)
		{
			if (module == null) throw new ArgumentNullException(nameof(module));
			if (name == null) throw new ArgumentNullException(nameof(name));

			Module = module;
			DeclaringType = declaringType;
			Token = token;
			Name = name;
			GenTypeArgs = genTypeArgs;
			CallingConvention = callingConvention;
			Parameters = parameters;
		}

		public MetaModule Module { get; }
		public MetaType DeclaringType { get; }
		public MetaDataToken Token { get; }
		public string Name { get; }
		public int GenTypeArgs { get; }
		public CallingConventions CallingConvention { get; }
		public ImmutableArray<MetaParameter> Parameters { get; }
	}
}
