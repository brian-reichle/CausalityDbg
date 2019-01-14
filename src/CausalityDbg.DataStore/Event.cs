// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace CausalityDbg.DataStore
{
	public sealed class Event : IEventScope
	{
		public Event(Scope host, DataItem item, long timestamp)
		{
			Band = host.Band;
			Host = host;
			Item = item;
			Timestamp = timestamp;
		}

		public Band Band { get; }
		public Scope Host { get; }
		public DataItem Item { get; }
		public long Timestamp { get; }
	}
}
