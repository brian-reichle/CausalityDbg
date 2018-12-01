// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.ObjectModel;
using System.Diagnostics;
using CausalityDbg.Core;

namespace CausalityDbg.Main
{
	sealed class TrackerModel : ViewModel, ITrackerStatus
	{
		public TrackerModel(TrackerWrapper tracker)
		{
			_tracker = tracker;
			_tracker.Status = this;
			Notifications = new ObservableCollection<Notification>();
		}

		public string Caption
		{
			get
			{
				if (_caption == null)
				{
					if (ProcessID == 0)
					{
						return "CausalityDbg";
					}
					else if (string.IsNullOrEmpty(ProcessName))
					{
						_caption = "CausalityDbg - " + ProcessID;
					}
					else
					{
						_caption = "CausalityDbg - " + ProcessName + " (" + ProcessID + ")";
					}
				}

				return _caption;
			}
		}

		public int ProcessID
		{
			[DebuggerStepThrough]
			get => _processID;
			private set
			{
				_processID = value;
				_caption = null;
				OnPropertyChanged();
				OnPropertyChanged(nameof(Caption));
			}
		}

		public string ProcessName
		{
			[DebuggerStepThrough]
			get => _processName;
			private set
			{
				_processName = value;
				_caption = null;
				OnPropertyChanged();
				OnPropertyChanged(nameof(Caption));
			}
		}

		public IDataProvider Source
		{
			[DebuggerStepThrough]
			get => _source;
			set
			{
				_source = value;
				OnPropertyChanged();
			}
		}

		public IEventScope Selected
		{
			[DebuggerStepThrough]
			get => _selected;
			set
			{
				_selected = value;
				OnPropertyChanged();
			}
		}

		public ObservableCollection<Notification> Notifications { get; }

		public void Launch(LaunchArguments args)
		{
			var provider = new DataProvider();
			Notifications.Clear();
			_tracker.StartProcess(args, provider);
			Source = provider;
			Selected = null;
		}

		public void Attach(int processID)
		{
			var provider = new DataProvider();
			Notifications.Clear();
			_tracker.SetProcess(processID, provider);
			Source = provider;
			Selected = null;
		}

		public void Detach()
		{
			_tracker.Detach();
		}

		public void Notify(TrackerNotificationLevel level, string modulePath, string text)
			=> Notifications.Add(new Notification(level, modulePath, text));

		void ITrackerStatus.SetStatus(int pid)
		{
			ProcessID = pid;
			var data = ProcessData.FromPID(pid);
			ProcessName = data?.ProcessName;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		int _processID;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		string _processName;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		string _caption;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		IDataProvider _source;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		IEventScope _selected;
		readonly TrackerWrapper _tracker;
	}
}
