// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Diagnostics;

namespace CausalityDbg.Core
{
	[DebuggerDisplay("{HookType}: {Category.Name}")]
	public sealed class ConfigHook
	{
		internal ConfigHook(ConfigHookType hookType, ConfigCategory category, string key)
		{
			HookType = hookType;
			Category = category;
			Key = key;
		}

		public ConfigHookType HookType { get; }
		public ConfigCategory Category { get; }
		public string Key { get; }
	}
}
