// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using CausalityDbg.DataStore;

namespace CausalityDbg.Main
{
	sealed class TimelineControlView : FrameworkElement, IScrollInfo
	{
		const double BarHeight = 10;
		const double BandTopPadding = 10;
		const double BandBottomPadding = 10;

		const double StepX = 50;
		const double StepY = 50;

		public TimelineControlView()
		{
			_bandOffsets = new Dictionary<Band, double>();
			ScaleControl = new TimelineControlScale(this);
		}

		public IDataProvider Source
		{
			[DebuggerStepThrough]
			get => _source;
			set
			{
				if (_source != null)
				{
					_source.DataChanged -= Source_DataChanged;
				}

				_source = value;

				if (value != null)
				{
					_source.DataChanged += Source_DataChanged;
				}

				InvalidateMeasure();
				InvalidateVisual();
				InvalidateOwner();
			}
		}

		public double Scale
		{
			[DebuggerStepThrough]
			get => _scale;
			set
			{
				_scale = value;
				InvalidateMeasure();
				InvalidateVisual();
				InvalidateOwner();
			}
		}

		public double FadeOut
		{
			[DebuggerStepThrough]
			get => _fadeOut;
			set
			{
				_fadeOut = value;
				InvalidateVisual();
			}
		}

		public Thickness Padding
		{
			[DebuggerStepThrough]
			get => _padding;
			set
			{
				_padding = value;
				InvalidateMeasure();
				InvalidateVisual();
				InvalidateOwner();
			}
		}

		public Geometry EventGlyph
		{
			[DebuggerStepThrough]
			get => _eventGlyph;
			set
			{
				_eventGlyph = value;
				InvalidateVisual();
			}
		}

		public IEventScope HitTest(Point point)
		{
			if (_source == null)
			{
				return null;
			}
			else
			{
				var transform = BuildTransform();
				var invertedTransform = transform.Inverse;
				var p = invertedTransform.Transform(point);

				Band band = null;
				var depth = 0;

				foreach (var pair in _bandOffsets)
				{
					var top = pair.Value;
					var bottom = top + (pair.Key.MaxDepth + 1) * BarHeight + BandTopPadding + BandBottomPadding;

					if (top < p.Y && bottom > p.Y)
					{
						band = pair.Key;
						depth = (int)Math.Floor((p.Y - pair.Value - BandTopPadding) / BarHeight);
						break;
					}
				}

				IEventScope result = FindEvent(transform, invertedTransform, band, point);

				if (result != null)
				{
					return result;
				}

				return FindScope(invertedTransform, band, depth, point.X);
			}
		}

		public IEventScope Selection
		{
			[DebuggerStepThrough]
			get => _eventScope;
			set
			{
				_eventScope = value;
				InvalidateVisual();
			}
		}

		public TraceMode TraceMode
		{
			[DebuggerStepThrough]
			get => _traceMode;
			set
			{
				_traceMode = value;
				InvalidateVisual();
			}
		}

		public TimelineControlScale ScaleControl { get; }

		public Transform BuildTransform() => BuildTransform(Padding, HorizontalOffset, VerticalOffset, _scale);

