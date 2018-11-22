// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CausalityDbg.IL
{
	sealed class CorExceptionFatClauseCollection : CorExceptionClauseCollection
	{
		internal CorExceptionFatClauseCollection(byte[] blob)
			: base(blob)
		{
		}

		public override int Count => (_blob.Length - 4) / 24;
		public override CorExceptionClause this[int index] => ReadRow(_blob, index * 24 + 4);

		public override CorExceptionClause FromFilterOffset(int offset)
		{
			for (var index = 4; index < _blob.Length; index += 24)
			{
				if (BitConverter.ToInt32(_blob, index) == (int)ExceptionHandlingClauseOptions.Filter &&
					BitConverter.ToInt32(_blob, index + 20) <= offset &&
					BitConverter.ToInt32(_blob, index + 12) > offset)
				{
					return ReadRow(_blob, index);
				}
			}

			return null;
		}

		public override CorExceptionClause FromHandlerOffset(int offset)
		{
			for (var index = 4; index < _blob.Length; index += 24)
			{
				var tmp = offset - BitConverter.ToInt32(_blob, index + 12);

				if (tmp >= 0 && tmp < BitConverter.ToInt32(_blob, index + 16))
				{
					return ReadRow(_blob, index);
				}
			}

			return null;
		}

		public override IEnumerator<CorExceptionClause> GetEnumerator()
		{
			for (var index = 4; index < _blob.Length; index += 24)
			{
				yield return ReadRow(_blob, index);
			}
		}

		static CorExceptionClause ReadRow(byte[] dataSection, int offset)
		{
			var flags = BitConverter.ToInt32(dataSection, offset);
			var tryOffset = BitConverter.ToInt32(dataSection, offset + 4);
			var tryLength = BitConverter.ToInt32(dataSection, offset + 8);
			var handlerOffset = BitConverter.ToInt32(dataSection, offset + 12);
			var handlerLength = BitConverter.ToInt32(dataSection, offset + 16);
			var data = BitConverter.ToInt32(dataSection, offset + 20);

			return new CorExceptionClause(
				(ExceptionHandlingClauseOptions)flags,
				tryOffset,
				tryLength,
				handlerOffset,
				handlerLength,
				data);
		}
	}
}
