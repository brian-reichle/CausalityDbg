// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace CausalityDbg.IL
{
	public sealed partial class SigTypeGen : SigType
	{
		public SigTypeGen(CorElementType elementType, uint index)
			: base(elementType)
		{
			Index = index;
		}

		public uint Index { get; }

		public override void Apply(ISigTypeVisitor visitor) => visitor.Visit(this);
	}
}
