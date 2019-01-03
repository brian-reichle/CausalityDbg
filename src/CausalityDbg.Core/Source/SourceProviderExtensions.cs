// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;

namespace CausalityDbg.Core
{
	public static class SourceProviderExtensions
	{
		public static SourceSection GetSourceSection(this ISourceProvider provider, FrameILData frame)
		{
			if (provider == null) throw new ArgumentNullException(nameof(provider));
			if (frame == null) throw new ArgumentNullException(nameof(frame));

			return provider.GetSourceSection(frame.Function, frame.ILOffset.GetValueOrDefault());
		}
	}
}
