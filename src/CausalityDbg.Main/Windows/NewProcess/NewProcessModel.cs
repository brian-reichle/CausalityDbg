// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Diagnostics;
using System.IO;
using CausalityDbg.Core;

namespace CausalityDbg.Main
{
	sealed class NewProcessModel : ViewModel
	{
		public NewProcessModel(TrackerModel tracker)
		{
			var settings = SettingsStorage.Launch;

			_tracker = tracker;
			_process = settings.Process;
			_directory = settings.Directory;
			_arguments = settings.Arguments;
		}

		public string Process
		{
			[DebuggerStepThrough]
			get => _process;
			set => SetField(ref _process, value ?? string.Empty);
		}

		public string Arguments
		{
			[DebuggerStepThrough]
			get => _arguments;
			set => SetField(ref _arguments, value ?? string.Empty);
		}

		public string Directory
		{
			[DebuggerStepThrough]
			get => _directory;
			set => SetField(ref _directory, value ?? string.Empty);
		}

		public NGenMode NGenMode
		{
			[DebuggerStepThrough]
			get => _ngenMode;
			set
			{
				_ngenMode = value;
				OnPropertyChanged();
			}
		}

		public void Launch()
		{
			_tracker.Launch(CreteLaunchArguments());

			var settings = SettingsStorage.Launch;

			if (Process != settings.Process ||
				Directory != settings.Directory ||
				Arguments != settings.Arguments ||
				NGenMode != settings.Mode)
			{
				SettingsStorage.Launch = new SettingsLaunch(Process, Directory, Arguments, NGenMode);
			}
		}

		LaunchArguments CreteLaunchArguments()
		{
			var directory = Directory;
			var process = Process;

			if (string.IsNullOrEmpty(directory))
			{
				directory = ".";
			}

			if (!Path.IsPathRooted(process))
			{
				process = Path.Combine(Environment.CurrentDirectory, process);
			}

			var args = new LaunchArguments(process, Arguments, directory);

			switch (NGenMode)
			{
				case NGenMode.Targeted:
					args.UseDebugNGENImages = true;
					break;

				case NGenMode.Dissable:
					var environment = Environment.GetEnvironmentVariables();
					environment["COMPLUS_ZapDisable"] = "1";
					args.Environment = environment;
					break;
			}

			return args;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		string _process;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		string _arguments;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		string _directory;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		NGenMode _ngenMode;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly TrackerModel _tracker;
	}
}
