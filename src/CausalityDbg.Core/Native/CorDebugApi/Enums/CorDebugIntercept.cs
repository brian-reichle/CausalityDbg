// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;

namespace CausalityDbg.Core
{
	[Flags]
	enum CorDebugIntercept
	{
		/// <summary>
		/// No code can be intercepted.
		/// </summary>
		INTERCEPT_NONE = 0x0,

		/// <summary>
		/// A constructor can be intercepted.
		/// </summary>
		INTERCEPT_CLASS_INIT = 0x01,

		/// <summary>
		/// An exception filter can be intercepted.
		/// </summary>
		INTERCEPT_EXCEPTION_FILTER = 0x02,

		/// <summary>
		/// Code that enforces security can be intercepted.
		/// </summary>
		INTERCEPT_SECURITY = 0x04,

		/// <summary>
		/// A context policy can be intercepted.
		/// </summary>
		INTERCEPT_CONTEXT_POLICY = 0x08,

		/// <summary>
		/// Not used.
		/// </summary>
		INTERCEPT_INTERCEPTION = 0x10,

		/// <summary>
		/// All code can be intercepted.
		/// </summary>
		INTERCEPT_ALL = 0xffff,
	}
}
