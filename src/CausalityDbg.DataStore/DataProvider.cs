// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CausalityDbg.DataStore
{
	public sealed class DataProvider : IDataProvider
	{
		public DataProvider()
		{
			var freq = Stopwatch.Frequency >> 2;
			_mappings = new TimelineMappings(freq, freq >> 3);
			_bands = new List<Band>();
			_closedBands = new List<Band>();
			_scopes = new List<Scope>();
			_events = new List<Event>();
			_bandCoolDown = freq >> 3;
		}

		public int CountBands => _bands.Count;
		public int CountScopes => _scopes.Count;
		public int CountEvents => _events.Count;
		public IEnumerable<Band> Bands => _bands;

		public IEnumerable<Scope> FindScopes(long lowerBound, long upperBound)
		{
			foreach (var x in _scopes)
			{
				if (x.FromTimestamp > upperBound) continue;
				if (x.ToTimestamp.HasValue && x.ToTimestamp.Value < lowerBound) continue;
				yield return x;
			}
		}

		public IEnumerable<Event> FindEvents(long lowerBound, long upperBound)
		{
			foreach (var x in _events)
			{
				if (x.Timestamp < lowerBound) continue;
				if (x.Timestamp > upperBound) continue;
				yield return x;
			}
		}

		public IEnumerable<TimelineSection> FindSections(long lowerBound, long upperBound)
			=> _mappings.GetSections(lowerBound, upperBound);

		public long UpperBound => _mappings.UpperViewBound;

		public void NewScope(int scopeId, int? hostScopeId, int? triggerId, long fromTimestamp, DataItem item)
		{
			Scope scope;

			if (scopeId != _scopes.Count) throw new InvalidOperationException("scope id's have fallen out of sync.");

			var trigger = triggerId == null ? null : _events[triggerId.Value];

			if (hostScopeId.HasValue)
			{
				scope = new Scope(_scopes[hostScopeId.Value], trigger, item, scopeId, _mappings.Mark(fromTimestamp));
			}
			else
			{
				scope = new Scope(GetAvailableBand(fromTimestamp), trigger, item, scopeId, _mappings.Mark(fromTimestamp));
			}

			if (scope.Depth > scope.Band.MaxDepth)
			{
				scope.Band.MaxDepth = scope.Depth;
			}

			_scopes.Add(scope);

			OnDataChanged();
		}

		public void NewEvent(int eventId, int hostScopeId, long timestamp, DataItem item)
		{
			if (eventId != _events.Count) throw new InvalidOperationException("Event id's have fallen out of sync.");

			_events.Add(new Event(_scopes[hostScopeId], item, _mappings.Mark(timestamp)));
			OnDataChanged();
		}

		public void CloseScope(int scopeId, long toTimestamp)
		{
			var scope = _scopes[scopeId];
			scope.Close(_mappings.Mark(toTimestamp));

			if (scope.Host == null)
			{
				scope.Band.Available = toTimestamp + _bandCoolDown;
				_closedBands.Add(scope.Band);
			}

			OnDataChanged();
		}

		public event EventHandler DataChanged;

		void OnDataChanged() => DataChanged?.Invoke(this, EventArgs.Empty);

		Band GetAvailableBand(long start)
		{
			Band result = null;

			foreach (var band in _closedBands)
			{
				if (band.Available < start && (result == null || band.ID < result.ID))
				{
					result = band;
				}
			}

			if (result == null)
			{
				_bands.Add(result = new Band(_bands.Count));
			}
			else
			{
				_closedBands.Remove(result);
			}

			result.Available = long.MaxValue;
			return result;
		}

		readonly TimelineMappings _mappings;
		readonly List<Band> _bands;
		readonly List<Band> _closedBands;
		readonly List<Scope> _scopes;
		readonly List<Event> _events;
		readonly long _bandCoolDown;
	}
}
