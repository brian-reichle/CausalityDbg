// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace CausalityDbg.DataStore
{
	public interface IEventScope
	{
		Band Band { get; }
		Scope Host { get; }
		DataItem Item { get; }
	}
}
