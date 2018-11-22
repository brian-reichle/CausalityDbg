// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Windows;
using System.Windows.Input;

namespace CausalityDbg.Main
{
	public partial class SettingsWindow : Window
	{
		public static readonly RoutedCommand AddTool = new RoutedCommand(nameof(AddTool), typeof(SettingsWindow));
		public static readonly RoutedCommand RemoveTool = new RoutedCommand(nameof(RemoveTool), typeof(SettingsWindow));

		public SettingsWindow()
		{
			InitializeComponent();
		}

		void OnSaveExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			SettingsStorage.Instance = ((SettingsModel)e.Parameter).ToSettings();
			Close();
		}

		void OnCloseExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			Close();
		}

		void OnAddToolExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			((SettingsModel)DataContext).AddTool();
		}

		void OnRemoveToolExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			((SettingsModel)DataContext).RemoveTool((ToolModel)e.Parameter);
		}
	}
}
