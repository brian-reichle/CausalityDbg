// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace CausalityDbg.Main
{
	partial class ErrorLogWindow : Window
	{
		static ErrorLogWindow()
		{
			CommandManager.RegisterClassCommandBinding(typeof(ErrorLogWindow), new CommandBinding(ApplicationCommands.Close, OnCloseExecuted));
		}

		ErrorLogWindow()
		{
			_errors = new ObservableCollection<ErrorItemModel>();
			_view = CollectionViewSource.GetDefaultView(_errors);
			DataContext = _errors;
			InitializeComponent();

			_view.CurrentChanged += View_CurrentChanged;
		}

		public static void Log(ErrorItemModel item)
		{
			if (_currentWindow == null)
			{
				_currentWindow = new ErrorLogWindow();
				_currentWindow._errors.Add(item);
				_currentWindow.Show();
			}
			else
			{
				_currentWindow._errors.Add(item);
			}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);

			if (!e.Cancel)
			{
				var write = 0;

				try
				{
					_flushing = true;

					for (var read = 0; read < _errors.Count; read++)
					{
						var current = _errors[read];

						if (!current.HasBeenSeen)
						{
							_errors[write++] = current;
						}
					}

					if (write > 0)
					{
						while (write < _errors.Count)
						{
							_errors.RemoveAt(_errors.Count - 1);
						}
					}
				}
				finally
				{
					_flushing = false;
				}

				if (write > 0)
				{
					_view.MoveCurrentToFirst();
					e.Cancel = true;
				}
				else
				{
					_currentWindow = null;
				}
			}
		}

		static void OnCloseExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			((Window)sender).Close();
		}

		void View_CurrentChanged(object sender, EventArgs e)
		{
			if (!_flushing && _view.CurrentItem is ErrorItemModel item)
			{
				item.HasBeenSeen = true;
			}
		}

		static ErrorLogWindow _currentWindow;

		bool _flushing;
		readonly ObservableCollection<ErrorItemModel> _errors;
		readonly ICollectionView _view;
	}
}
