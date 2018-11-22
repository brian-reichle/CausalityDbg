// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Diagnostics;

namespace CausalityDbg.Core
{
	[DebuggerDisplay("{Name}")]
	public sealed class ArgumentType
	{
		public ArgumentType(string name, bool byRef)
		{
			Name = name;
			ByRef = byRef;
		}

		public string Name { get; }
		public bool ByRef { get; }
	}
}
