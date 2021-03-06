// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace CausalityDbg.Core.CorDebugApi
{
	static class StepperExtensions
	{
		public static void StepRange(this ICorDebugStepper stepper, int start, int end)
		{
			stepper.StepRange(
				false,
				new[]
				{
					new COR_DEBUG_STEP_RANGE() { StartOffset = start, EndOffset = end },
				},
				1);
		}
	}
}
