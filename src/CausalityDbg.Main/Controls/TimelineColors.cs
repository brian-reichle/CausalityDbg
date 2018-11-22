// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Generic;
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
			lock (_brushes)
			{
				if (!_brushes.TryGetValue(color, out var brush))
				{
					var blue = (byte)(color & 0xFF);
					var green = (byte)((color >> 8) & 0xFF);
					var red = (byte)((color >> 16) & 0xFF);

					brush = new SolidColorBrush(Color.FromRgb(red, green, blue));
					brush.Freeze();
					_brushes.Add(color, brush);
				}

				return brush;
			}
		}

		public static Pen GetPen(int color)
		{
			lock (_pens)
			{
				if (!_pens.TryGetValue(color, out var pen))
				{
					pen = new Pen(GetBrush(color), 1);
					_pens.Add(color, pen);
				}

				return pen;
			}
		}

		static readonly Dictionary<int, Brush> _brushes = new Dictionary<int, Brush>();
		static readonly Dictionary<int, Pen> _pens = new Dictionary<int, Pen>();

		static T Freeze<T>(T value)
			where T : Freezable
		{
			value.Freeze();
			return value;
		}
	}
}
