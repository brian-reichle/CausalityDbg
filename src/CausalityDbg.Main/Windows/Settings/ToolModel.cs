// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Diagnostics;

namespace CausalityDbg.Main
{
	[DebuggerDisplay("Tool: {Name}")]
	sealed class ToolModel : ViewModel
	{
		public ToolModel()
		{
		}

		public ToolModel(SettingsExternalTool tool)
		{
			_tool = tool;
			Name = tool.Name;
			Process = tool.Process;
			Arguments = tool.Arguments;
		}

		public string Name
		{
			[DebuggerStepThrough]
			get => _name;
			set => SetField(ref _name, value);
		}

		public string Process
		{
			[DebuggerStepThrough]
			get => _process;
			set => SetField(ref _process, value);
		}

		public string Arguments
		{
			[DebuggerStepThrough]
			get => _arguments;
			set => SetField(ref _arguments, value);
		}

		public SettingsExternalTool ToTool()
		{
			if (_tool != null &&
				_tool.Name == Name &&
				_tool.Process == Process &&
				_tool.Arguments == Arguments)
			{
				return _tool;
			}

			_tool = new SettingsExternalTool(Name, Process, Arguments);
			return _tool;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		string _name;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		string _process;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		string _arguments;
		SettingsExternalTool _tool;
	}
}
