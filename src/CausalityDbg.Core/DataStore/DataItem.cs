// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Diagnostics;
using CausalityDbg.Configuration;

namespace CausalityDbg.Core
{
	[DebuggerDisplay("Category = {Category.Name}")]
	public class DataItem
	{
		public DataItem(ConfigCategory category, TraceData stackTrace)
		{
			Category = category;
			StackTrace = stackTrace;
		}

		public ConfigCategory Category { get; }
		public TraceData StackTrace { get; }
	}
}
