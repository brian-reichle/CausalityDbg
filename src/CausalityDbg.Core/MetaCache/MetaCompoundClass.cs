// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Immutable;
using System.Diagnostics;

namespace CausalityDbg.Core.MetaCache
{
	[DebuggerDisplay("Class: {TargetType.Name}")]
	sealed class MetaCompoundClass : MetaCompound
	{
		public MetaCompoundClass(MetaType targetType, ImmutableArray<MetaCompound> genericArgs)
		{
			if (targetType == null) throw new ArgumentNullException(nameof(targetType));

			TargetType = targetType;
			GenericArgs = genericArgs;
		}

		public MetaType TargetType { get; }
		public ImmutableArray<MetaCompound> GenericArgs { get; }
		public override void Apply(IMetaCompoundVisitor visitor) => visitor.Visit(this);
	}
}
