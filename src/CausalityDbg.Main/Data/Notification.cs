// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using CausalityDbg.Core;

namespace CausalityDbg.Main
{
	sealed class Notification
	{
		public Notification(TrackerNotificationLevel level, string module, string text)
		{
			Level = level;
			Module = module;
			Text = text;
		}

		public TrackerNotificationLevel Level { get; }
		public string Module { get; }
		public string Text { get; }
	}
}
