// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;

namespace CausalityDbg.DataStore
{
	public interface IDataProvider
	{
		int CountBands { get; }
		int CountScopes { get; }
		int CountEvents { get; }

		IEnumerable<Band> Bands { get; }
		IEnumerable<Scope> FindScopes(long lowerBound, long upperBound);
		IEnumerable<Event> FindEvents(long lowerBound, long upperBound);
		IEnumerable<TimelineSection> FindSections(long lowerBound, long upperBound);

		long UpperBound { get; }

		event EventHandler DataChanged;
	}
}
