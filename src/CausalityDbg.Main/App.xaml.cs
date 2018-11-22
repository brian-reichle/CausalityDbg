// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Windows;
using System.Windows.Threading;
using CausalityDbg.Core;

namespace CausalityDbg.Main
{
	public partial class App : Application
	{
		const string ProviderKey = "Provider";
		const string ConfigKey = "Config";

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			var config = ConfigParser.Load("Config\\Config.xml");

			_wrapper = new TrackerWrapper(Dispatcher, config);
			_wrapper.ExceptionThrown += ExceptionThrown;

			Dispatcher.UnhandledException += UnhandledException;
			AppDomain.CurrentDomain.UnhandledException += UnhandledException;

			Resources.Add(ConfigKey, config);
			Resources.Add(ProviderKey, new TrackerModel(_wrapper));
		}

		protected override void OnExit(ExitEventArgs e)
		{
			Resources.Remove(ProviderKey);

			if (_wrapper != null)
			{
				_wrapper.Dispose();
			}

			base.OnExit(e);
		}

		static void ExceptionThrown(object sender, ExceptionThrownEventArgs e)
		{
			var attachEx = e.Exception as AttachException;

			if (attachEx != null)
			{
				MessageBox.Show(attachEx.Message, "Error attaching to process");
			}
			else
			{
				ExceptionReporter.Report(e.Exception);
			}
		}

		static void UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			ExceptionReporter.Report(e.Exception);
			e.Handled = true;
		}

		static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			var ex = e.ExceptionObject as Exception;

			if (ex != null)
			{
				ExceptionReporter.Report(ex);
			}
		}

		TrackerWrapper _wrapper;
	}
}
