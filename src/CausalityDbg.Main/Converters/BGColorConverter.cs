// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using CausalityDbg.Configuration;

namespace CausalityDbg.Main
{
	[ValueConversion(typeof(ConfigCategory), typeof(Brush))]
	sealed class BGColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is ConfigCategory category)
			{
				return TimelineColors.GetBrush(category.BackgroundColor);
			}

			return null;
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotSupportedException();
	}
}
