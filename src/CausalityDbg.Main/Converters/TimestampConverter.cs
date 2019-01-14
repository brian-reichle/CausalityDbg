// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using CausalityDbg.DataStore;

namespace CausalityDbg.Main
{
	sealed class TimestampConverter : DependencyObject, IMultiValueConverter
	{
		public TimestampConverter()
		{
			var tmp = 1d / Stopwatch.Frequency;
			var count = 1;
			var template = "0.000000000";

			while (tmp < 1)
			{
				count++;
				tmp *= 10;
			}

			_formatString = count >= template.Length ? template : template.Substring(0, count);
		}

		public string DefaultValue { get; set; }

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values == null || values.Length != 2 || values[0] == null) return DefaultValue;

			var rawTimestamp = (long)values[0];
			var provider = values[1] as IDataProvider;
			if (provider == null) return "Missing Provider";

			var timestamp = GetTimestamp(provider, rawTimestamp);
			if (timestamp == null) return "Unmapped Value";

			return ((timestamp.Value - GetInitalOffset(provider)) / (double)Stopwatch.Frequency)
				.ToString(_formatString, CultureInfo.InvariantCulture);
		}

		object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		static long? GetTimestamp(IDataProvider provider, long rawTimestamp)
		{
			foreach (var section in provider.FindSections(rawTimestamp, rawTimestamp))
			{
				return rawTimestamp - section.ViewStart + section.RealStart;
			}

			return null;
		}

		static long GetInitalOffset(IDataProvider provider)
		{
			foreach (var section in provider.FindSections(0, 0))
			{
				return section.RealStart;
			}

			return 0;
		}

		readonly string _formatString;
	}
}
