// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Immutable;
using System.Diagnostics;

namespace CausalityDbg.Configuration
{
	[DebuggerDisplay("ConfigClass: {FullClassName}")]
	public sealed class ConfigClass
	{
		internal ConfigClass(string fullClassName, string className, ImmutableArray<ConfigMethodRef> methods, ImmutableArray<ConfigClass> classes)
		{
			FullClassName = fullClassName;
			ClassName = className;
			Methods = methods;
			NestedClasses = classes;
		}

		public string FullClassName { get; }
		public string ClassName { get; }
		public ImmutableArray<ConfigMethodRef> Methods { get; }
		public ImmutableArray<ConfigClass> NestedClasses { get; }
	}
}
