// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
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

		public static readonly RoutedCommand RunTool = new RoutedCommand(nameof(RunTool), typeof(ToolContextMenu));
		public static readonly RoutedCommand ShowDetails = new RoutedCommand(nameof(ShowDetails), typeof(ToolContextMenu));

		static ToolContextMenu()
		{
			CommandManager.RegisterClassCommandBinding(typeof(ToolContextMenu), new CommandBinding(RunTool, OnRunToolExecuted, CanRunToolExecute));
			CommandManager.RegisterClassCommandBinding(typeof(ToolContextMenu), new CommandBinding(ShowDetails, OnShowDetailsExecuted, CanShowDetailsExecute));
		}

		public FrameILData Frame
		{
			get => (FrameILData)GetValue(FrameProperty);
			set => SetValue(FrameProperty, value);
		}

		protected override void OnOpened(RoutedEventArgs e)
		{
			Items.Add(new MenuItem()
			{
				Header = "Show Details",
				Command = ShowDetails,
			});

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
						Command = RunTool,
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

		static void CanRunToolExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			var control = (ToolContextMenu)sender;

			e.CanExecute = control.Frame != null
				&& e.Parameter != null
				&& CanExecute((SettingsExternalTool)e.Parameter, control.Frame);
		}

		static void OnRunToolExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			var control = (ToolContextMenu)sender;
			var frame = control.Frame;
			var tool = (SettingsExternalTool)e.Parameter;

			if (frame != null && tool != null)
			{
				tool.Launch(frame);
			}
		}

		static void CanShowDetailsExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			var control = (ToolContextMenu)sender;

			e.CanExecute = control.Frame != null;
			e.Handled = true;
		}

		static void OnShowDetailsExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			var control = (ToolContextMenu)sender;
			e.Handled = true;

			new FrameDetailsView(control.Frame).Show();
		}

		static bool CanExecute(SettingsExternalTool tool, FrameILData frame)
		{
			if (frame.IsInMemory || tool == null) return false;

			var flags = tool.CalculateFlags();

			if (!File.Exists(tool.Process)) return false;
			if ((flags & ToolFlags.Invalid) != 0) return false;
			if ((flags & ToolFlags.NeedsSource) != 0 && frame.Source == null) return false;

			return true;
		}
	}
}
