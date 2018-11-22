// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Diagnostics;
using System.Globalization;
using CausalityDbg.Core;

namespace CausalityDbg.Main
{
	static class ToolLauncher
	{
		public static Process Launch(this SettingsExternalTool tool, FrameILData frame)
		{
			return tool.Launch(x =>
			{
				switch (x)
				{
					case "AssemblyLocation": return frame.ModuleLocation;
					case "IDString": return frame.IDString;
					case "Source": return frame.Source?.File;
					case "Line": return frame.Source?.FromLine.ToString(CultureInfo.InvariantCulture);
					default: return null;
				}
			});
		}

		public static Process Launch(this SettingsExternalTool tool, Func<string, string> macroProcessor)
		{
			var info = new ProcessStartInfo();
			info.FileName = tool.Process;
			info.Arguments = TemplateProcessor.ProcessTemplate(tool.Arguments, macroProcessor);

			return Process.Start(info);
		}

		public static ToolFlags MacroFlags(string macro)
		{
			if (macro != null)
			{
				switch (macro)
				{
					case "Source":
					case "Line":
						return ToolFlags.NeedsSource;

					case "AssemblyLocation":
					case "IDString":
						return ToolFlags.None;
				}
			}

			return ToolFlags.Invalid;
		}

		public static ToolFlags CalculateFlags(this SettingsExternalTool tool)
		{
			var result = ToolFlags.None;

			try
			{
				if (tool != null)
				{
					foreach (var macro in TemplateProcessor.GetMacros(tool.Arguments))
					{
						result |= MacroFlags(macro);
					}
				}
			}
			catch (FormatException)
			{
				result |= ToolFlags.Invalid;
			}

			return result;
		}
	}
}
