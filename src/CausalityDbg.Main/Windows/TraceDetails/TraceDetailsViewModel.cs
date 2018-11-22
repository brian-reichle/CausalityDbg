// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace CausalityDbg.Main
{
	sealed class TraceDetailsViewModel : ViewModel
	{
		public TraceDetailsViewModel(IDataProvider provider)
		{
			_provider = provider;
		}

		public int CountBands => _provider.CountBands;
		public int CountScopes => _provider.CountScopes;
		public int CountEvents => _provider.CountEvents;

		readonly IDataProvider _provider;
	}
}
