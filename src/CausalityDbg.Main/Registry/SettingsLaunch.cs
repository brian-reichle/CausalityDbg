// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace CausalityDbg.Main
{
	sealed class SettingsLaunch
	{
		public SettingsLaunch(string process, string directory, string arguments, string runtimeVersion, NGenMode mode)
		{
			Process = process;
			Directory = directory;
			Arguments = arguments;
			RuntimeVersion = runtimeVersion;
			Mode = mode;
		}

		public string Process { get; }
		public string Directory { get; }
		public string Arguments { get; }
		public string RuntimeVersion { get; }
		public NGenMode Mode { get; }
	}
}
