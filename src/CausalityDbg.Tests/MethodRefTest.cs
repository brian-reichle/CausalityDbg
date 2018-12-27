// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Linq;
using CausalityDbg.Configuration;
using CausalityDbg.Core;
using NUnit.Framework;

namespace CausalityDbg.Tests
{
	[TestFixture]
	class MethodRefTest
	{
		[TestCase("NameOnly", "NameOnly", null)]
		[TestCase("NoParams()", "NoParams", new string[] { })]
		[TestCase("SingleParam(Namespace.Class.Param0)", "SingleParam", new[] { "Namespace.Class.Param0" })]
		[TestCase("MultiParam(Namespace.Class.Param0, Namespace.Class.Param1)", "MultiParam", new[] { "Namespace.Class.Param0", "Namespace.Class.Param1" })]
		[TestCase("Nested(Namespace.Parent::Child)", "Nested", new[] { "Namespace.Parent::Child" })]
		public void Parse(string text, string name, string[] args)
		{
			var method = MethodRef.Parse(text);

			Assert.That(method.Text, Is.EqualTo(text));
			Assert.That(method.Name, Is.EqualTo(name));
			Assert.That(method.SpecifiesArgTypes, Is.EqualTo(args != null));
			Assert.That(method.ArgTypes.Select(x => x.Name), Is.EqualTo(args ?? System.Array.Empty<string>()));
			Assert.That(method.ArgTypes.Select(x => x.ByRef), Has.All.EqualTo(false));
		}

		public void ParseByRef()
		{
			var method = MethodRef.Parse("Method(Arg1, Arg2&, Arg3)");

			Assert.That(method.ArgTypes.Select(x => x.Name), Is.EqualTo(new[] { "Arg1", "Arg2", "Arg3" }));
			Assert.That(method.ArgTypes.Select(x => x.ByRef), Is.EqualTo(new[] { false, true, false }));
		}
	}
}
