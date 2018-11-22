// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Diagnostics;

namespace CausalityDbg.Core
{
	[DebuggerDisplay("Category ({Code})")]
	public sealed class ConfigCategory
	{
		internal ConfigCategory(string code, string name, int foregroundColor, int backgroundColor)
		{
			Code = code;
			Name = name;
			ForegroundColor = foregroundColor;
			BackgroundColor = backgroundColor;
		}

		internal ConfigCategory(ConfigCategoryType type, int fgColor, int bgColor)
		{
			Code = string.Empty;
			Name = type.ToString();
			ForegroundColor = fgColor;
			BackgroundColor = bgColor;
		}

		public string Code { get; }
		public string Name { get; }
		public int ForegroundColor { get; }
		public int BackgroundColor { get; }
	}
}
