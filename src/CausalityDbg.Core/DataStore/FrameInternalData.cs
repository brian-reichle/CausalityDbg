// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Diagnostics;

namespace CausalityDbg.Core
{
	[DebuggerDisplay("Text = {Text}")]
	public sealed class FrameInternalData : FrameData
	{
		internal FrameInternalData(string text)
		{
			Text = text;
		}

		public string Text { get; }
	}
}
