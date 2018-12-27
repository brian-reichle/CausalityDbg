// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace CausalityDbg.Configuration
{
	public enum ConfigCategoryType
	{
		ManagedThread,
		Exception,
		Break,
		Trace,

		Catch,
		Filter,
		Finally,
		Fault,

		// Leave 'Custom' as the last value.
		Custom,
	}
}
