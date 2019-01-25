// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Linq;
using CausalityDbg.Main;
using NUnit.Framework;

namespace CausalityDbg.Tests
{
	[TestFixture]
	public class TemplateProcessorTest
	{
		[TestCase(null, ExpectedResult = null)]
		[TestCase("", ExpectedResult = "")]
		[TestCase("SuperMagic", ExpectedResult = "SuperMagic")]
		[TestCase("{0}", ExpectedResult = "Zero")]
		[TestCase("{{0}}", ExpectedResult = "{0}")]
		[TestCase("{{{0}}}", ExpectedResult = "{Zero}")]
		[TestCase("Left {1} Middle {2} Right", ExpectedResult = "Left One Middle Two Right")]
		[TestCase("[{3}]", ExpectedResult = "[]")]
		public string Format(string template)
		{
			return TemplateProcessor.ProcessTemplate(template, (x) =>
			{
				switch (x)
				{
					case "0": return "Zero";
					case "1": return "One";
					case "2": return "Two";
					default: return null;
				}
			});
		}

		[TestCase("", ExpectedResult = new string[] { })]
		[TestCase("SuperMagic", ExpectedResult = new string[] { })]
		[TestCase("{0}", ExpectedResult = new string[] { "0" })]
		[TestCase("{{0}}", ExpectedResult = new string[] { })]
		[TestCase("{{{0}}}", ExpectedResult = new string[] { "0" })]
		[TestCase("Left {1} Middle {2} Right", ExpectedResult = new string[] { "1", "2" })]
		[TestCase("[{3}]", ExpectedResult = new string[] { "3" })]
		public string[] GetMacros(string template)
		{
			return TemplateProcessor.GetMacros(template).ToArray();
		}

		[Test]
		public void GetNullMacro()
		{
			Assert.That(() => TemplateProcessor.GetMacros(null).ToArray(), Throws.ArgumentNullException);
		}
	}
}
