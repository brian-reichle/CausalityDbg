// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using CausalityDbg.Configuration;

namespace CausalityDbg.Core
{
	public sealed class TraceDataItem : DataItem
	{
		public TraceDataItem(ConfigCategory category, TraceData stackTrace, string text)
			: base(category, stackTrace)
		{
			Text = text;
		}

		public string Text { get; }
	}
}
