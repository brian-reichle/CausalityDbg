// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CausalityDbg.Main
{
	static class ControlTextExtensions
	{
		public static Typeface GetTypeface(this FrameworkElement fe)
		{
			return new Typeface(
				TextBlock.GetFontFamily(fe),
				TextBlock.GetFontStyle(fe),
				TextBlock.GetFontWeight(fe),
				TextBlock.GetFontStretch(fe));
		}

		public static FormattedText GetFormattedText(this FrameworkElement fe, string text, CultureInfo culture)
		{
			var dpi = VisualTreeHelper.GetDpi(fe);
			return new FormattedText(
				text,
				culture,
				TextBlock.GetFlowDirection(fe),
				fe.GetTypeface(),
				TextBlock.GetFontSize(fe),
				TextBlock.GetForeground(fe),
				dpi.PixelsPerDip);
		}
	}
}
