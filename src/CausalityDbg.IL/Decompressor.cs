// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;

namespace CausalityDbg.IL
{
	static class Decompressor
	{
		public static uint ReadCompressedUInt(ReadOnlySpan<byte> blob, ref int index)
		{
			if (index >= blob.Length)
			{
				throw new InvalidSignatureException();
			}

			uint tmp = blob[index];

			if ((tmp & 0x80) == 0)
			{
				index++;
			}
			else if ((tmp & 0xC0) == 0x80)
			{
				if (index + 1 >= blob.Length)
				{
					throw new InvalidSignatureException();
				}

				tmp = ((tmp & 0x3F) << 8) | blob[index + 1];
				index += 2;
			}
			else if ((tmp & 0xE0) == 0xC0)
			{
				if (index + 3 >= blob.Length)
				{
					throw new InvalidSignatureException();
				}

				tmp = ((tmp & 0x1f) << 8) | blob[index + 1];
				tmp = (tmp << 8) | blob[index + 2];
				tmp = (tmp << 8) | blob[index + 3];
				index += 4;
			}
			else
			{
				tmp = 0;
				index++;
			}

			return tmp;
		}

		public static int ReadCompressedInt(ReadOnlySpan<byte> blob, ref int index)
		{
			if (index >= blob.Length)
			{
				throw new InvalidSignatureException();
			}

			int tmp = blob[index];
			int mask;

			if ((tmp & 0x80) == 0)
			{
				index++;
				mask = unchecked((int)0xFFFFFFC0);
			}
			else if ((tmp & 0xC0) == 0x80)
			{
				if (index + 1 >= blob.Length)
				{
					throw new InvalidSignatureException();
				}

				tmp = ((tmp & 0x3F) << 8) | blob[index + 1];
				index += 2;
				mask = unchecked((int)0xFFFFE000);
			}
			else if ((tmp & 0xE0) == 0xC0)
			{
				if (index + 3 >= blob.Length)
				{
					throw new InvalidSignatureException();
				}

				tmp = ((tmp & 0x1f) << 8) | blob[index + 1];
				tmp = (tmp << 8) | blob[index + 2];
				tmp = (tmp << 8) | blob[index + 3];
				index += 4;
				mask = unchecked((int)0xF0000000);
			}
			else
			{
				tmp = 0;
				index++;
				mask = 0;
			}

			if ((tmp & 1) == 0)
			{
				mask = 0;
			}

			tmp = (tmp >> 1) | mask;

			return tmp;
		}

		public static MetaDataToken ReadTypeDefOrRefOrSpecEncoded(ReadOnlySpan<byte> blob, ref int index)
		{
			var val = ReadCompressedUInt(blob, ref index);

			var type = (val & 0x03) switch
			{
				0 => TokenType.TypeDef,
				1 => TokenType.TypeRef,
				2 => TokenType.TypeSpec,
				_ => throw new InvalidSignatureException(),
			};
			return new MetaDataToken(unchecked((uint)type) | val >> 2);
		}
	}
}
