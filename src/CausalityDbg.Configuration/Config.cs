// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Immutable;

namespace CausalityDbg.Configuration
{
	public sealed class Config
	{
		internal Config(ImmutableArray<ConfigAssembly> assemblies, ImmutableArray<ConfigCategory> categories, ImmutableArray<ConfigCategory> systemTriggers)
		{
			Assemblies = assemblies;
			Categories = categories;
			_systemTriggers = systemTriggers;
		}

		public ImmutableArray<ConfigCategory> Categories { get; }
		public ImmutableArray<ConfigAssembly> Assemblies { get; }

		public ConfigCategory ManagedThreadCategory => _systemTriggers[(int)ConfigCategoryType.ManagedThread];
		public ConfigCategory ExceptionCategory => _systemTriggers[(int)ConfigCategoryType.Exception];
		public ConfigCategory BreakCategory => _systemTriggers[(int)ConfigCategoryType.Break];
		public ConfigCategory TraceCategory => _systemTriggers[(int)ConfigCategoryType.Trace];
		public ConfigCategory CatchCategory => _systemTriggers[(int)ConfigCategoryType.Catch];
		public ConfigCategory FilterCategory => _systemTriggers[(int)ConfigCategoryType.Finally];
		public ConfigCategory FinallyCategory => _systemTriggers[(int)ConfigCategoryType.Finally];
		public ConfigCategory FaultCategory => _systemTriggers[(int)ConfigCategoryType.Fault];

		readonly ImmutableArray<ConfigCategory> _systemTriggers;
	}
}
