// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;

namespace CausalityDbg.Main
{
	sealed class ExceptionThrownEventArgs : EventArgs
	{
		public ExceptionThrownEventArgs(Exception exception)
		{
			Exception = exception;
		}

		public Exception Exception { get; }
	}
}
