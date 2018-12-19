// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace CausalityDbg.IL
{
	sealed class CorExceptionSmallClauseCollection : CorExceptionClauseCollection
	{
		internal CorExceptionSmallClauseCollection(byte[] blob)
		{
			_blob = blob;
		}

		public override CorExceptionClause FromFilterOffset(int offset)
		{
			for (var index = 4; index < _blob.Length; index += 12)
			{
				if (BitConverter.ToInt16(_blob, index) == (int)ExceptionHandlingClauseOptions.Filter &&
					BitConverter.ToInt32(_blob, index + 8) <= offset &&
					BitConverter.ToInt16(_blob, index + 5) > offset)
				{
					return ReadRow(_blob, index);
				}
			}

			return null;
		}

		public override CorExceptionClause FromHandlerOffset(int offset)
		{
			for (var index = 4; index < _blob.Length; index += 12)
			{
				var tmp = offset - BitConverter.ToInt16(_blob, index + 5);

				if (tmp >= 0 && tmp < _blob[index + 7])
				{
					return ReadRow(_blob, index);
				}
			}

			return null;
		}

		public override IEnumerator<CorExceptionClause> GetEnumerator()
		{
			for (var index = 4; index < _blob.Length; index += 12)
			{
				yield return ReadRow(_blob, index);
			}
		}

		static CorExceptionClause ReadRow(byte[] dataSection, int offset)
		{
			var flags = BitConverter.ToInt16(dataSection, offset);
			var tryOffset = BitConverter.ToInt16(dataSection, offset + 2);
			var tryLength = dataSection[offset + 4];
			var handlerOffset = BitConverter.ToInt16(dataSection, offset + 5);
			var handlerLength = dataSection[offset + 7];
			var data = BitConverter.ToInt32(dataSection, offset + 8);

			return new CorExceptionClause(
				(ExceptionHandlingClauseOptions)flags,
				tryOffset,
				tryLength,
				handlerOffset,
				handlerLength,
				data);
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly byte[] _blob;
	}
}
