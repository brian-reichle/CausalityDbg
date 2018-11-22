// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Windows;

namespace CausalityDbg.Main
{
	sealed class ProcessSelectedEventArgs : RoutedEventArgs
	{
		public ProcessSelectedEventArgs(RoutedEvent routedEvent, object source, int processID)
			: base(routedEvent, source)
		{
			ProcessID = processID;
		}

		public int ProcessID { get; }
	}
}