		protected override Size MeasureOverride(Size availableSize)
		{
			_bandOffsets.Clear();

			if (_source == null)
			{
				ExtentHeight = 30;
				ExtentWidth = 30;
			}
			else
			{
				var height = Padding.Top;

				foreach (var band in _source.Bands)
				{
					_bandOffsets.Add(band, height);
					height += BandTopPadding + BandBottomPadding + (band.MaxDepth + 1) * BarHeight;
				}

				var width = Padding.Left + Padding.Right + _source.UpperBound * Scale;

				width = Math.Max(availableSize.Width, width);
				height = Math.Max(availableSize.Height, height + Padding.Bottom);

				if (ExtentHeight != height || ExtentWidth != width)
				{
					ExtentWidth = width;
					ExtentHeight = height;
					InvalidateOwner();
				}
			}

			return new Size(
				double.IsPositiveInfinity(availableSize.Width) ? ExtentWidth : availableSize.Width,
				double.IsPositiveInfinity(availableSize.Height) ? ExtentHeight : availableSize.Height);
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			if (finalSize.Height != ViewportHeight || finalSize.Width != ViewportWidth)
			{
				ViewportHeight = finalSize.Height;
				ViewportWidth = finalSize.Width;
				InvalidateOwner();
			}

			if (_source != null)
			{
				var offset = _timestampFoci * Scale + Padding.Left - _viewportFoci;
				HorizontalOffset = ConstrainOffset(offset, ViewportWidth, ExtentWidth);
				_viewportFoci = ViewportWidth / 2;
				TimestampFociFromViewport();
			}
			else
			{
				_viewportFoci = 0;
				_timestampFoci = 0;
			}

			ScaleControl.InvalidateVisual();
			return finalSize;
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			var viewRect = new Rect(-3, 0, ViewportWidth + 6, ViewportHeight);

			base.OnRender(drawingContext);
			drawingContext.DrawRectangle(Brushes.Transparent, null, viewRect);

			if (_source != null && _bandOffsets.Count > 0)
			{
				var transform = BuildTransform();
				var inverseTransform = transform.Inverse;

				var timestampBounds = inverseTransform.TransformBounds(viewRect);
				var lowerBound = (long)Math.Floor(timestampBounds.Left);
				var upperBound = (long)Math.Ceiling(timestampBounds.Right);

				DrawBandSeparators(drawingContext, transform);
				PaintSection(drawingContext, transform, lowerBound, upperBound);

				TimelineSection prevSection = null;

				foreach (var section in _source.FindSections(lowerBound, upperBound))
				{
					if (prevSection != null)
					{
						var rect = new Rect(new Point(prevSection.ViewEnd, 0), new Point(section.ViewStart, 0));
						rect = transform.TransformBounds(rect);
						rect = new Rect(rect.Left, 0, rect.Width, ViewportHeight);
						drawingContext.DrawRectangle(null, TimelineColors.CollapsePen, rect);
					}

					prevSection = section;
				}
			}
		}

		void PaintSection(DrawingContext drawingContext, Transform transform, long lowerBound, long upperBound)
		{
			foreach (var scope in _source.FindScopes(lowerBound, upperBound))
			{
				var rect = transform.TransformBounds(GetScopeRect(scope, lowerBound, upperBound));
				var category = scope.Item.Category;
				var needsPop = false;

				if (!scope.ToTimestamp.HasValue && FadeOut > 0)
				{
					var alphaBrush = new LinearGradientBrush(
						Colors.White,
						Colors.Transparent,
						new Point(rect.Right, 0),
						new Point(rect.Right + FadeOut, 0));

					alphaBrush.MappingMode = BrushMappingMode.Absolute;
					alphaBrush.SpreadMethod = GradientSpreadMethod.Pad;

					drawingContext.PushOpacityMask(alphaBrush);
					rect = new Rect(rect.Left, rect.Top, rect.Width + FadeOut, rect.Height);
					needsPop = true;
				}

				drawingContext.DrawRectangle(
					TimelineColors.GetBrush(category.BackgroundColor),
					TimelineColors.GetPen(category.ForegroundColor),
					rect);

				if (needsPop)
				{
					drawingContext.Pop();
				}
			}

			foreach (var @event in _source.FindEvents(lowerBound, upperBound))
			{
				var tip = transform.Transform(GetEventOrigin(@event));
				var category = @event.Item.Category;

				drawingContext.PushTransform(new TranslateTransform(tip.X, tip.Y));

				drawingContext.DrawGeometry(
					TimelineColors.GetBrush(category.BackgroundColor),
					TimelineColors.GetPen(category.ForegroundColor),
					EventGlyph);

				drawingContext.Pop();
			}

			PaintSelectionRelationships(drawingContext, transform);
		}

		void PaintSelectionRelationships(DrawingContext drawingContext, Transform transform)
		{
			if (TraceMode == TraceMode.Direct)
			{
				switch (Selection)
				{
					case Event @event:
						PaintEventRelationships(drawingContext, transform, @event);
						break;

					case Scope scope:
						PaintScopeRelationships(drawingContext, transform, scope);
						break;
				}
			}
			else if (TraceMode == TraceMode.FullTrace)
			{
				var es = Selection;

				while (es != null)
				{
					var scope = es as Scope ?? es.Host;

					while (scope != null && scope.Trigger == null)
					{
						scope = scope.Host;
					}

					if (scope == null)
					{
						break;
					}

					PaintScopeRelationships(drawingContext, transform, scope);

					es = scope.Trigger;
				}
			}
		}

		void PaintEventRelationships(DrawingContext drawingContext, Transform transform, Event @event)
		{
			foreach (var scope in _source.FindScopes(@event.Timestamp - 1, _source.UpperBound))
			{
				if (scope.Trigger == @event)
				{
					PaintRelationship(drawingContext, transform, @event, scope);
				}
			}
		}

