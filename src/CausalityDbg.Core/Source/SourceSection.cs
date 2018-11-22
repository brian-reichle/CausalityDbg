// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Diagnostics;

namespace CausalityDbg.Core
{
	[DebuggerDisplay("{File} ({FromLine}, {FromColumn})")]
	public sealed class SourceSection
	{
		public SourceSection(string file, int fromLine, int fromColumn, int toLine, int toColumn)
		{
			File = file;
			FromLine = fromLine;
			FromColumn = fromColumn;
			ToLine = toLine;
			ToColumn = toColumn;
		}

		public string File { get; }
		public int FromLine { get; }
		public int FromColumn { get; }
		public int ToLine { get; }
		public int ToColumn { get; }
	}
}
