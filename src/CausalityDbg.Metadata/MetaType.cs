// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Diagnostics;

namespace CausalityDbg.Metadata
{
	[DebuggerDisplay("Type: {Name}")]
	public sealed class MetaType
	{
		internal MetaType(MetaModule module, MetaType declaringType, string name, int genTypeArgs)
		{
			if (module == null) throw new ArgumentNullException(nameof(module));
			if (name == null) throw new ArgumentNullException(nameof(name));

			Module = module;
			DeclaringType = declaringType;
			Name = name;
			GenTypeArgs = genTypeArgs;
		}

		public MetaModule Module { get; }
		public MetaType DeclaringType { get; }
		public string Name { get; }
		public int GenTypeArgs { get; }
	}
}
