// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Immutable;

namespace CausalityDbg.IL
{
	public sealed partial class SigTypeArray : SigType
	{
		public SigTypeArray(
			SigType baseType,
			uint rank,
			ImmutableArray<uint> sizes,
			ImmutableArray<int> lowerBounds)
			: base(CorElementType.ELEMENT_TYPE_ARRAY)
		{
			BaseType = baseType;
			Rank = rank;
			Sizes = sizes;
			LowerBounds = lowerBounds;
		}

		public SigType BaseType { get; }
		public uint Rank { get; }
		public ImmutableArray<uint> Sizes { get; }
		public ImmutableArray<int> LowerBounds { get; }

		public override void Apply(ISigTypeVisitor visitor) => visitor.Visit(this);
	}
}
