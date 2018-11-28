// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.IO;
using System.Text;
using System.Xml;
using CausalityDbg.IL;
using NUnit.Framework;

namespace CausalityDbg.Tests
{
	[TestFixture(true)]
	[TestFixture(false)]
	class CorExceptionClauseCollectionTest
	{
		public CorExceptionClauseCollectionTest(bool isFat)
		{
			_blob = isFat ? ConstructFatCollection() : ConstructShortCollection();
			_collection = CorExceptionClauseCollection.New(_blob);
		}

		[TestCase(0, ExpectedResult = null)]
		[TestCase(1, ExpectedResult = 1)]
		[TestCase(2, ExpectedResult = null)]
		[TestCase(3, ExpectedResult = null)]
		[TestCase(4, ExpectedResult = 4)]
		[TestCase(5, ExpectedResult = 5)]
		[TestCase(6, ExpectedResult = 6)]
		[TestCase(7, ExpectedResult = 5)]
		[TestCase(8, ExpectedResult = 5)]
		[TestCase(9, ExpectedResult = 9)]
		[TestCase(10, ExpectedResult = 10)]
		[TestCase(11, ExpectedResult = 11)]
		public int? FromHandler(int offset)
		{
			var clause = _collection.FromHandlerOffset(offset);
			return clause == null ? null : (int?)clause.HandlerOffset;
		}

		[TestCase(0, ExpectedResult = null)]
		[TestCase(1, ExpectedResult = null)]
		[TestCase(2, ExpectedResult = null)]
		[TestCase(3, ExpectedResult = null)]
		[TestCase(4, ExpectedResult = null)]
		[TestCase(5, ExpectedResult = null)]
		[TestCase(6, ExpectedResult = null)]
		[TestCase(7, ExpectedResult = null)]
		[TestCase(8, ExpectedResult = 8)]
		[TestCase(9, ExpectedResult = null)]
		[TestCase(10, ExpectedResult = null)]
		[TestCase(11, ExpectedResult = null)]
		public int? FromFilter(int offset)
		{
			var clause = _collection.FromFilterOffset(offset);
			return clause == null ? null : (int?)clause.FilterOffset;
		}

		[Test]
		public void Count()
		{
			Assert.That(_collection.Count, Is.EqualTo(7));
		}

		[Test]
		public void BlobConsistency()
		{
			var isFat = (_blob[0] & (byte)CorILMethodSectFlags.CorILMethod_Sect_FatFormat) != 0;
			var isEHTable = (_blob[0] & (byte)CorILMethodSectFlags.CorILMethod_Sect_EHTable) != 0;
			var len = isFat ? BitConverter.ToInt32(_blob, 0) >> 8 : _blob[1];

			Assert.That(len, Is.EqualTo(_blob.Length));
			Assert.That(isEHTable, Is.True);
		}

		#region Implementation

		static string Format(CorExceptionClauseCollection clauses)
		{
			var builder = new StringBuilder();

			using (var xmlWriter = new XmlTextWriter(new StringWriter(builder)))
			{
				xmlWriter.Formatting = Formatting.Indented;
				xmlWriter.IndentChar = ' ';
				xmlWriter.Indentation = 2;

				xmlWriter.WriteStartElement("Clauses");

				foreach (var clause in clauses)
				{
					xmlWriter.WriteStartElement(clause.Flags.ToString());

					xmlWriter.WriteStartElement("Try");
					xmlWriter.WriteAttributeString("offset", clause.TryOffset.ToString());
					xmlWriter.WriteAttributeString("length", clause.TryLength.ToString());
					xmlWriter.WriteEndElement();

					xmlWriter.WriteStartElement("Handler");
					xmlWriter.WriteAttributeString("offset", clause.HandlerOffset.ToString());
					xmlWriter.WriteAttributeString("length", clause.HandlerLength.ToString());
					xmlWriter.WriteEndElement();

					if (!clause.ClassToken.IsNil)
					{
						xmlWriter.WriteStartElement("Exception");
						xmlWriter.WriteAttributeString("token", clause.ClassToken.ToString());
						xmlWriter.WriteEndElement();
					}

					if (clause.FilterOffset != 0)
					{
						xmlWriter.WriteStartElement("Filter");
						xmlWriter.WriteAttributeString("offset", clause.FilterOffset.ToString());
						xmlWriter.WriteEndElement();
					}

					xmlWriter.WriteEndElement();
				}

				xmlWriter.WriteEndElement();
				xmlWriter.Flush();
			}

			builder.AppendLine();
			return builder.ToString();
		}

		readonly byte[] _blob;
		readonly CorExceptionClauseCollection _collection;

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
		static byte[] ConstructShortCollection()
		{
			return new byte[]
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
		}

		static byte[] ConstructFatCollection()
		{
			return new byte[]
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
		}

		#endregion
	}
}
