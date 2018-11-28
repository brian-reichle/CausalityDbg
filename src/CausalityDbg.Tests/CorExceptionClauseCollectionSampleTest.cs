// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using CausalityDbg.IL;
using NUnit.Framework;

namespace CausalityDbg.Tests
{
	[TestFixture]
	class CorExceptionClauseCollectionSampleTest
	{
		[TestCaseSource(nameof(DataSections))]
		public void CheckClause(string name)
		{
			var tuple = TestHelper.ReadBinaryTextPair(name);
			var text = Format(CorExceptionClauseCollection.New(tuple.Key));

			try
			{
				Assert.That(text, Is.EqualTo(tuple.Value));
			}
			catch
			{
				Trace.Write(text);
				throw;
			}
		}

		#region Implementation

		static TestCaseData[] DataSections()
		{
			const string prefix = "CausalityDbg.Tests.TestFiles.DataSections.";
			const string suffix = ".txt";
			return TestHelper.GetResourceBasedTests(prefix, suffix);
		}

		string Format(CorExceptionClauseCollection clauses)
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

		#endregion
	}
}
