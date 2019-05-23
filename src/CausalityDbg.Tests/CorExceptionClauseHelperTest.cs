// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using CausalityDbg.IL;
using NUnit.Framework;

namespace CausalityDbg.Tests
{
	[TestFixture(true)]
	[TestFixture(false)]
	public class CorExceptionClauseHelperTest
	{
		[Test]
		public void IsExceptionData()
		{
			Assert.That(CorExceptionClauseHelper.IsExceptionData(Blob()), Is.True);
		}

		[TestCase(0, ExpectedResult = null)]
		[TestCase(1, ExpectedResult = "1-2:Clause")]
		[TestCase(2, ExpectedResult = null)]
		[TestCase(3, ExpectedResult = null)]
		[TestCase(4, ExpectedResult = "4-5:Clause")]
		[TestCase(5, ExpectedResult = "5-10:Clause")]
		[TestCase(6, ExpectedResult = "6-7:Fault")]
		[TestCase(7, ExpectedResult = "5-10:Clause")]
		[TestCase(8, ExpectedResult = "5-10:Clause")]
		[TestCase(9, ExpectedResult = "9-10:Filter")]
		[TestCase(10, ExpectedResult = "10-11:Finally")]
		[TestCase(11, ExpectedResult = "11-12:Finally")]
		public string FromHandler(int offset)
		{
			if (CorExceptionClauseHelper.HandlerFromOffset(Blob(), offset, out var start, out var end, out var flags))
			{
				return start + "-" + end + ":" + flags;
			}

			return null;
		}

		[TestCase(0, ExpectedResult = null)]
		[TestCase(1, ExpectedResult = null)]
		[TestCase(2, ExpectedResult = null)]
		[TestCase(3, ExpectedResult = null)]
		[TestCase(4, ExpectedResult = null)]
		[TestCase(5, ExpectedResult = null)]
		[TestCase(6, ExpectedResult = null)]
		[TestCase(7, ExpectedResult = null)]
		[TestCase(8, ExpectedResult = "8-9")]
		[TestCase(9, ExpectedResult = null)]
		[TestCase(10, ExpectedResult = null)]
		[TestCase(11, ExpectedResult = null)]
		public string FromFilter(int offset)
		{
			if (CorExceptionClauseHelper.FilterFromOffset(Blob(), offset, out var start, out var end))
			{
				return start + "-" + end;
			}

			return null;
		}

		[Test]
		public void BlobConsistency()
		{
			var blob = Blob();
			var isFat = (blob[0] & (byte)CorILMethodSectFlags.CorILMethod_Sect_FatFormat) != 0;
			var isEHTable = (blob[0] & (byte)CorILMethodSectFlags.CorILMethod_Sect_EHTable) != 0;
			var len = isFat ? MemoryMarshal.Read<int>(blob) >> 8 : blob[1];

			Assert.That(len, Is.EqualTo(blob.Length));
			Assert.That(isEHTable, Is.True);
		}

		#region Implementation

		public CorExceptionClauseHelperTest(bool isFat)
		{
			_isFat = isFat;
		}

		ReadOnlySpan<byte> Blob() => _isFat ? ConstructFatCollection() : ConstructShortCollection();

		readonly bool _isFat;

		// try
		// {
		//     try
		//     {
		//         // 0
		//     }
		//     catch (ArgumentException)
		//     {
		//         // 1
		//     }
		//     // 2
		//     try
		//     {
		//         // 3
		//     }
		//     catch (InvalidOperationException)
		//     {
		//         // 4
		//     }
		//     catch (FormatException)
		//     {
		//         try
		//         {
		//             try
		//             {
		//                 // 5
		//             }
		//             fault
		//             {
		//                 // 6
		//             }
		//             // 7
		//         }
		//         filter
		//         {
		//             // 8
		//         }
		//         catch
		//         {
		//             // 9
		//         }
		//     }
		//     finally
		//     {
		//         // 10
		//     }
		// }
		// finally
		// {
		//     // 11
		// }

		// Clause [0] : 0 :  1 :  1 : 1 : 02000011 :
		// Clause [0] : 3 :  1 :  4 : 1 : 02000012 :
		// Fault  [4] : 5 :  1 :  6 : 1 :          :
		// Filter [1] : 5 :  3 :  9 : 1 : 8        :
		// Clause [0] : 3 :  1 :  5 : 5 : 02000013 :
		// Finally[2] : 3 :  7 : 10 : 1 :          :
		// Finally[2] : 0 : 11 : 11 : 1 :          :
		static ReadOnlySpan<byte> ConstructShortCollection() => new byte[]
		{
			/*         |--|  |--|  |--------| */
			/* 0x00 */ 0x01, 0x58, 0x00, 0x00,

			/*         |--------|  |--------|  |--|  |--------|  |--|  |--------------------| */
			/* 0x04 */ 0x00, 0x00, 0x00, 0x00, 0x01, 0x01, 0x00, 0x01, 0x11, 0x00, 0x00, 0x02,
			/* 0x10 */ 0x00, 0x00, 0x03, 0x00, 0x01, 0x04, 0x00, 0x01, 0x12, 0x00, 0x00, 0x02,
			/* 0x1C */ 0x04, 0x00, 0x05, 0x00, 0x01, 0x06, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00,
			/* 0x28 */ 0x01, 0x00, 0x05, 0x00, 0x03, 0x09, 0x00, 0x01, 0x08, 0x00, 0x00, 0x00,
			/* 0x34 */ 0x00, 0x00, 0x03, 0x00, 0x01, 0x05, 0x00, 0x05, 0x13, 0x00, 0x00, 0x02,
			/* 0x40 */ 0x02, 0x00, 0x03, 0x00, 0x07, 0x0a, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00,
			/* 0x4C */ 0x02, 0x00, 0x00, 0x00, 0x0b, 0x0b, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00,
			/* 0x58 */
		};

		static ReadOnlySpan<byte> ConstructFatCollection() => new byte[]
		{
			/*         |--|  |--------------| */
			/* 0x00 */ 0x41, 0xAC, 0x00, 0x00,

			/*         |--------------------|  |--------------------|  |--------------------|  |--------------------|  |--------------------|  |--------------------| */
			/* 0x04 */ 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x11, 0x00, 0x00, 0x02,
			/* 0x1C */ 0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x12, 0x00, 0x00, 0x02,
			/* 0x34 */ 0x04, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			/* 0x4C */ 0x01, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00,
			/* 0x64 */ 0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x13, 0x00, 0x00, 0x02,
			/* 0x7C */ 0x02, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x07, 0x00, 0x00, 0x00, 0x0a, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			/* 0x94 */ 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0b, 0x00, 0x00, 0x00, 0x0b, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			/* 0xAC */
		};

		#endregion
	}
}
