// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using CausalityDbg.Configuration;
using CausalityDbg.DataStore;

namespace CausalityDbg.Main
{
	sealed class FrameWrapper
	{
		public FrameWrapper(TraceWrapper trace, int index, FrameData frame, ConfigCategory category)
		{
			Trace = trace;
			Index = index;
			Frame = frame;
			Category = category;
		}

		public TraceWrapper Trace { get; }
		public int Index { get; }
		public FrameData Frame { get; }
		public ConfigCategory Category { get; }
	}
}
