// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Diagnostics;
using System.IO;
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

		string Format(SigMethod method)
		{
			using (var writer = new StringWriter())
			using (var xmlWriter = new XmlTextWriter(writer))
			{
				xmlWriter.Formatting = Formatting.Indented;
				xmlWriter.IndentChar = ' ';
				xmlWriter.Indentation = 2;

				new SigFormatter(xmlWriter).Visit(method);
				xmlWriter.Flush();

				return writer.ToString();
			}
		}

		#endregion
	}
}
