// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Windows;
using System.Windows.Input;

namespace CausalityDbg.Main
{
	public partial class MainWindow : Window
	{
		public static readonly RoutedCommand Detach = new RoutedCommand(nameof(Detach), typeof(MainWindow));
		public static readonly RoutedCommand OpenSettings = new RoutedCommand(nameof(OpenSettings), typeof(MainWindow));
		public static readonly RoutedCommand Launch = new RoutedCommand(nameof(Launch), typeof(MainWindow));
		public static readonly RoutedCommand Attach = new RoutedCommand(nameof(Attach), typeof(MainWindow));
		public static readonly RoutedCommand ShowCaptureStatistics = new RoutedCommand(nameof(ShowCaptureStatistics), typeof(MainWindow));

		public MainWindow()
		{
			InitializeComponent();
		}

		void OnCloseExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;
			Close();
		}

		void OnHelpExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;

			var about = new About()
			{
				Owner = this,
			};

			about.Show();
		}

		void OnDetachExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;
			var model = (TrackerModel)DataContext;

			if (model != null)
			{
				model.Detach();
			}
		}

		void OnOpenSettingsExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;

			var model = new SettingsModel(SettingsStorage.Instance);

			var window = new SettingsWindow()
			{
				DataContext = model,
				Owner = this,
			};

			window.ShowDialog();
		}

		void OnLaunchExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;

			var model = (TrackerModel)DataContext;

			var window = new NewProcessView()
			{
				DataContext = new NewProcessModel(model),
				Owner = this,
			};

			window.ShowDialog();
		}

		void OnAttachExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;

			var model = (TrackerModel)DataContext;

			var window = new AttachProcessView()
			{
				DataContext = new AttachProcessModel(model),
				Owner = this,
			};

			window.ShowDialog();
		}

		void OnShowCaptureStatisticsExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;

			var model = (TrackerModel)DataContext;
			var source = model.Source;

			if (source == null)
			{
				MessageBox.Show("No details captured.", "Capture Details");
			}
			else
			{
				var window = new TraceDetails()
				{
					DataContext = new TraceDetailsViewModel(source),
					Owner = this,
				};

				window.Show();
			}
		}

		void OnProcessSelected(object sender, ProcessSelectedEventArgs e)
		{
			e.Handled = true;

			var model = (TrackerModel)DataContext;
			model.Attach(e.ProcessID);
		}
	}
}
