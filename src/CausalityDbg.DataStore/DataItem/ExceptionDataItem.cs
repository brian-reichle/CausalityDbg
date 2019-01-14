// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using CausalityDbg.Configuration;

namespace CausalityDbg.DataStore
{
	public sealed class ExceptionDataItem : DataItem
	{
		public ExceptionDataItem(ConfigCategory category, TraceData stackTrace, string exceptionType)
			: base(category, stackTrace)
		{
			ExceptionType = exceptionType;
		}

		public string ExceptionType { get; }
		public string ExceptionMessage { get; set; }
	}
}
