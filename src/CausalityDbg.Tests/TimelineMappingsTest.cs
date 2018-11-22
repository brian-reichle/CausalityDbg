// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Linq;
using CausalityDbg.Main;
using NUnit.Framework;

namespace CausalityDbg.Tests
{
	[TestFixture]
	class TimelineMappingsTest
	{
		[Test]
		public void Empty()
		{
			var mappings = new TimelineMappings(1000, 100);
			var sections = mappings.GetSections(0, 2000).ToArray();
			Assert.That(sections.Length, Is.EqualTo(0));
		}

		[Test]
		public void FirstMark()
		{
			var mappings = new TimelineMappings(1000, 100);

			Assert.That(mappings.Mark(15000), Is.EqualTo(0));
			Assert.That(mappings.UpperViewBound, Is.EqualTo(0));

			var sections = mappings.GetSections(0, 2000).ToArray();
			Assert.That(sections.Length, Is.EqualTo(1));

			Assert.That(sections[0].RealStart, Is.EqualTo(15000));
			Assert.That(sections[0].RealEnd, Is.EqualTo(15000));
			Assert.That(sections[0].ViewStart, Is.EqualTo(0));
			Assert.That(sections[0].ViewEnd, Is.EqualTo(0));
			Assert.That(sections[0].Duration, Is.EqualTo(1));
		}

		[Test]
		public void MergeMark()
		{
			var mappings = new TimelineMappings(1000, 100);
			Assert.That(mappings.Mark(15000), Is.EqualTo(0));
			Assert.That(mappings.Mark(15999), Is.EqualTo(999));
			Assert.That(mappings.UpperViewBound, Is.EqualTo(999));

			var sections = mappings.GetSections(0, 2000).ToArray();
			Assert.That(sections.Length, Is.EqualTo(1));
			Assert.That(sections[0].RealStart, Is.EqualTo(15000));
			Assert.That(sections[0].RealEnd, Is.EqualTo(15999));
			Assert.That(sections[0].ViewStart, Is.EqualTo(0));
			Assert.That(sections[0].ViewEnd, Is.EqualTo(999));
			Assert.That(sections[0].Duration, Is.EqualTo(1000));
		}

		[Test]
		public void SplitMark()
		{
			var mappings = new TimelineMappings(1000, 100);
			Assert.That(mappings.Mark(15000), Is.EqualTo(0));
			Assert.That(mappings.Mark(16000), Is.EqualTo(199));
			Assert.That(mappings.UpperViewBound, Is.EqualTo(199));

			var sections = mappings.GetSections(0, 2000).ToArray();
			Assert.That(sections.Length, Is.EqualTo(2));

			Assert.That(sections[0].RealStart, Is.EqualTo(15000));
			Assert.That(sections[0].RealEnd, Is.EqualTo(15099));
			Assert.That(sections[0].ViewStart, Is.EqualTo(0));
			Assert.That(sections[0].ViewEnd, Is.EqualTo(99));
			Assert.That(sections[0].Duration, Is.EqualTo(100));

			Assert.That(sections[1].RealStart, Is.EqualTo(15901));
			Assert.That(sections[1].RealEnd, Is.EqualTo(16000));
			Assert.That(sections[1].ViewStart, Is.EqualTo(100));
			Assert.That(sections[1].ViewEnd, Is.EqualTo(199));
			Assert.That(sections[1].Duration, Is.EqualTo(100));
		}

		[Test]
		public void GetSections()
		{
			/*
			 *       [00] - 09
			 *  10 - [19] - 28
			 *  29 - [38] - 47
			 *  48 - [57] - 66
			 *  67 - [76]
			 */
			var mappings = new TimelineMappings(100, 10);
			Assert.That(mappings.Mark(1000), Is.EqualTo(0));
			Assert.That(mappings.Mark(2000), Is.EqualTo(19));
			Assert.That(mappings.Mark(3000), Is.EqualTo(38));
			Assert.That(mappings.Mark(4000), Is.EqualTo(57));
			Assert.That(mappings.Mark(5000), Is.EqualTo(76));
			Assert.That(mappings.UpperViewBound, Is.EqualTo(76));

			var sections = mappings.GetSections(25, 50).ToArray();
			Assert.That(sections.Length, Is.EqualTo(3));

			Assert.That(sections[0].ViewStart, Is.EqualTo(10));
			Assert.That(sections[0].ViewEnd, Is.EqualTo(28));

			Assert.That(sections[1].ViewStart, Is.EqualTo(29));
			Assert.That(sections[1].ViewEnd, Is.EqualTo(47));

			Assert.That(sections[2].ViewStart, Is.EqualTo(48));
			Assert.That(sections[2].ViewEnd, Is.EqualTo(66));
		}
	}
}
