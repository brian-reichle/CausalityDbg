// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Diagnostics;

namespace CausalityDbg.Main
{
	sealed class AttachProcessModel : ViewModel
	{
		public AttachProcessModel(TrackerModel tracker)
		{
			_tracker = tracker;
		}

		public int ProcessID
		{
			[DebuggerStepThrough]
			get => _processID;
			set => SetField(ref _processID, value);
		}

		public void Attach()
		{
			_tracker.Attach(ProcessID);
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		int _processID;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly TrackerModel _tracker;
	}
}
