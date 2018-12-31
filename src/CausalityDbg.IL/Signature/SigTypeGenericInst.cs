// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Immutable;

namespace CausalityDbg.IL
{
	public sealed partial class SigTypeGenericInst : SigType
	{
		public SigTypeGenericInst(SigTypeUserType template, ImmutableArray<SigType> genArguments)
			: base(CorElementType.ELEMENT_TYPE_GENERICINST)
		{
			Template = template;
			GenArguments = genArguments;
		}

		public SigTypeUserType Template { get; }
		public ImmutableArray<SigType> GenArguments { get; }

		public override TResult Apply<TArg, TResult>(ISigTypeVisitor<TArg, TResult> visitor, TArg arg) => visitor.Visit(this, arg);
	}
}
