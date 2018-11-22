// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Generic;
using CausalityDbg.Core.CorDebugApi;

namespace CausalityDbg.Core
{
	sealed class EventMapper
	{
		public EventMapper()
		{
			_lookup = new Dictionary<ConfigCategory, ValueDictionary<int>>();
		}

		public void Set(ConfigCategory category, ICorDebugReferenceValue key, int entityID)
		{
			if (!_lookup.TryGetValue(category, out var mapping))
			{
				mapping = new ValueDictionary<int>();
				_lookup.Add(category, mapping);
			}

			mapping.Set(key, entityID);
		}

		public void Remove(ConfigCategory category, ICorDebugReferenceValue key)
		{
			if (_lookup.TryGetValue(category, out var mapping))
			{
				mapping.Remove(key);

				if (mapping.Count == 0)
				{
					_lookup.Remove(category);
				}
			}
		}

		public bool TryGetValue(ConfigCategory category, ICorDebugReferenceValue key, out int entityID)
		{
			if (!_lookup.TryGetValue(category, out var mapping))
			{
				entityID = 0;
				return false;
			}

			return mapping.TryGetValue(key, out entityID);
		}

		public void Purge()
		{
			List<ConfigCategory> emptyCategories = null;

			foreach (var pair in _lookup)
			{
				pair.Value.Purge();

				if (pair.Value.Count == 0)
				{
					if (emptyCategories == null)
					{
						emptyCategories = new List<ConfigCategory>();
					}

					emptyCategories.Add(pair.Key);
				}
			}

			if (emptyCategories != null)
			{
				foreach (var category in emptyCategories)
				{
					_lookup.Remove(category);
				}
			}
		}

		readonly Dictionary<ConfigCategory, ValueDictionary<int>> _lookup;
	}
}
