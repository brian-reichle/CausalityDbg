// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace CausalityDbg.IL
{
	public abstract partial class SigType
	{
		protected SigType(CorElementType elementType)
		{
			ElementType = elementType;
		}

		public CorElementType ElementType { get; }

		public abstract TResult Apply<TArg, TResult>(ISigTypeVisitor<TArg, TResult> visitor, TArg arg);
	}
}
