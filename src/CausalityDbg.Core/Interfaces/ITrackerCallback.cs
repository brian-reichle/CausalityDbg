// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using CausalityDbg.DataStore;

namespace CausalityDbg.Core
{
	public interface ITrackerCallback
	{
		void StartTracking(int pid);
		void EndTracking();

		void Notify(TrackerNotificationLevel level, string modulePath, string text);

		void NewScope(int scopeId, int? hostScopeId, int? triggerId, long fromTimestamp, DataItem item);
		void NewEvent(int eventId, int hostScopeId, long timestamp, DataItem item);

		void CloseScope(int scopeId, long toTimestamp);

		void ReportException(Exception ex);
	}
}
