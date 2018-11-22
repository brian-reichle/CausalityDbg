// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace CausalityDbg.IL
{
	public interface ISigTypeVisitor
	{
		void Visit(SigTypePrimitive element);
		void Visit(SigTypeGen element);
		void Visit(SigTypeUserType element);
		void Visit(SigTypePointer element);
		void Visit(SigTypeGenericInst element);
		void Visit(SigTypeArray element);
		void Visit(SigTypeSZArray element);
		void Visit(SigTypeFNPtr element);
	}
}
