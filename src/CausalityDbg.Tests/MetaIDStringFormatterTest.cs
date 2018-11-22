// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using CausalityDbg.Core.MetaCache;
using NUnit.Framework;

namespace CausalityDbg.Tests
{
	[TestFixture]
	class MetaIDStringFormatterTest
	{
		[TestCaseSource(nameof(MethodFormatSource))]
		public string FormatMethod(MetaFunction function)
		{
			var formatter = new MetaIDStringFormatter();
			formatter.AppendFunction(function);
			return formatter.ToString();
		}

		protected static TestCaseData[] MethodFormatSource()
		{
			var module = new MetaModule("Dummy1.dll", MetaModuleFlags.None);

			var type1 = module.NewType("Dummy.Type1");
			var type2 = module.NewType("Dummy.Type2");
			var type3 = module.NewType("Dummy.Type3");
			var type4 = module.NewType("Dummy.Type4");

			var gType1 = module.NewType("Dummy.Type`1", 1);
			var gType2 = module.NewType("Dummy.Type`2", 2);

			return new[]
			{
				new TestCaseData(type1.NewFunction("Function"))
					.SetName("Basic")
					.Returns("M:Dummy.Type1.Function"),

				new TestCaseData(type1.NewFunction("Function", type2.Init().ToParam()))
					.SetName("SingleArg")
					.Returns("M:Dummy.Type1.Function(Dummy.Type2)"),

				new TestCaseData(type1.NewFunction("Function", type2.Init().ToParam(), type3.Init().ToParam()))
					.SetName("MultiArg")
					.Returns("M:Dummy.Type1.Function(Dummy.Type2,Dummy.Type3)"),

				new TestCaseData(type1.NewFunction("Function", type2.Init().Ptr().ToParam()))
					.SetName("Pointer")
					.Returns("M:Dummy.Type1.Function(Dummy.Type2*)"),

				new TestCaseData(type1.NewFunction("Function", type2.Init().ByRef().ToParam()))
					.SetName("ByRef")
					.Returns("M:Dummy.Type1.Function(Dummy.Type2@)"),

				new TestCaseData(type1.NewFunction("Function", type2.Init().ToArray(1).ToParam()))
					.SetName("Array")
					.Returns("M:Dummy.Type1.Function(Dummy.Type2[])"),

				new TestCaseData(type1.NewFunction("Function", type2.Init().ToArray(2).ToParam()))
					.SetName("MultiArray")
					.Returns("M:Dummy.Type1.Function(Dummy.Type2[0:,0:])"),

				new TestCaseData(gType2.NewFunction("Function", 1, new MetaCompoundGenArg(true, 0).ToParam(), new MetaCompoundGenArg(false, 1).ToParam()))
					.SetName("GenericFunction")
					.Returns("M:Dummy.Type`2.Function``1(``0,`1)"),

				new TestCaseData(type1.NewFunction("Function", type2.Init().ToArray(2).ToArray(1).ToParam()))
					.SetName("NestedArray")
					.Returns("M:Dummy.Type1.Function(Dummy.Type2[0:,0:][])"),

				new TestCaseData(type1.NewFunction("Function", gType1.NewType("Nested1").NewType("Nested2", 1).Init(type2.Init(), type3.Init()).ToParam()))
					.SetName("NestedGenerics")
					.Returns("M:Dummy.Type1.Function(Dummy.Type{Dummy.Type2}.Nested1.Nested2{Dummy.Type3})"),

				new TestCaseData(type1.NewFunction("Function", gType1.Init(type2.Init().ToArray(1)).ToArray(1).ToParam()))
					.SetName("AGA")
					.Returns("M:Dummy.Type1.Function(Dummy.Type{Dummy.Type2[]}[])"),
			};
		}
	}
}
