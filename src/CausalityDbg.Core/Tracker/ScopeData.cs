// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using CausalityDbg.DataStore;

namespace CausalityDbg.Core
{
	sealed class ScopeData
	{
		public ScopeData(ScopeData old, TraceData trace, int id)
		{
			Old = old;
			Trace = trace;
			ScopeId = id;
			Active = true;
		}

		public ScopeData Old { get; }
		public int ScopeId { get; }
		public TraceData Trace { get; }
		public bool Active { get; set; }
	}
}
