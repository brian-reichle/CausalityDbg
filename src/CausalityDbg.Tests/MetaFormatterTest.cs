// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using CausalityDbg.Core.MetaCache;
using NUnit.Framework;

namespace CausalityDbg.Tests
{
	[TestFixture]
	class MetaFormatterTest
	{
		[TestCase(0, ExpectedResult = "Function()")]
		[TestCase(1, ExpectedResult = "Function<Dummy.Type1>()")]
		[TestCase(2, ExpectedResult = "Function<Dummy.Type1, Dummy.Type2>()")]
		public string GlobalFunction(int genericArgs)
		{
			var frame = _module.NewFunction("Function", genericArgs).ToFrame(GetGenericArgs(genericArgs));
			return MetaFormatter.Format(frame);
		}

		[Test]
		public void Function()
		{
			var frame = _type1.NewFunction("Function").ToFrame();
			Assert.That(MetaFormatter.Format(frame), Is.EqualTo("Dummy.Type1.Function()"));
		}

		[Test]
		public void GenericFunction()
		{
			var frame = _gType2.NewFunction("Function", 1).ToFrame(GetGenericArgs(3));
			Assert.That(MetaFormatter.Format(frame), Is.EqualTo("Dummy.Type<Dummy.Type1, Dummy.Type2>.Function<Dummy.Type3>()"));
		}

		[Test]
		public void NestedType()
		{
			var frame = _type1.NewType("Nested").NewFunction("Function").ToFrame();
			Assert.That(MetaFormatter.Format(frame), Is.EqualTo("Dummy.Type1.Nested.Function()"));
		}

		[Test]
		public void GenericNestedType()
		{
			var frame = _gType1.NewType("Nested1").NewType("Nested2", 1).NewFunction("Function").ToFrame(GetGenericArgs(2));
			Assert.That(MetaFormatter.Format(frame), Is.EqualTo("Dummy.Type<Dummy.Type1>.Nested1.Nested2<Dummy.Type2>.Function()"));
		}

		[Test]
		public void NestedGenerics()
		{
			var frame = _gType1.NewFunction("Function").ToFrame(_gType1.Init(GetGenericArgs(1)));
			Assert.That(MetaFormatter.Format(frame), Is.EqualTo("Dummy.Type<Dummy.Type<Dummy.Type1>>.Function()"));
		}

		[Test]
		public void SingleParameter()
		{
			var frame = _type1.NewFunction("Function", _type2.Init().ToParam("arg")).ToFrame();
			Assert.That(MetaFormatter.Format(frame), Is.EqualTo("Dummy.Type1.Function(Dummy.Type2 arg)"));
		}

		[Test]
		public void MultipleParameters()
		{
			var frame = _type1.NewFunction("Function", _type2.Init().ToParam("arg1"), _type3.Init().ToParam("arg2")).ToFrame();
			Assert.That(MetaFormatter.Format(frame), Is.EqualTo("Dummy.Type1.Function(Dummy.Type2 arg1, Dummy.Type3 arg2)"));
		}

		[Test]
		public void UnnamedParameter()
		{
			var frame = _type1.NewFunction("Function", _type2.Init().ToParam()).ToFrame();
			Assert.That(MetaFormatter.Format(frame), Is.EqualTo("Dummy.Type1.Function(Dummy.Type2)"));
		}

		[Test]
		public void ByRef()
		{
			var type = _type1.Init().ByRef();
			Assert.That(MetaFormatter.Format(type), Is.EqualTo("Dummy.Type1&"));
		}

		[Test]
		public void Pointer()
		{
			var type = _type1.Init().Ptr();
			Assert.That(MetaFormatter.Format(type), Is.EqualTo("Dummy.Type1*"));
		}

		[TestCase(false, 0, ExpectedResult = "Dummy.Type<Dummy.Type1, Dummy.Type2>.Function<Dummy.Type3, Dummy.Type4>(Dummy.Type1 arg)")]
		[TestCase(false, 1, ExpectedResult = "Dummy.Type<Dummy.Type1, Dummy.Type2>.Function<Dummy.Type3, Dummy.Type4>(Dummy.Type2 arg)")]
		[TestCase(true, 0, ExpectedResult = "Dummy.Type<Dummy.Type1, Dummy.Type2>.Function<Dummy.Type3, Dummy.Type4>(Dummy.Type3 arg)")]
		[TestCase(true, 1, ExpectedResult = "Dummy.Type<Dummy.Type1, Dummy.Type2>.Function<Dummy.Type3, Dummy.Type4>(Dummy.Type4 arg)")]
		public string TypeGenArgRef(bool method, int index)
		{
			var param = new MetaCompoundGenArg(method, index).ToParam("arg");
			var frame = _gType2.NewFunction("Function", 2, param).ToFrame(GetGenericArgs(4));
			return MetaFormatter.Format(frame);
		}

		[TestCase(1, ExpectedResult = "Dummy.Type1[]")]
		[TestCase(2, ExpectedResult = "Dummy.Type1[,]")]
		[TestCase(3, ExpectedResult = "Dummy.Type1[,,]")]
		public string Array(int rank)
		{
			var type = _type1.Init().ToArray(rank);
			return MetaFormatter.Format(type);
		}

		#region Implementation

		public MetaFormatterTest()
		{
			_module = new MetaModule("Dummy1.dll", MetaModuleFlags.None);

			_type1 = _module.NewType("Dummy.Type1");
			_type2 = _module.NewType("Dummy.Type2");
			_type3 = _module.NewType("Dummy.Type3");
			_type4 = _module.NewType("Dummy.Type4");

			_gType1 = _module.NewType("Dummy.Type`1", 1);
			_gType2 = _module.NewType("Dummy.Type`2", 2);
		}

		MetaCompound[] GetGenericArgs(int count)
		{
			if (count == 0)
			{
				return null;
			}

			var result = new MetaCompound[count];

			switch (count)
			{
				case 4:
					result[3] = _type4.Init();
					goto case 3;

				case 3:
					result[2] = _type3.Init();
					goto case 2;

				case 2:
					result[1] = _type2.Init();
					goto case 1;

				case 1:
					result[0] = _type1.Init();
					return result;

				default: throw new ArgumentException();
			}
		}

		readonly MetaModule _module;
		readonly MetaType _type1;
		readonly MetaType _type2;
		readonly MetaType _type3;
		readonly MetaType _type4;
		readonly MetaType _gType1;
		readonly MetaType _gType2;

		#endregion
	}
}
