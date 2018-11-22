// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace CausalityDbg.Core
{
	[DebuggerDisplay("ConfigAssembly: {AssemblyRef.FullyQualifiedName}")]
	public sealed class ConfigAssembly
	{
		internal static readonly ReadOnlyCollection<ConfigAssembly> EmptyList = new ReadOnlyCollection<ConfigAssembly>(System.Array.Empty<ConfigAssembly>());

		internal ConfigAssembly(AssemblyRef assemblyRef, ImmutableArray<ConfigClass> classes)
		{
			AssemblyRef = assemblyRef;
			Classes = classes;
		}

		public AssemblyRef AssemblyRef { get; }
		public ImmutableArray<ConfigClass> Classes { get; }
	}
}
