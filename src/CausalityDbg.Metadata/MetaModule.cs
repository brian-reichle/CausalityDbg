// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Diagnostics;

namespace CausalityDbg.Metadata
{
	[DebuggerDisplay("Module: {Name}")]
	public sealed class MetaModule
	{
		internal MetaModule(string name, MetaModuleFlags flags)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));

			Name = name;
			Flags = flags;
		}

		public string Name { get; }
		public MetaModuleFlags Flags { get; }
	}
}
