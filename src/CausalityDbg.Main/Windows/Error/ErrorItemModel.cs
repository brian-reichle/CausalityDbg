// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Diagnostics;

namespace CausalityDbg.Main
{
	sealed class ErrorItemModel : ViewModel
	{
		public ErrorItemModel(string summaryText, string detailText)
		{
			SummaryText = summaryText;
			DetailText = detailText;
		}

		public string SummaryText { get; }
		public string DetailText { get; }

		public bool HasBeenSeen
		{
			[DebuggerStepThrough]
			get => _hasBeenSeen;
			set => SetField(ref _hasBeenSeen, value);
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool _hasBeenSeen;
	}
}
