// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Concurrent;
using System.Windows;
using System.Windows.Media;

namespace CausalityDbg.Main
{
	static class TimelineColors
	{
		public static readonly Pen SeparatorPen = Freeze(new Pen(Brushes.Black, 1));
		public static readonly Pen CollapsePen = Freeze(new Pen(Brushes.Red, 1));
		public static readonly Pen RelationshipPen1 = Freeze(new Pen(Brushes.White, 2));
		public static readonly Pen RelationshipPen2 = Freeze(new Pen(Brushes.Black, 1) { DashStyle = DashStyles.Dot });

		public static Brush GetBrush(int color)
		{
			return _brushes.GetOrAdd(color, c =>
			{
				var blue = (byte)(color & 0xFF);
				var green = (byte)((color >> 8) & 0xFF);
				var red = (byte)((color >> 16) & 0xFF);

				var brush = new SolidColorBrush(Color.FromRgb(red, green, blue));
				brush.Freeze();
				return brush;
			});
		}

		public static Pen GetPen(int color)
			=> _pens.GetOrAdd(color, c => new Pen(GetBrush(c), 1));

		static readonly ConcurrentDictionary<int, Brush> _brushes = new ConcurrentDictionary<int, Brush>();
		static readonly ConcurrentDictionary<int, Pen> _pens = new ConcurrentDictionary<int, Pen>();

		static T Freeze<T>(T value)
			where T : Freezable
		{
			value.Freeze();
			return value;
		}
	}
}
