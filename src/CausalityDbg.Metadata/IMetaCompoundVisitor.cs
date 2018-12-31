// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace CausalityDbg.Metadata
{
	public interface IMetaCompoundVisitor
	{
		void Visit(MetaCompoundArray type);
		void Visit(MetaCompoundByRef type);
		void Visit(MetaCompoundClass type);
		void Visit(MetaCompoundGenArg type);
		void Visit(MetaCompoundPointer type);
	}
}
