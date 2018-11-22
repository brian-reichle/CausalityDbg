// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CausalityDbg.Main
{
	sealed class TimelineMappings
	{
		public TimelineMappings(long threashold, long lip)
		{
			_sections = new List<TimelineSection>();
			_lip = lip;
			_threashold = Math.Max(threashold, lip << 1);
		}

		public long UpperViewBound { get; private set; }

		public IEnumerable<TimelineSection> GetSections(long viewStart, long viewEnd)
		{
			foreach (var section in _sections)
			{
				if (section.ViewStart > viewEnd) break;
				if (section.ViewEnd < viewStart) continue;
				yield return section;
			}
		}

		public long Mark(long realTimestamp)
		{
			if (_sections.Count == 0)
			{
				_sections.Add(new TimelineSection(realTimestamp, 0, 1));
				return 0;
			}

			var lastSection = _sections[_sections.Count - 1];
			var tmp = realTimestamp - lastSection.RealStart;

			if (tmp < 0)
			{
				throw new InvalidOperationException("timestamp was backdated before last section");
			}

			var adjustment = tmp - lastSection.Duration + 1;

			if (adjustment < _threashold)
			{
				if (adjustment > 0)
				{
					lastSection.Duration = tmp + 1;
					UpperViewBound = lastSection.ViewEnd;
				}

				return lastSection.ViewStart + tmp;
			}

			lastSection.Duration += _lip - 1;

			lastSection = new TimelineSection(
				realTimestamp - _lip + 1,
				lastSection.ViewEnd + 1,
				_lip);

			_sections.Add(lastSection);

			return UpperViewBound = lastSection.ViewEnd;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly long _threashold;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly long _lip;
		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		readonly List<TimelineSection> _sections;
	}
}
