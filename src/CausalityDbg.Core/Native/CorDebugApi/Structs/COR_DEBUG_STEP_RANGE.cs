// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Runtime.InteropServices;

namespace CausalityDbg.Core.CorDebugApi
{
	[StructLayout(LayoutKind.Sequential)]
	struct COR_DEBUG_STEP_RANGE
	{
		// ULONG32 startOffset;
		public int StartOffset;

		// ULONG32 endOffset;
		public int EndOffset;
	}
}
