// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections;

namespace CausalityDbg.Core
{
	public interface ITracker : IDisposable
	{
		void Attach(int pid, ITrackerCallback callback);
		void Attach(string process, string arguments, string directory, ITrackerCallback callback, IDictionary environment, bool useDebugNGENImages);
	}
}
