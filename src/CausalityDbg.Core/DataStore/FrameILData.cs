// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Diagnostics;
using System.IO;
using CausalityDbg.Core.MetaCache;

namespace CausalityDbg.Core
{
	[DebuggerDisplay("FrameText = {FrameText}")]
	public sealed class FrameILData : FrameData
	{
		internal FrameILData(MetaFrame mataFrame)
		{
			_metaFrame = mataFrame;
		}

		public string ModuleLocation => _metaFrame.Function.Module.Name;
		public string ModuleName => Path.GetFileName(_metaFrame.Function.Module.Name);
		public string FrameText => MetaFormatter.Format(_metaFrame);

		public string IDString
		{
			get
			{
				var formatter = new MetaIDStringFormatter();
				formatter.AppendFunction(_metaFrame.Function);
				return formatter.ToString();
			}
		}

		public int? ILOffset => _metaFrame.ILOffset;
		public bool IsInMemory => (_metaFrame.Function.Module.Flags & MetaModuleFlags.IsInMemory) != 0;

		public SourceSection Source
		{
			get
			{
				using (var provider = new SourceProvider())
				{
					return provider.Get(_metaFrame);
				}
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly MetaFrame _metaFrame;
	}
}
