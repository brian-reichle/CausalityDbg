// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace CausalityDbg.Core.CorDebugApi
{
	enum CorDebugStepReason
	{
		/// <summary>
		/// Stepping completed normally, within the same function.
		/// </summary>
		STEP_NORMAL = 0,

		/// <summary>
		/// Stepping continued normally, after the function returned.
		/// </summary>
		STEP_RETURN = 1,

		/// <summary>
		/// Stepping continued normally, at the beginning of a newly called function.
		/// </summary>
		STEP_CALL = 2,

		/// <summary>
		/// An exception was generated and control was passed to an exception filter.
		/// </summary>
		STEP_EXCEPTION_FILTER = 3,

		/// <summary>
		/// An exception was generated and control was passed to an exception handler.
		/// </summary>
		STEP_EXCEPTION_HANDLER = 4,

		/// <summary>
		/// Control was passed to an interceptor.
		/// </summary>
		STEP_INTERCEPT = 5,

		/// <summary>
		/// The thread exited before the step was completed.
		/// </summary>
		STEP_EXIT = 6,
	}
}
