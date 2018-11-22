// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Immutable;
using System.Diagnostics;

namespace CausalityDbg.Core
{
	[DebuggerDisplay("ConfigMethodRef: {MethodName.Text}")]
	public sealed class ConfigMethodRef
	{
		internal ConfigMethodRef(MethodRef method, ImmutableArray<ConfigHook> hooks)
		{
			Method = method;
			Hooks = hooks;
		}

		public MethodRef Method { get; }
		public ImmutableArray<ConfigHook> Hooks { get; }
	}
}
