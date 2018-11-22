// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace CausalityDbg.IL
{
	public sealed partial class SigCustomModifier
	{
		public SigCustomModifier(CorElementType elementType, MetaDataToken token)
		{
			ElementType = elementType;
			Token = token;
		}

		public CorElementType ElementType { get; }
		public MetaDataToken Token { get; }
	}
}
