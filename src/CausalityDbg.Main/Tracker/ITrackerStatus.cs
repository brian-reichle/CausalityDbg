// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using CausalityDbg.Core;

namespace CausalityDbg.Main
{
	interface ITrackerStatus
	{
		void SetStatus(int pid);
		void Notify(TrackerNotificationLevel level, string modulePath, string text);
	}
}
