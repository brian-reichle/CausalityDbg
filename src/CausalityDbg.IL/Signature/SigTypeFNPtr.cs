// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace CausalityDbg.IL
{
	public sealed partial class SigTypeFNPtr : SigType
	{
		public SigTypeFNPtr(SigMethod method)
			: base(CorElementType.ELEMENT_TYPE_FNPTR)
		{
			Method = method;
		}

		public SigMethod Method { get; }

		public override TResult Apply<TArg, TResult>(ISigTypeVisitor<TArg, TResult> visitor, TArg arg) => visitor.Visit(this, arg);
	}
}
