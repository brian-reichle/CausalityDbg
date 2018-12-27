// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using CausalityDbg.Configuration;

namespace CausalityDbg.Core
{
	public static class TrackerFactory
	{
		public static ITracker New(Config config, ITrackerCallback callback, int pid)
		{
			return new AttachTracker(config, callback, pid);
		}

		public static ITracker New(Config config, ITrackerCallback callback, LaunchArguments launchArguments)
		{
			return new LaunchTracker(config, callback, launchArguments);
		}
	}
}
