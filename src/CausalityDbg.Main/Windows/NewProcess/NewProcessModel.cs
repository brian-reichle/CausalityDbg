// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Diagnostics;
using System.IO;

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

			_tracker.Launch(process, Arguments, directory, NGenMode);

			var settings = SettingsStorage.Launch;

			if (Process != settings.Process ||
				Directory != settings.Directory ||
				Arguments != settings.Arguments ||
				NGenMode != settings.Mode)
			{
				SettingsStorage.Launch = new SettingsLaunch(Process, Directory, Arguments, NGenMode);
			}
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
