// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;

namespace CausalityDbg.Core
{
	static class SpanUtils
	{
		public static unsafe ReadOnlySpan<T> Create<T>(IntPtr ptr, int length) => new ReadOnlySpan<T>((void*)ptr, length);
	}
}
