// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CausalityDbg.Main
{
	public partial class AttachProcessView : Window
	{
		public static readonly RoutedCommand Attach = new RoutedCommand(nameof(Attach), typeof(AttachProcessView));

		public AttachProcessView()
		{
			InitializeComponent();
		}

		void OnCloseExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			Close();
		}

		void OnAttachExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			CommitRequest.Commit(this);

			if (HasErrors())
			{
				MessageBox.Show("This form has errors. Please fix your shit and try again.", "Attach");
			}
			else
			{
				var model = (AttachProcessModel)e.Parameter;
				Close();
				model.Attach();
			}
		}

		bool HasErrors()
		{
			return Validation.GetHasError(processTextBox);
		}
	}
}
