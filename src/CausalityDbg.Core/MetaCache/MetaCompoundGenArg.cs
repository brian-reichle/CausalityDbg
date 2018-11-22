// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Diagnostics;

namespace CausalityDbg.Core.MetaCache
{
	[DebuggerDisplay("GenArg: {Index}, Method={Method}")]
	sealed class MetaCompoundGenArg : MetaCompound
	{
		public MetaCompoundGenArg(bool method, int index)
		{
			Method = method;
			Index = index;
		}

		public bool Method { get; }
		public int Index { get; }

		public override void Apply(IMetaCompoundVisitor visitor) => visitor.Visit(this);
	}
}
