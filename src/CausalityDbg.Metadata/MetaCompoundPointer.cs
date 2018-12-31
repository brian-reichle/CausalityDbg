// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;

namespace CausalityDbg.Metadata
{
	public sealed class MetaCompoundPointer : MetaCompound
	{
		internal MetaCompoundPointer(MetaCompound targetType)
		{
			if (targetType == null) throw new ArgumentNullException(nameof(targetType));

			TargetType = targetType;
		}

		public MetaCompound TargetType { get; }

		public override void Apply(IMetaCompoundVisitor visitor) => visitor.Visit(this);
	}
}
