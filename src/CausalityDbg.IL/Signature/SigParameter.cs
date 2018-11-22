// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Immutable;

namespace CausalityDbg.IL
{
	public sealed partial class SigParameter
	{
		public SigParameter(SigType type, bool byRef, int byRefIndex, ImmutableArray<SigCustomModifier> customModifiers)
		{
			ValueType = type;
			ByRef = byRef;
			ByRefIndex = byRefIndex;
			CustomModifiers = customModifiers;
		}

		public bool ByRef { get; }
		public int ByRefIndex { get; }
		public SigType ValueType { get; }
		public ImmutableArray<SigCustomModifier> CustomModifiers { get; }
	}
}
