// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Globalization;
using System.Windows.Data;

namespace CausalityDbg.Main
{
	[ValueConversion(typeof(int), typeof(string), ParameterType = typeof(int))]
	sealed class LabelAndCountConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var label = (string)parameter;
			var count = (int)value;

			return count == 0 ? label : (label + " (" + count + ")");
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotImplementedException();
	}
}
