// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace CausalityDbg.Core
{
	public sealed class ExceptionDataItem : DataItem
	{
		public ExceptionDataItem(ConfigCategory category, TraceData stackTrace, string exceptionType)
			: base(category, stackTrace)
		{
			ExceptionType = exceptionType;
		}

		public string ExceptionType { get; }
		public string ExceptionMessage { get; internal set; }
	}
}
