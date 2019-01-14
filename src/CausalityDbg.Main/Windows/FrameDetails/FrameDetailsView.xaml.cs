// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CausalityDbg.Core;
using CausalityDbg.DataStore;
using CausalityDbg.Source;

namespace CausalityDbg.Main
{
	public partial class FrameDetailsView : Window
	{
		const string NA = "N/A";
		const string RowSharedSizeGroup = "Field";

		public FrameDetailsView(FrameILData frame, ISourceProvider provider)
		{
			InitializeComponent();

			AddFullWidthRow("Module", NewTextField(frame.ModuleName));
			AddFullWidthRow("Method", NewTextField(frame.FrameText));
			AddFullWidthRow("IL Offset", NewTextField(frame.ILOffset.HasValue ? FormatHex(frame.ILOffset.Value) : NA));
			AddFullWidthRow("Is In Memory", NewBoolField(frame.IsInMemory));

			var source = provider.GetSourceSection(frame);

			string filename;
			string line;
			string column;

			if (source != null)
			{
				filename = source.File;
				line = FormatDecimal(source.FromLine);
				column = FormatDecimal(source.FromColumn);
			}
			else
			{
				filename = line = column = NA;
			}

			AddFullWidthRow("Filename", NewTextField(filename));
			AddPairRow("Line", NewTextField(line), "Column", NewTextField(column));
		}

		void AddFullWidthRow(string label, FrameworkElement element)
		{
			var labelControl = new TextBlock() { Text = label };
			var rowNum = PART_Grid.RowDefinitions.Count;

			PART_Grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto, SharedSizeGroup = RowSharedSizeGroup });

			Grid.SetColumn(labelControl, 0);
			Grid.SetRow(labelControl, rowNum);

			Grid.SetColumn(element, 2);
			Grid.SetColumnSpan(element, 5);
			Grid.SetRow(element, rowNum);

			PART_Grid.Children.Add(labelControl);
			PART_Grid.Children.Add(element);
		}

		void AddPairRow(string label1, FrameworkElement element1, string label2, FrameworkElement element2)
		{
			var labelControl1 = new TextBlock() { Text = label1 };
			var labelControl2 = new TextBlock() { Text = label2 };
			var rowNum = PART_Grid.RowDefinitions.Count;

			PART_Grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto, SharedSizeGroup = RowSharedSizeGroup });

			Grid.SetColumn(labelControl1, 0);
			Grid.SetRow(labelControl1, rowNum);

			Grid.SetColumn(element1, 2);
			Grid.SetRow(element1, rowNum);

			Grid.SetColumn(labelControl2, 4);
			Grid.SetRow(labelControl2, rowNum);

			Grid.SetColumn(element2, 6);
			Grid.SetRow(element2, rowNum);

			PART_Grid.Children.Add(labelControl1);
			PART_Grid.Children.Add(labelControl2);
			PART_Grid.Children.Add(element1);
			PART_Grid.Children.Add(element2);
		}

		static string FormatDecimal(int value) => value.ToString(CultureInfo.InvariantCulture);
		static string FormatHex(int value) => value.ToString("X4", CultureInfo.InvariantCulture);

		static CheckBox NewBoolField(bool value)
		{
			return new CheckBox()
			{
				IsEnabled = false,
				IsChecked = value,
				VerticalAlignment = VerticalAlignment.Center,
			};
		}

		static TextBox NewTextField(string value)
		{
			return new TextBox()
			{
				IsReadOnly = true,
				Text = value,
				Background = SystemColors.ControlBrush,
			};
		}

		void Close_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Close();
		}
	}
}
