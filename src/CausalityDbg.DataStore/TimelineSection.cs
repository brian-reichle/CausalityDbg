// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Diagnostics;

namespace CausalityDbg.DataStore
{
	[DebuggerDisplay("({ViewStart}-{ViewEnd})")]
	public sealed class TimelineSection
	{
		public TimelineSection(long realOffset, long viewOffset, long duration)
		{
			RealStart = realOffset;
			ViewStart = viewOffset;
			Duration = duration;
		}

		public long RealStart { get; }
		public long ViewStart { get; }
		public long RealEnd => RealStart + Duration - 1;
		public long ViewEnd => ViewStart + Duration - 1;
		public long Duration { get; set; }
	}
}
