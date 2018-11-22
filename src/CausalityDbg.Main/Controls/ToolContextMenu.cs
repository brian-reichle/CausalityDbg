// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CausalityDbg.Core;

namespace CausalityDbg.Main
{
	sealed class ToolContextMenu : ContextMenu
	{
		public static readonly DependencyProperty FrameProperty = DependencyProperty.Register(
			"Frame",
			typeof(FrameILData),
			typeof(ToolContextMenu),
			new FrameworkPropertyMetadata(null));

		public ToolContextMenu()
		{
			_command = new ToolCommand(this);
		}

		public FrameILData Frame
		{
			get => (FrameILData)GetValue(FrameProperty);
			set => SetValue(FrameProperty, value);
		}

		protected override void OnOpened(RoutedEventArgs e)
		{
			var detailsItem = new MenuItem()
			{
				Header = "Show Details",
			};

			detailsItem.Click += ShowDetails;

			Items.Add(detailsItem);

			var externalTools = SettingsStorage.Instance.Tools;

			if (externalTools.Length > 0)
			{
				Items.Add(new Separator());

				foreach (var tool in externalTools)
				{
					Items.Add(new MenuItem()
					{
						Header = tool.Name,
						CommandParameter = tool,
						Command = _command,
					});
				}
			}

			base.OnOpened(e);
		}

		protected override void OnClosed(RoutedEventArgs e)
		{
			base.OnClosed(e);
			Items.Clear();
		}

		void ShowDetails(object sender, RoutedEventArgs e) => new FrameDetailsView(Frame).Show();

		void RunTool(SettingsExternalTool tool)
		{
			if (Frame != null)
			{
				tool.Launch(Frame);
			}
		}

		sealed class ToolCommand : ICommand
		{
			public ToolCommand(ToolContextMenu parent)
			{
				_parent = parent;
			}

			public void Execute(object parameter) => _parent.RunTool((SettingsExternalTool)parameter);

			public bool CanExecute(object parameter)
			{
				var frame = _parent.Frame;

				if (frame == null ||
					frame.IsInMemory ||
					parameter == null) return false;

				var tool = (SettingsExternalTool)parameter;
				var flags = tool.CalculateFlags();

				if (!File.Exists(tool.Process)) return false;
				if ((flags & ToolFlags.Invalid) != 0) return false;
				if ((flags & ToolFlags.NeedsSource) != 0 && _parent.Frame.Source == null) return false;

				return true;
			}

			event EventHandler ICommand.CanExecuteChanged
			{
				add { }
				remove { }
			}

			readonly ToolContextMenu _parent;
		}

		readonly ICommand _command;
	}
}
