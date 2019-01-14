// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Diagnostics;

namespace CausalityDbg.DataStore
{
	[DebuggerDisplay("Scope ({ID})")]
	public sealed class Scope : IEventScope
	{
		public Scope(Band band, Event trigger, DataItem item, int id, long fromTimestamp)
		{
			Band = band;
			Host = null;
			Trigger = trigger;
			Item = item;
			ID = id;
			Depth = 0;
			FromTimestamp = fromTimestamp;
			ToTimestamp = null;
		}

		public Scope(Scope host, Event trigger, DataItem item, int id, long fromTimestamp)
		{
			Band = host.Band;
			Host = host;
			Trigger = trigger;
			Item = item;
			ID = id;
			Depth = host.Depth + 1;
			FromTimestamp = fromTimestamp;
			ToTimestamp = null;
		}

		internal void Close(long toTimestamp)
		{
			ToTimestamp = toTimestamp;
		}

		public Band Band { get; }
		public Scope Host { get; }
		public Event Trigger { get; }
		public DataItem Item { get; }
		public int ID { get; }
		public int Depth { get; }
		public long FromTimestamp { get; }
		public long? ToTimestamp { get; private set; }
	}
}
