// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace CausalityDbg.IL
{
	public static class CorExceptionClauseHelper
	{
		public static bool IsExceptionData(ReadOnlySpan<byte> data)
		{
			return data != null
				&& data.Length >= 0
				&& (data[0] & (byte)CorILMethodSectFlags.CorILMethod_Sect_EHTable) != 0;
		}

		public static bool FilterFromOffset(ReadOnlySpan<byte> data, int offset, out int start, out int end)
		{
			if (IsFat(data))
			{
				return FromFilterOffsetFat(data, offset, out start, out end);
			}
			else
			{
				return FromFilterOffsetSmall(data, offset, out start, out end);
			}
		}

		public static bool HandlerFromOffset(ReadOnlySpan<byte> data, int offset, out int start, out int end, out ExceptionHandlingClauseOptions flags)
		{
			if (IsFat(data))
			{
				return FromHandlerOffsetFat(data, offset, out start, out end, out flags);
			}
			else
			{
				return FromHandlerOffsetSmall(data, offset, out start, out end, out flags);
			}
		}

		static bool IsFat(ReadOnlySpan<byte> blob) => (blob[0] & (byte)CorILMethodSectFlags.CorILMethod_Sect_FatFormat) != 0;

		static bool FromFilterOffsetFat(ReadOnlySpan<byte> data, int offset, out int start, out int end)
		{
			var clauses = MemoryMarshal.Cast<byte, CorFatExceptionClause>(data.Slice(4));

			for (var index = 0; index < clauses.Length; index++)
			{
				ref readonly var clause = ref clauses[index];

				if (clause.Flags == (int)ExceptionHandlingClauseOptions.Filter &&
					clause.FilterOrType <= offset &&
					clause.HandlerOffset > offset)
				{
					end = clause.HandlerOffset;
					start = clause.FilterOrType;
					return true;
				}
			}

			start = end = 0;
			return false;
		}

		static bool FromHandlerOffsetFat(ReadOnlySpan<byte> data, int offset, out int start, out int end, out ExceptionHandlingClauseOptions flags)
		{
			var clauses = MemoryMarshal.Cast<byte, CorFatExceptionClause>(data.Slice(4));

			for (var index = 0; index < clauses.Length; index++)
			{
				ref readonly var clause = ref clauses[index];
				var tmp = offset - clause.HandlerOffset;

				if (tmp >= 0 && tmp < clause.HandlerLength)
				{
					flags = (ExceptionHandlingClauseOptions)clause.Flags;
					start = clause.HandlerOffset;
					end = start + clause.HandlerLength;
					return true;
				}
			}

			start = end = 0;
			flags = default;
			return false;
		}

		static bool FromFilterOffsetSmall(ReadOnlySpan<byte> data, int offset, out int start, out int end)
		{
			var clauses = MemoryMarshal.Cast<byte, CorSmallExceptionClause>(data.Slice(4));

			for (var index = 0; index < clauses.Length; index++)
			{
				ref readonly var clause = ref clauses[index];

				if (clause.Flags == (int)ExceptionHandlingClauseOptions.Filter &&
					clause.FilterOrType <= offset &&
					clause.HandlerOffset > offset)
				{
					end = clause.HandlerOffset;
					start = clause.FilterOrType;
					return true;
				}
			}

			start = end = 0;
			return false;
		}

		static bool FromHandlerOffsetSmall(ReadOnlySpan<byte> data, int offset, out int start, out int end, out ExceptionHandlingClauseOptions flags)
		{
			var clauses = MemoryMarshal.Cast<byte, CorSmallExceptionClause>(data.Slice(4));

			for (var index = 0; index < clauses.Length; index++)
			{
				ref readonly var clause = ref clauses[index];
				var tmp = offset - clause.HandlerOffset;

				if (tmp >= 0 && tmp < clause.HandlerLength)
				{
					flags = (ExceptionHandlingClauseOptions)clause.Flags;
					start = clause.HandlerOffset;
					end = start + clause.HandlerLength;
					return true;
				}
			}

			start = end = 0;
			flags = default;
			return false;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 24)]
		struct CorFatExceptionClause
		{
			public int Flags;
			public int TryOffset;
			public int TryLength;
			public int HandlerOffset;
			public int HandlerLength;
			public int FilterOrType;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
		struct CorSmallExceptionClause
		{
			public ushort Flags;
			public ushort TryOffset;
			public byte TryLength;
			public ushort HandlerOffset;
			public byte HandlerLength;
			public int FilterOrType;
		}
	}
}
