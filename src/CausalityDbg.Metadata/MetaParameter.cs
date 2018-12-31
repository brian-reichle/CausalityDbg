// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Diagnostics;

namespace CausalityDbg.Metadata
{
	[DebuggerDisplay("Param: {Name}")]
	public sealed class MetaParameter
	{
		internal MetaParameter(string name, MetaCompound parameterType)
		{
			if (parameterType == null) throw new ArgumentNullException(nameof(parameterType));

			Name = name;
			ParameterType = parameterType;
		}

		public string Name { get; }
		public MetaCompound ParameterType { get; }
	}
}
