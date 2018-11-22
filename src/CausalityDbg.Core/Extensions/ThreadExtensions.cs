// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using CausalityDbg.Core.CorDebugApi;

namespace CausalityDbg.Core
{
	static class ThreadExtensions
	{
		public static bool HasQueuedCallbacks(this ICorDebugThread thread)
		{
			return thread.GetProcess().HasQueuedCallbacks(thread);
		}
	}
}
