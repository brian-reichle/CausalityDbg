// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace CausalityDbg.Core.CorDebugApi
{
	enum CorDebugThreadState
	{
		/// <summary>
		/// The thread runs freely, unless a debug event occurs.
		/// </summary>
		THREAD_RUN,

		/// <summary>
		/// The thread cannot run.
		/// </summary>
		THREAD_SUSPEND,
	}
}
