// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace CausalityDbg.Main
{
	sealed class TimelineControlScale : FrameworkElement
	{
		const int MarkHeight = 4;

		public TimelineControlScale(TimelineControlView view)
		{
			_view = view;
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			return new Size(
				double.IsPositiveInfinity(availableSize.Width) ? 10 : availableSize.Width,
				double.IsPositiveInfinity(availableSize.Height) ? 20 : Math.Min(20, availableSize.Height));
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			drawingContext.DrawLine(TimelineColors.SeparatorPen, new Point(0, ActualHeight), new Point(ActualWidth, ActualHeight));

			var source = _view.Source;

			if (source == null)
			{
				return;
			}

			var transform = _view.BuildTransform();
			var inverseTransform = transform.Inverse;

			var bounds = inverseTransform.TransformBounds(new Rect(new Size(ActualWidth, ActualHeight)));

			var fromTimestamp = (long)Math.Floor(bounds.Left);
			var toTimestamp = (long)Math.Ceiling(bounds.Right);

			DrawMarkers(drawingContext, transform, fromTimestamp, toTimestamp);
		}

		void DrawMarkers(DrawingContext drawingContext, Transform transform, long fromTimestamp, long toTimestamp)
		{
			var span = toTimestamp - fromTimestamp;
			var step = Stopwatch.Frequency;
			var initalOffset = GetInitalOffset();

			while (step * 2 > span)
			{
				step /= 10;
			}

			foreach (var section in _view.Source.FindSections(fromTimestamp, toTimestamp))
			{
				var lowerBound = Math.Max(fromTimestamp, section.ViewStart);
				var upperBound = Math.Min(toTimestamp, section.ViewEnd);

				DrawSectionMarkers(drawingContext, transform, section, initalOffset, step, lowerBound, upperBound);
				DrawSectionTime(drawingContext, transform, section, initalOffset, lowerBound, upperBound);
			}
		}

		void DrawSectionMarkers(DrawingContext drawingContext, Transform transform, TimelineSection section, long offset, long step, long fromTimestamp, long toTimestamp)
		{
			var diff = section.RealStart - offset - section.ViewStart;
			var start = RoundToLastMultiple(fromTimestamp + diff, step) - diff;
			var top = ActualHeight - MarkHeight;
			var bottom = ActualHeight;

			if (start < fromTimestamp)
			{
				start += step;
			}

			for (var i = start; i < toTimestamp; i += step)
			{
				var x = transform.Transform(new Point(i, 0)).X;
				drawingContext.DrawLine(TimelineColors.SeparatorPen, new Point(x, top), new Point(x, bottom));
			}
		}

		void DrawSectionTime(DrawingContext drawingContext, Transform transform, TimelineSection section, long offset, long fromTimestamp, long toTimestamp)
		{
			var ticks = fromTimestamp - section.ViewStart + section.RealStart - offset;
			var seconds = Math.Round(ticks / (double)Stopwatch.Frequency, 4);
			var formattedText = this.GetFormattedText(seconds.ToString(CultureInfo.InvariantCulture), CultureInfo.CurrentCulture);
			var rect = transform.TransformBounds(new Rect(new Point(fromTimestamp, 0), new Point(toTimestamp, 0)));

			if (rect.Width > formattedText.Width + 4)
			{
				drawingContext.PushClip(new RectangleGeometry(new Rect(rect.Left, 0, rect.Width, ActualHeight)));
				drawingContext.PushTransform(new TranslateTransform(rect.Left, 0));
				drawingContext.DrawText(formattedText, new Point(2, 2));
				drawingContext.Pop();
				drawingContext.Pop();
			}
		}

		long GetInitalOffset()
		{
			foreach (var section in _view.Source.FindSections(0, 0))
			{
				return section.RealStart;
			}

			return 0;
		}

		static long RoundToLastMultiple(long value, long step)
			=> (value / step) * step;

		readonly TimelineControlView _view;
	}
}
