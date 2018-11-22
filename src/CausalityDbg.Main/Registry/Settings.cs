// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Immutable;

namespace CausalityDbg.Main
{
	sealed class Settings
	{
		public Settings(ImmutableArray<SettingsExternalTool> tools)
		{
			Tools = tools;
		}

		public ImmutableArray<SettingsExternalTool> Tools { get; }
	}
}
