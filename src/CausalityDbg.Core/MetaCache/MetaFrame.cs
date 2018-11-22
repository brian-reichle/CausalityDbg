// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Immutable;
using System.Diagnostics;

namespace CausalityDbg.Core.MetaCache
{
	[DebuggerDisplay("Frame: {Function.Name} + {ILOffset}")]
	sealed class MetaFrame
	{
		public MetaFrame(MetaFunction function, int? ilOffset, ImmutableArray<MetaCompound> genericArgs)
		{
			if (function == null) throw new ArgumentNullException(nameof(function));

			Function = function;
			ILOffset = ilOffset;
			GenericArgs = genericArgs;
		}

		public MetaFunction Function { get; }
		public int? ILOffset { get; }
		public ImmutableArray<MetaCompound> GenericArgs { get; }
	}
}
