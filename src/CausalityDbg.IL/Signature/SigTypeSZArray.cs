// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Immutable;

namespace CausalityDbg.IL
{
	public sealed partial class SigTypeSZArray : SigType
	{
		public SigTypeSZArray(SigType baseType, ImmutableArray<SigCustomModifier> customModifiers)
			: base(CorElementType.ELEMENT_TYPE_SZARRAY)
		{
			BaseType = baseType;
			CustomModifiers = customModifiers;
		}

		public SigType BaseType { get; }
		public ImmutableArray<SigCustomModifier> CustomModifiers { get; }

		public override TResult Apply<TArg, TResult>(ISigTypeVisitor<TArg, TResult> visitor, TArg arg) => visitor.Visit(this, arg);
	}
}
