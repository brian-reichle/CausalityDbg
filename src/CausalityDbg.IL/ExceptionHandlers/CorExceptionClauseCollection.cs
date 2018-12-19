// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace CausalityDbg.IL
{
	public abstract class CorExceptionClauseCollection : IEnumerable<CorExceptionClause>
	{
		public static CorExceptionClauseCollection New(byte[] blob)
		{
			if (blob == null || blob.Length == 0)
			{
				return null;
			}

			var flags = blob[0];

			if ((flags & (byte)CorILMethodSectFlags.CorILMethod_Sect_EHTable) == 0)
			{
				return null;
			}

			if ((flags & (byte)CorILMethodSectFlags.CorILMethod_Sect_FatFormat) == 0)
			{
				return new CorExceptionSmallClauseCollection(blob);
			}
			else
			{
				return new CorExceptionFatClauseCollection(blob);
			}
		}

		private protected CorExceptionClauseCollection()
		{
		}

		public abstract CorExceptionClause FromFilterOffset(int offset);

		public abstract CorExceptionClause FromHandlerOffset(int offset);

		public abstract IEnumerator<CorExceptionClause> GetEnumerator();

		[DebuggerStepThrough]
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
