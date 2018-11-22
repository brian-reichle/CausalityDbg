// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;

namespace CausalityDbg.Core.CorDebugApi
{
	[Flags]
	enum CorDebugExceptionFlags
	{
		None = 0x0000,

		/// <summary>
		/// The exception is interceptable.
		/// </summary>
		DEBUG_EXCEPTION_CAN_BE_INTERCEPTED = 0x0001,
	}
}
