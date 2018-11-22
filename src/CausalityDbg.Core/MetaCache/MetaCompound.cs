// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace CausalityDbg.Core.MetaCache
{
	abstract class MetaCompound
	{
		protected MetaCompound()
		{
		}

		public abstract void Apply(IMetaCompoundVisitor visitor);
	}
}
