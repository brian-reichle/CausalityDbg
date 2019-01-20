// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Globalization;
using System.Windows.Data;
using CausalityDbg.DataStore;

namespace CausalityDbg.Main
{
	[ValueConversion(typeof(IEventScope), typeof(TraceWrapper))]
	sealed class StackTraceProcessor : IValueConverter
	{
		public static StackTraceProcessor Default { get; } = new StackTraceProcessor();

		StackTraceProcessor()
		{
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
			=> value is IEventScope current ? new TraceWrapper(current) : null;

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotSupportedException();
	}
}
