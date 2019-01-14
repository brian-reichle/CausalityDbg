// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Text;
using CausalityDbg.Configuration;
using CausalityDbg.DataStore;
using CausalityDbg.Main;
using NUnit.Framework;

namespace CausalityDbg.Tests
{
	[TestFixture]
	class TraceWrapperTest
	{
		[Test]
		public void Populate1()
		{
			const string expected =
@"0: ex1-3 [Exception]
1: ex1-2
2: ex1-1
3: fs2-3! [Task]
4: fs2-2
5: fs2-1
";

			var actual = FormatTrace(new TraceWrapper(_exceptionEvent1));

			Assert.That(actual, Is.EqualTo(expected));
		}

		[Test]
		public void Populate2()
		{
			const string expected =
@"0: ca-3! [Exception]
1: ca-2
2: ca-1
3: fs2-3! [Task]
4: fs2-2
5: fs2-1
";

			var actual = FormatTrace(new TraceWrapper(_exceptionEvent2));

			Assert.That(actual, Is.EqualTo(expected));
		}

		#region Implementation

		public TraceWrapperTest()
		{
			_band = new Band(0);

			_managedThreadCategory = new ConfigCategory(ConfigCategoryType.ManagedThread, 1, 1);
			_exceptionCategory = new ConfigCategory(ConfigCategoryType.Exception, 2, 2);
			_catchCategory = new ConfigCategory(ConfigCategoryType.Catch, 3, 3);
			_taskCategory = new ConfigCategory("Task", "Task", 4, 4);

			_managedThreadScope = new Scope(_band, null, new DataItem(_managedThreadCategory, TraceData.Empty), 0, 0);

			_taskEvent1 = NewEvent(_managedThreadScope, _taskCategory, "fe1", 3);
			_taskScope1 = NewScope(_managedThreadScope, _taskEvent1, _taskCategory, "fs1", 3);
			_taskEvent2 = NewEvent(_taskScope1, _taskCategory, "fe2", 3);
			_taskScope2 = NewScope(_managedThreadScope, _taskEvent2, _taskCategory, "fs2", 3);
			_exceptionEvent1 = NewEvent(_taskScope2, _exceptionCategory, "ex1", 3);
			_catchScope = NewScope(_taskScope2, _exceptionEvent1, _catchCategory, "ca", 3);
			_exceptionEvent2 = NewEvent(_catchScope, _exceptionCategory, "ex2", 0);
		}

		static string FormatTrace(TraceWrapper trace)
		{
			var builder = new StringBuilder();

			var indexWidth = (trace.Count - 1).ToString(CultureInfo.InvariantCulture).Length;

			foreach (var frame in trace)
			{
				var index = frame.Index.ToString(CultureInfo.InvariantCulture);

				builder.Append(index);
				builder.Append(' ', indexWidth - index.Length);
				builder.Append(": ");
				builder.Append(FrameText(frame.Frame));

				if (frame.Category != null)
				{
					builder.Append(" [");
					builder.Append(frame.Category.Name);
					builder.Append(']');
				}

				if (frame.Trace != trace)
				{
					builder.Append(" <-- wrong trace");
				}

				builder.AppendLine();
			}

			return builder.ToString();
		}

		static string FormatTrace(TraceData trace)
		{
			var builder = new StringBuilder();
			var indexWidth = (trace.TotalDepth - 1).ToString(CultureInfo.InvariantCulture).Length;
			var colStarts = GetColumnStarts(trace);

			for (var r = trace.TotalDepth; r > 0; r--)
			{
				var sol = builder.Length;
				builder.Append(trace.TotalDepth - r);
				builder.Append(' ', Math.Max(0, sol + indexWidth - builder.Length));
				builder.Append(": ");

				sol = builder.Length;

				for (var t = trace; t != null && t.TotalDepth >= r; t = t.ContainingTrace)
				{
					var i = t.TotalDepth - r;
					builder.Append(' ', Math.Max(0, sol + colStarts[t] - builder.Length));
					builder.Append(i >= t.Frames.Length ? "." : FrameText(t.Frames[i]));
				}

				builder.AppendLine();
			}

			return builder.ToString();
		}

		static Dictionary<TraceData, int> GetColumnStarts(TraceData trace)
		{
			var starts = new Dictionary<TraceData, int>();
			var pos = 0;

			for (var t = trace; t != null; t = t.ContainingTrace)
			{
				starts.Add(t, pos);

				var width = 1;

				for (var i = 0; i < t.Frames.Length; i++)
				{
					var len = FrameText(t.Frames[i]).Length;

					if (len > width)
					{
						width = len;
					}
				}

				pos += width + 1;
			}

			return starts;
		}

		static string FrameText(FrameData frame)
		{
			var iFrame = (FrameInternalData)frame;
			return iFrame == null ? "<null>" : iFrame.Text;
		}

		static Event NewEvent(Scope host, ConfigCategory category, string label, int count)
			=> new Event(host, NewDataItem(host.Item, category, label, count), 0);

		static Scope NewScope(Scope host, Event trigger, ConfigCategory category, string label, int count)
			=> new Scope(host, trigger, NewDataItem(host.Item, category, label, count), 0, 0);

		static DataItem NewDataItem(DataItem baseDataItem, ConfigCategory category, string label, int count)
			=> new DataItem(category, NewTrace(baseDataItem?.StackTrace, label, count));

		static TraceData NewTrace(TraceData baseTrace, string label, int count)
		{
			ImmutableArray<FrameData>.Builder frames;
			long topAddress;

			if (baseTrace == null || baseTrace.Frames.Length == 0)
			{
				baseTrace = TraceData.Empty;
				frames = ImmutableArray.CreateBuilder<FrameData>(count);
				frames.Count = count;
				topAddress = frames.Count;
			}
			else
			{
				var text = FrameText(baseTrace.Frames[0]) + "!";

				frames = ImmutableArray.CreateBuilder<FrameData>(count + 1);
				frames.Count = count + 1;
				frames[count] = new FrameInternalData(text);
				topAddress = baseTrace.TopAddress + frames.Count;
			}

			for (var i = 0; i < count; i++)
			{
				var text = label + "-" + (count - i);
				frames[i] = new FrameInternalData(text);
			}

			return new TraceData(topAddress, baseTrace, frames.MoveToImmutable());
		}

		readonly Band _band;

		readonly ConfigCategory _exceptionCategory;
		readonly ConfigCategory _managedThreadCategory;
		readonly ConfigCategory _catchCategory;
		readonly ConfigCategory _taskCategory;

		readonly Scope _managedThreadScope;
		readonly Scope _taskScope1;
		readonly Scope _taskScope2;
		readonly Scope _catchScope;

		readonly Event _taskEvent1;
		readonly Event _taskEvent2;
		readonly Event _exceptionEvent1;
		readonly Event _exceptionEvent2;

		#endregion
	}
}
