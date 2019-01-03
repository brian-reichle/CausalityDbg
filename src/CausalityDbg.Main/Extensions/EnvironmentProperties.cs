// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Windows;
using CausalityDbg.Configuration;
using CausalityDbg.Core;
using CausalityDbg.Source;

namespace CausalityDbg.Main
{
	static class EnvironmentProperties
	{
		public static readonly DependencyProperty ConfigProperty = DependencyProperty.RegisterAttached(
			"Config",
			typeof(Config),
			typeof(EnvironmentProperties),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

		public static readonly DependencyProperty SourceProviderProperty = DependencyProperty.RegisterAttached(
			"SourceProvider",
			typeof(ISourceProvider),
			typeof(EnvironmentProperties),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

		public static Config GetConfig(FrameworkElement element) => (Config)element.GetValue(ConfigProperty);
		public static void SetConfig(FrameworkElement element, Config config) => element.SetValue(ConfigProperty, config);
		public static ISourceProvider GetSourceProvider(DependencyObject dobj) => (ISourceProvider)dobj.GetValue(SourceProviderProperty);
		public static void SetSourceProvider(DependencyObject dobj, ISourceProvider value) => dobj.SetValue(SourceProviderProperty, value);
	}
}
