// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Windows;
using System.Windows.Input;

namespace CausalityDbg.Main
{
	partial class About : Window
	{
		public About()
		{
			InitializeComponent();
		}

		void Close_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;
			Close();
		}
	}
}
