// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Globalization;
using System.Windows.Data;

namespace CausalityDbg.Main
{
	[ValueConversion(typeof(object), typeof(bool), ParameterType = typeof(object))]
	sealed class EnumRadioConverter : IValueConverter
	{
		public object UnsetValue { get; set; }

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
			=> Equals(value, parameter);

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> ((bool)value) ? parameter : UnsetValue;
	}
}
