// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Immutable;
using System.Reflection;

namespace CausalityDbg.IL
{
	public sealed partial class SigMethod
	{
		public SigMethod(
			CallingConventions callingConvention,
			uint genParamCount,
			int orderedParamCount,
			SigParameter retType,
			ImmutableArray<SigParameter> parameters)
		{
			CallingConvention = callingConvention;
			GenParamCount = genParamCount;
			OrderedParamCount = orderedParamCount;
			RetType = retType;
			Parameters = parameters;
		}

		public CallingConventions CallingConvention { get; }
		public uint GenParamCount { get; }
		public int OrderedParamCount { get; }
		public SigParameter RetType { get; }
		public ImmutableArray<SigParameter> Parameters { get; }
	}
}
