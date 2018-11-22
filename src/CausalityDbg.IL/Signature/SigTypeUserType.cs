// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace CausalityDbg.IL
{
	public sealed partial class SigTypeUserType : SigType
	{
		public SigTypeUserType(CorElementType elementType, MetaDataToken token)
			: base(elementType)
		{
			Token = token;
		}

		public MetaDataToken Token { get; }

		public override void Apply(ISigTypeVisitor visitor) => visitor.Visit(this);
	}
}
