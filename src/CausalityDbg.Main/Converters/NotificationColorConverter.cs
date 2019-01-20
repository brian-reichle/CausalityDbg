// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using CausalityDbg.Core;

namespace CausalityDbg.Main
{
	[ValueConversion(typeof(TrackerNotificationLevel?), typeof(Brush))]
	sealed class NotificationColorConverter : IValueConverter
	{
		public static NotificationColorConverter Default { get; } = new NotificationColorConverter();

		NotificationColorConverter()
		{
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var level = value as TrackerNotificationLevel?;

			if (level != null)
			{
				return GetBrush(level.Value);
			}

			return null;
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotSupportedException();

		static Brush GetBrush(TrackerNotificationLevel value1)
		{
			switch (value1)
			{
				case TrackerNotificationLevel.Error: return Brushes.Red;
				case TrackerNotificationLevel.Warning: return Brushes.Goldenrod;
				default: return null;
			}
		}
	}
}
