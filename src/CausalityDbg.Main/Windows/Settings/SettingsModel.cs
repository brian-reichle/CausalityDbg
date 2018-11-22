// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;

namespace CausalityDbg.Main
{
	sealed class SettingsModel : ViewModel
	{
		public SettingsModel(Settings settings)
		{
			_settings = settings;
			Tools = new ObservableCollection<ToolModel>(settings.Tools.Select(x => new ToolModel(x)));
		}

		public void AddTool()
		{
			Tools.Add(new ToolModel()
			{
				Name = "New Tool",
			});
		}

		public void RemoveTool(ToolModel tool)
		{
			Tools.Remove(tool);
		}

		public ObservableCollection<ToolModel> Tools { get; }

		public Settings ToSettings()
		{
			var tools = ToExternalTools(Tools, _settings.Tools);

			if (tools != _settings.Tools)
			{
				_settings = new Settings(tools);
			}

			return _settings;
		}

		static ImmutableArray<SettingsExternalTool> ToExternalTools(IList<ToolModel> newTools, ImmutableArray<SettingsExternalTool> existingTools)
		{
			ImmutableArray<SettingsExternalTool>.Builder builder = null;

			for (var i = 0; i < newTools.Count; i++)
			{
				var actual = newTools[i].ToTool();

				if (builder != null)
				{
					builder[i] = actual;
				}
				else if (i >= existingTools.Length || existingTools[i] != actual)
				{
					builder = ImmutableArray.CreateBuilder<SettingsExternalTool>(newTools.Count);
					builder.Count = newTools.Count;

					for (var j = 0; j < i; j++)
					{
						builder[j] = newTools[j].ToTool();
					}

					builder[i] = actual;
				}
			}

			if (builder != null)
			{
				return builder.Count == builder.Capacity ? builder.MoveToImmutable() : builder.ToImmutable();
			}
			else if (newTools.Count < existingTools.Length)
			{
				return existingTools.RemoveRange(newTools.Count, existingTools.Length - newTools.Count);
			}
			else
			{
				return existingTools;
			}
		}

		Settings _settings;
	}
}
