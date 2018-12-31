// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace CausalityDbg.IL
{
	public interface ISigTypeVisitor<TArg, TResult>
	{
		TResult Visit(SigTypePrimitive element, TArg arg);
		TResult Visit(SigTypeGen element, TArg arg);
		TResult Visit(SigTypeUserType element, TArg arg);
		TResult Visit(SigTypePointer element, TArg arg);
		TResult Visit(SigTypeGenericInst element, TArg arg);
		TResult Visit(SigTypeArray element, TArg arg);
		TResult Visit(SigTypeSZArray element, TArg arg);
		TResult Visit(SigTypeFNPtr element, TArg arg);
	}
}
