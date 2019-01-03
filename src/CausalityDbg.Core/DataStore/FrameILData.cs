// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using CausalityDbg.Metadata;

namespace CausalityDbg.Core
{
	[DebuggerDisplay("FrameText = {FrameText}")]
	public sealed class FrameILData : FrameData
	{
		internal FrameILData(MetaFunction function, int? ilOffset, ImmutableArray<MetaCompound> genericArgs)
		{
			Function = function;
			ILOffset = ilOffset;
			GenericArgs = genericArgs;
		}

		internal MetaFunction Function { get; }
		public int? ILOffset { get; }
		internal ImmutableArray<MetaCompound> GenericArgs { get; }

		public string ModuleLocation => Function.Module.Name;
		public string ModuleName => Path.GetFileName(Function.Module.Name);
		public string FrameText => MetaFormatter.Format(Function, GenericArgs);
		public bool IsInMemory => (Function.Module.Flags & MetaModuleFlags.IsInMemory) != 0;

		public string IDString
		{
			get
			{
				var formatter = new MetaIDStringFormatter();
				formatter.AppendFunction(Function);
				return formatter.ToString();
			}
		}
	}
}
