// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Diagnostics;
using System.Reflection;

namespace CausalityDbg.IL
{
	[DebuggerDisplay("{Flags}: {TryOffset}-{TryLength} {HandlerOffset}-{HandlerLength}")]
	public sealed class CorExceptionClause
	{
		public CorExceptionClause(
			ExceptionHandlingClauseOptions flags,
			int tryOffset,
			int tryLength,
			int handlerOffset,
			int handlerLength,
			int data)
		{
			Flags = flags;
			TryOffset = tryOffset;
			TryLength = tryLength;
			HandlerOffset = handlerOffset;
			HandlerLength = handlerLength;
			_data = data;
		}

		public ExceptionHandlingClauseOptions Flags { get; }
		public int TryOffset { get; }
		public int TryLength { get; }
		public int HandlerOffset { get; }
		public int HandlerLength { get; }

		public MetaDataToken ClassToken
		{
			get
			{
				if (Flags == ExceptionHandlingClauseOptions.Clause)
				{
					return new MetaDataToken((uint)_data);
				}
				else
				{
					return MetaDataToken.Nil;
				}
			}
		}

		public int FilterOffset
		{
			get
			{
				if (Flags == ExceptionHandlingClauseOptions.Filter)
				{
					return _data;
				}
				else
				{
					return 0;
				}
			}
		}

		readonly int _data;
	}
}
