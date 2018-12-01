// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections;

namespace CausalityDbg.Core
{
	public sealed class LaunchArguments
	{
		public LaunchArguments(string process, string arguments, string directory)
		{
			if (process == null) throw new ArgumentNullException(nameof(process));
			if (arguments == null) throw new ArgumentNullException(nameof(arguments));
			if (directory == null) throw new ArgumentNullException(nameof(directory));

			Process = process;
			Arguments = arguments;
			Directory = directory;
		}

		public string Process { get; }
		public string Arguments { get; }
		public string Directory { get; }
		public IDictionary Environment { get; set; }
		public bool UseDebugNGENImages { get; set; }
	}
}