		void PaintScopeRelationships(DrawingContext drawingContext, Transform transform, Scope scope)
		{
			if (scope.Trigger != null)
			{
				PaintRelationship(drawingContext, transform, scope.Trigger, scope);
			}
		}

		void PaintRelationship(DrawingContext drawingContext, Transform transform, Event @event, Scope scope)
		{
			var eventTip = EventAnchor(transform.Transform(GetEventOrigin(@event)));
			var scopeAnchor = ScopeAnchor(transform.TransformBounds(GetScopeRect(scope)));

			drawingContext.DrawLine(TimelineColors.RelationshipPen1, eventTip, scopeAnchor);
			drawingContext.DrawLine(TimelineColors.RelationshipPen2, eventTip, scopeAnchor);
		}

		Event FindEvent(GeneralTransform transform, GeneralTransform invertedTransform, Band band, Point point)
		{
			var glyphBounds = EventGlyph.Bounds;
			var bounds = invertedTransform.TransformBounds(new Rect(point.X - glyphBounds.Right, 0, glyphBounds.Width, 0));
			var lowerBound = (long)Math.Floor(bounds.Left);
			var upperBound = (long)Math.Ceiling(bounds.Right);

			Event result = null;

			foreach (var @event in _source.FindEvents(lowerBound, upperBound))
			{
				if (@event.Band != band) continue;

				var tip = transform.Transform(GetEventOrigin(@event));
				tip = new Point(point.X - tip.X, point.Y - tip.Y);

				var category = @event.Item.Category;

				if (EventGlyph.FillContains(tip) || EventGlyph.StrokeContains(TimelineColors.GetPen(category.ForegroundColor), tip))
				{
					result = @event;
				}
			}

			return result;
		}

		Scope FindScope(GeneralTransform invertedTransform, Band band, int depth, double x)
		{
			var bounds = invertedTransform.TransformBounds(new Rect(Math.Floor(x), 0, 1, 0));
			var lowerBound = (long)Math.Floor(bounds.Left);
			var upperBound = (long)Math.Ceiling(bounds.Right);

			foreach (var scope in _source.FindScopes(lowerBound, upperBound))
			{
				if (scope.Band != band) continue;
				if (scope.Depth != depth) continue;
				return scope;
			}

			return null;
		}

		Rect GetScopeRect(Scope scope)
		{
			var lower = scope.FromTimestamp;
			var upper = scope.ToTimestamp.GetValueOrDefault(_source.UpperBound);

			return new Rect(
				lower,
				_bandOffsets[scope.Band] + BandTopPadding + scope.Depth * BarHeight,
				upper - lower,
				BarHeight);
		}

		Rect GetScopeRect(Scope scope, long lowerBound, long upperBound)
		{
			var lower = scope.FromTimestamp;
			var upper = scope.ToTimestamp.GetValueOrDefault(_source.UpperBound);

			if (lower < upperBound && upper > lowerBound)
			{
				if (lower < lowerBound)
				{
					lower = lowerBound;
				}

				if (upper > upperBound)
				{
					upper = upperBound;
				}
			}

			return new Rect(
				lower,
				_bandOffsets[scope.Band] + BandTopPadding + scope.Depth * BarHeight,
				upper - lower,
				BarHeight);
		}

		Point GetEventOrigin(Event @event)
		{
			return new Point(
				@event.Timestamp,
				_bandOffsets[@event.Band] + BandTopPadding + @event.Host.Depth * BarHeight);
		}

		static Point ScopeAnchor(Rect rect)
		{
			return new Point(
				rect.X + Math.Min(3, rect.Width / 2),
				rect.Y + Math.Min(3, rect.Height / 2));
		}

		Point EventAnchor(Point origin) => new Point(origin.X, origin.Y + EventGlyph.Bounds.Top);

		#region IScrollInfo Members

		public bool CanHorizontallyScroll { get; set; }
		public bool CanVerticallyScroll { get; set; }
		public ScrollViewer ScrollOwner { get; set; }

		public double ExtentHeight { get; private set; }
		public double ExtentWidth { get; private set; }
		public double VerticalOffset { get; private set; }
		public double HorizontalOffset { get; private set; }
		public double ViewportHeight { get; private set; }
		public double ViewportWidth { get; private set; }

