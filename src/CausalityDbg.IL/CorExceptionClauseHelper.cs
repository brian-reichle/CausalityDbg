// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Reflection;

namespace CausalityDbg.IL
{
	public static class CorExceptionClauseHelper
	{
		public static bool IsExceptionData(byte[] data)
		{
			return data != null
				&& data.Length >= 0
				&& (data[0] & (byte)CorILMethodSectFlags.CorILMethod_Sect_EHTable) != 0;
		}

		public static bool FilterFromOffset(byte[] data, int offset, out int start, out int end)
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

		public static bool HandlerFromOffset(byte[] data, int offset, out int start, out int end, out ExceptionHandlingClauseOptions flags)
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

		static bool IsFat(byte[] blob) => (blob[0] & (byte)CorILMethodSectFlags.CorILMethod_Sect_FatFormat) != 0;

		static bool FromFilterOffsetFat(byte[] data, int offset, out int start, out int end)
		{
			for (var index = 4; index < data.Length; index += 24)
			{
				if (BitConverter.ToInt32(data, index) == (int)ExceptionHandlingClauseOptions.Filter &&
					BitConverter.ToInt32(data, index + 20) <= offset &&
					BitConverter.ToInt32(data, index + 12) > offset)
				{
					end = BitConverter.ToInt32(data, index + 12);
					start = BitConverter.ToInt32(data, index + 20);
					return true;
				}
			}

			start = end = 0;
			return false;
		}

		static bool FromHandlerOffsetFat(byte[] data, int offset, out int start, out int end, out ExceptionHandlingClauseOptions flags)
		{
			for (var index = 4; index < data.Length; index += 24)
			{
				var tmp = offset - BitConverter.ToInt32(data, index + 12);

				if (tmp >= 0 && tmp < BitConverter.ToInt32(data, index + 16))
				{
					flags = (ExceptionHandlingClauseOptions)BitConverter.ToInt32(data, index);
					start = BitConverter.ToInt32(data, index + 12);
					end = start + BitConverter.ToInt32(data, index + 16);
					return true;
				}
			}

			start = end = 0;
			flags = default;
			return false;
		}

		static bool FromFilterOffsetSmall(byte[] data, int offset, out int start, out int end)
		{
			for (var index = 4; index < data.Length; index += 12)
			{
				if (BitConverter.ToInt16(data, index) == (int)ExceptionHandlingClauseOptions.Filter &&
					BitConverter.ToInt32(data, index + 8) <= offset &&
					BitConverter.ToInt16(data, index + 5) > offset)
				{
					end = BitConverter.ToInt16(data, index + 5);
					start = BitConverter.ToInt32(data, index + 8);
					return true;
				}
			}

			start = end = 0;
			return false;
		}

		static bool FromHandlerOffsetSmall(byte[] data, int offset, out int start, out int end, out ExceptionHandlingClauseOptions flags)
		{
			for (var index = 4; index < data.Length; index += 12)
			{
				var tmp = offset - BitConverter.ToInt16(data, index + 5);

				if (tmp >= 0 && tmp < data[index + 7])
				{
					flags = (ExceptionHandlingClauseOptions)BitConverter.ToInt16(data, index);
					start = BitConverter.ToInt16(data, index + 5);
					end = start + data[index + 7];
					return true;
				}
			}

			start = end = 0;
			flags = default;
			return false;
		}
	}
}
