// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using CausalityDbg.Core.CorDebugApi;

namespace CausalityDbg.Core
{
	sealed class ExceptionData
	{
		public ExceptionData(ICorDebugStepper initialStepper) => CurrentStepper = initialStepper;
		public ICorDebugStepper CurrentStepper { get; set; }
	}
}