		public void LineUp() => SetVerticalOffset(VerticalOffset - StepY);
		public void LineDown() => SetVerticalOffset(VerticalOffset + StepY);
		public void LineLeft() => SetHorizontalOffset(HorizontalOffset - StepX);
		public void LineRight() => SetHorizontalOffset(HorizontalOffset + StepX);
		public void MouseWheelUp() => SetVerticalOffset(VerticalOffset + -StepY);
		public void MouseWheelDown() => SetVerticalOffset(VerticalOffset + StepY);
		public void MouseWheelLeft() => SetHorizontalOffset(HorizontalOffset + StepX);
		public void MouseWheelRight() => SetHorizontalOffset(HorizontalOffset + -StepX);
		public void PageUp() => SetVerticalOffset(VerticalOffset - 0.9 * ViewportHeight);
		public void PageDown() => SetVerticalOffset(VerticalOffset + 0.9 * ViewportHeight);
		public void PageLeft() => SetHorizontalOffset(HorizontalOffset - 0.9 * ViewportWidth);
		public void PageRight() => SetHorizontalOffset(HorizontalOffset + 0.9 * ViewportWidth);

		public Rect MakeVisible(Visual visual, Rect rectangle)
		{
			var newRect = visual.TransformToAncestor(this).TransformBounds(rectangle);

			SetVerticalOffset(ChooseOffset(newRect.Top, newRect.Height, VerticalOffset, ViewportHeight));
			SetHorizontalOffset(ChooseOffset(newRect.Left, newRect.Width, HorizontalOffset, ViewportWidth));

			return newRect;
		}

		public void SetHorizontalOffset(double offset)
		{
			offset = ConstrainOffset(offset, ViewportWidth, ExtentWidth);

			if (HorizontalOffset != offset)
			{
				HorizontalOffset = offset;
				TimestampFociFromViewport();
				InvalidateVisual();
				InvalidateOwner();
			}
		}

		public void SetVerticalOffset(double offset)
		{
			offset = ConstrainOffset(offset, ViewportHeight, ExtentHeight);

			if (VerticalOffset != offset)
			{
				VerticalOffset = offset;
				InvalidateVisual();
				InvalidateOwner();
			}
		}

		#endregion

		static double ChooseOffset(double position, double length, double offset, double viewport)
		{
			if (position <= offset || length >= viewport)
			{
				return position;
			}

			var endAlignOffset = position + length - viewport;

			if (endAlignOffset > offset)
			{
				return endAlignOffset;
			}
			else
			{
				return offset;
			}
		}

		static double ConstrainOffset(double offset, double viewport, double extent)
		{
			if (offset < 0 || viewport > extent)
			{
				return 0;
			}
			else if (offset + viewport > extent)
			{
				return extent - viewport;
			}
			else
			{
				return offset;
			}
		}

		void Source_DataChanged(object sender, EventArgs e)
		{
			if (!_dataIsDirty)
			{
				_dataIsDirty = true;
				Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)UpdateData);
			}
		}

		void UpdateData()
		{
			_dataIsDirty = false;
			InvalidateMeasure();
			InvalidateVisual();
			InvalidateOwner();
		}

		void TimestampFociFromViewport()
		{
			_timestampFoci = (long)Math.Round(BuildTransform().Inverse.Transform(new Point(_viewportFoci, 0)).X);
		}

		void InvalidateOwner()
		{
			if (ScrollOwner != null)
			{
				ScrollOwner.InvalidateScrollInfo();
			}
		}

		void DrawBandSeparators(DrawingContext drawingContext, Transform transform)
		{
			var left = Math.Max(0, Padding.Left - HorizontalOffset);
			var right = Math.Min(ViewportWidth, ExtentWidth - HorizontalOffset - Padding.Right);

			foreach (var y in _bandOffsets.Values)
			{
				if (y != Padding.Top)
				{
					var newY = transform.Transform(new Point(0, y)).Y;
					drawingContext.DrawLine(TimelineColors.SeparatorPen, new Point(left, newY), new Point(right, newY));
				}
			}
		}

		static Transform BuildTransform(Thickness padding, double horizontalOffset, double verticalOffset, double scale)
		{
			var matrix = new Matrix();
			matrix.Scale(scale, 1);
			matrix.Translate(padding.Left - horizontalOffset, padding.Top - verticalOffset);

			var transform = new MatrixTransform(matrix);
			transform.Freeze();
			return transform;
		}

		bool _dataIsDirty;
		IDataProvider _source;
		Thickness _padding;
		Geometry _eventGlyph;
		IEventScope _eventScope;
		TraceMode _traceMode;
		double _scale;
		double _fadeOut;
		long _timestampFoci;
		double _viewportFoci;
		readonly Dictionary<Band, double> _bandOffsets;
	}
}
