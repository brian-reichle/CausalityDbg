// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Diagnostics;

namespace CausalityDbg.DataStore
{
	[DebuggerDisplay("Band {ID}")]
	public sealed class Band
	{
		public Band(int id)
		{
			ID = id;
		}

		public int ID { get; }
		public int MaxDepth { get; set; }
		public long Available { get; set; }
	}
}
