// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using CausalityDbg.IL;
using NUnit.Framework;

namespace CausalityDbg.Tests
{
	class SignatureReaderTest
	{
		[TestCaseSource(nameof(MethodSignatures))]
		public void CheckSig(string name)
		{
			var tuple = TestHelper.ReadBinaryTextPair(name);
			var text = Format(SignatureReader.ReadMethodDefSig(tuple.Key));

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

		static TestCaseData[] MethodSignatures()
		{
			const string prefix = "CausalityDbg.Tests.TestFiles.Signature.";
			const string suffix = ".txt";
			return TestHelper.GetResourceBasedTests(prefix, suffix);
		}

		static string Format(SigMethod method)
		{
			var builder = new StringBuilder();

			using (var xmlWriter = new XmlTextWriter(new StringWriter(builder)))
			{
				xmlWriter.Formatting = Formatting.Indented;
				xmlWriter.IndentChar = ' ';
				xmlWriter.Indentation = 2;

				SigFormatter.Instance.Visit(method, xmlWriter);
				xmlWriter.Flush();
			}

			builder.AppendLine();
			return builder.ToString();
		}

		#endregion
	}
}
