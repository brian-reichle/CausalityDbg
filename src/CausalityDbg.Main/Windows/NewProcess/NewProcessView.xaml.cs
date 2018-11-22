// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;

namespace CausalityDbg.Main
{
	public partial class NewProcessView : Window
	{
		public static readonly RoutedCommand Launch = new RoutedCommand(nameof(Launch), typeof(NewProcessView));
		public static readonly RoutedCommand Find = new RoutedCommand(nameof(Find), typeof(NewProcessView));

		public NewProcessView()
		{
			InitializeComponent();
		}

		void OnCloseExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			Close();
		}

		void OnLaunchExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			CommitRequest.Commit(this);

			if (HasErrors())
			{
				MessageBox.Show("This form has errors. Please fix your shit and try again.", "Launch");
			}
			else
			{
				var model = (NewProcessModel)e.Parameter;
				Close();
				model.Launch();
			}
		}

		void OnFindExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			var model = (NewProcessModel)e.Parameter;
			var dialog = new OpenFileDialog();
			dialog.FileName = model.Process;
			dialog.Filter = "All Files|*.*|Executable|*.exe";
			dialog.FilterIndex = 2;

			if (!string.IsNullOrWhiteSpace(model.Directory))
			{
				dialog.InitialDirectory = model.Directory;
			}
			else
			{
				dialog.InitialDirectory = Environment.CurrentDirectory;
			}

			if (dialog.ShowDialog().Value)
			{
				model.Process = dialog.FileName;
			}
		}

		bool HasErrors()
		{
			return Validation.GetHasError(processTextBox)
				|| Validation.GetHasError(directoryTextBox);
		}
	}
}
