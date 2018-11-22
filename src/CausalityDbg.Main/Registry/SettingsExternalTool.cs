// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Diagnostics;

namespace CausalityDbg.Main
{
	[DebuggerDisplay("Tool: {Name}")]
	sealed class SettingsExternalTool
	{
		public SettingsExternalTool(string name, string process, string arguments)
		{
			Name = name;
			Process = process;
			Arguments = arguments;
		}

		public string Name { get; }
		public string Process { get; }
		public string Arguments { get; }
	}
}
