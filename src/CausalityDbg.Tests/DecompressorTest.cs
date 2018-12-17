// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using CausalityDbg.IL;
using NUnit.Framework;

namespace CausalityDbg.Tests
{
	[TestFixture]
	sealed class DecompressorTest
	{
		[TestCase("03", 0x03)]
		[TestCase("7F", 0x7F)]
		[TestCase("FF", 0x00)]
		[TestCase("8080", 0x80)]
		[TestCase("AE57", 0x2E57)]
		[TestCase("BFFF", 0x3FFF)]
		[TestCase("C0004000", 0x4000)]
		[TestCase("DFFFFFFF", 0x1FFFFFFF)]
		public void DecompressUnsigned(string hex, int expectedValue)
		{
			var blob = BlobFromHex(hex);
			var index = 0;
			var val = Decompressor.ReadCompressedUInt(blob, ref index);

			Assert.That(index, Is.EqualTo(blob.Length));
			Assert.That(val, Is.EqualTo(expectedValue));
		}

		[TestCase("06", 03)]
		[TestCase("7B", -3)]
		[TestCase("FF", 0)]
		[TestCase("8080", 64)]
		[TestCase("01", -64)]
		[TestCase("C0004000", 8192)]
		[TestCase("8001", -8192)]
		[TestCase("DFFFFFFE", 268435455)]
		[TestCase("C0000001", -268435456)]
		public void DecompressSigned(string hex, int expectedValue)
		{
			var blob = BlobFromHex(hex);
			var index = 0;
			var val = Decompressor.ReadCompressedInt(blob, ref index);

			Assert.That(index, Is.EqualTo(blob.Length));
			Assert.That(val, Is.EqualTo(expectedValue));
		}

		[TestCase("49", 0x01000012u)]
		[TestCase("14", 0x02000005u)]
		[TestCase("1E", 0x1B000007u)]
		public void ReadTypeDefOrRefOrSpecEncoded(string hex, uint expectedValue)
		{
			var blob = BlobFromHex(hex);
			var index = 0;
			var val = Decompressor.ReadTypeDefOrRefOrSpecEncoded(blob, ref index);

			Assert.That(index, Is.EqualTo(blob.Length));
			Assert.That(val, Is.EqualTo(new MetaDataToken(expectedValue)));
		}

		#region Implementation

		static byte[] BlobFromHex(string hex)
		{
			var result = new byte[hex.Length >> 1];

			for (var i = 0; i < result.Length; i++)
			{
				var n = i << 1;
				result[i] = (byte)((Hex(hex[n]) << 4) | Hex(hex[n + 1]));
			}

			return result;
		}

		static byte Hex(char c)
		{
			if (c >= '0' && c <= '9')
			{
				return (byte)(c - '0');
			}
			else if (c >= 'A' && c <= 'F')
			{
				return (byte)(c - 'A' + 10);
			}
			else if (c >= 'a' && c <= 'f')
			{
				return (byte)(c - 'a' + 10);
			}
			else
			{
				throw new ArgumentOutOfRangeException(nameof(c));
			}
		}

		#endregion
	}
}
