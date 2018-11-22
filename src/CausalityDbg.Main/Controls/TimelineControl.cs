// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace CausalityDbg.Main
{
	[TemplatePart(Name = "PART_ScrollViewer", Type = typeof(ScrollViewer))]
	[TemplatePart(Name = "PART_ToolTip", Type = typeof(Popup))]
	[TemplatePart(Name = "PART_Scale", Type = typeof(AdornerDecorator))]
	sealed class TimelineControl : Control
	{
		public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
			nameof(Source),
			typeof(IDataProvider),
			typeof(TimelineControl),
			new FrameworkPropertyMetadata(
				null,
				FrameworkPropertyMetadataOptions.AffectsRender,
				(d, e) => ((TimelineControl)d).OnSourceChanged(e)));

		public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register(
			nameof(Scale),
			typeof(int),
			typeof(TimelineControl),
			new FrameworkPropertyMetadata(
				5,
				FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
				(d, e) => ((TimelineControl)d).OnScaleChanged(e)));

		public static readonly DependencyProperty FadeOutProperty = DependencyProperty.Register(
			nameof(FadeOut),
			typeof(double),
			typeof(TimelineControl),
			new FrameworkPropertyMetadata(
				5d,
				FrameworkPropertyMetadataOptions.AffectsRender,
				(d, e) => ((TimelineControl)d).OnFadeOutChanged()));

		public static readonly DependencyProperty SelectionProperty = DependencyProperty.Register(
			nameof(Selection),
			typeof(IEventScope),
			typeof(TimelineControl),
			new FrameworkPropertyMetadata(
				null,
				FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
				(d, e) => ((TimelineControl)d).OnSelectionChanged(e)));

		public static readonly DependencyProperty EventGlyphProperty = DependencyProperty.Register(
			nameof(EventGlyph),
			typeof(Geometry),
			typeof(TimelineControl),
			new FrameworkPropertyMetadata(
				null,
				FrameworkPropertyMetadataOptions.AffectsRender,
				(d, e) => ((TimelineControl)d).OnEventGlyphChanged(e)));

		public static readonly DependencyProperty TraceModeProperty = DependencyProperty.Register(
			nameof(TraceMode),
			typeof(TraceMode),
			typeof(TimelineControl),
			new FrameworkPropertyMetadata(
				TraceMode.Direct,
				FrameworkPropertyMetadataOptions.AffectsRender,
				(d, e) => ((TimelineControl)d).OnTraceModeChanged(e)));

		public static readonly RoutedCommand ZoomIn = new RoutedCommand(nameof(ZoomIn), typeof(TimelineControl));
		public static readonly RoutedCommand ZoomOut = new RoutedCommand(nameof(ZoomOut), typeof(TimelineControl));

		static TimelineControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TimelineControl), new FrameworkPropertyMetadata(typeof(TimelineControl)));
			PaddingProperty.OverrideMetadata(typeof(TimelineControl), new FrameworkPropertyMetadata((d, e) => ((TimelineControl)d).OnPaddingChanged(e)));

			CommandManager.RegisterClassCommandBinding(typeof(TimelineControl), new CommandBinding(ZoomIn, OnZoomInExecuted));
			CommandManager.RegisterClassCommandBinding(typeof(TimelineControl), new CommandBinding(ZoomOut, OnZoomOutExecuted));
		}

		public IDataProvider Source
		{
			get => (IDataProvider)GetValue(SourceProperty);
			set => SetValue(SourceProperty, value);
		}

		public IEventScope Selection
		{
			get => (IEventScope)GetValue(SelectionProperty);
			set => SetValue(SelectionProperty, value);
		}

		public TraceMode TraceMode
		{
			get => (TraceMode)GetValue(TraceModeProperty);
			set => SetValue(TraceModeProperty, value);
		}

		public int Scale
		{
			get => (int)GetValue(ScaleProperty);
			set => SetValue(ScaleProperty, value);
		}

		public double FadeOut
		{
			get => (double)GetValue(FadeOutProperty);
			set => SetValue(FadeOutProperty, value);
		}

		public Geometry EventGlyph
		{
			get => (Geometry)GetValue(EventGlyphProperty);
			set => SetValue(EventGlyphProperty, value);
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			var viewer = (ScrollViewer)GetTemplateChild("PART_ScrollViewer");
			var decorator = (AdornerDecorator)GetTemplateChild("PART_Scale");

			_toolTip = (Popup)GetTemplateChild("PART_ToolTip");

			if (viewer != null && decorator != null)
			{
				_timelineView = new TimelineControlView();
				_timelineView.Scale = TranslateScale((int)Scale);
				_timelineView.FadeOut = FadeOut;
				_timelineView.Padding = Padding;
				_timelineView.EventGlyph = EventGlyph;
				_timelineView.Selection = Selection;
				_timelineView.TraceMode = TraceMode;
				_timelineView.MouseDown += TimelineView_MouseDown;
				_timelineView.MouseMove += TimelineView_MouseMove;
				_timelineView.MouseLeave += TimelineView_MouseLeave;

				viewer.Content = _timelineView;
				viewer.CanContentScroll = true;

				decorator.Child = _timelineView.ScaleControl;

				if (Source != null)
				{
					_timelineView.Source = Source;
				}
			}

			if (_toolTip != null)
			{
				_toolTip.Placement = PlacementMode.Relative;
			}
		}

		static void OnZoomInExecuted(object sender, ExecutedRoutedEventArgs zoomIn)
		{
			var control = (TimelineControl)sender;
			var tmp = control.Scale + 1;
			if (tmp > 50) tmp = 50;
			control.Scale = tmp;
		}

		static void OnZoomOutExecuted(object sender, ExecutedRoutedEventArgs zoomOut)
		{
			var control = (TimelineControl)sender;
			var tmp = control.Scale - 1;
			if (tmp < 1) tmp = 1;
			control.Scale = tmp;
		}

		void TimelineView_MouseDown(object sender, MouseButtonEventArgs e)
		{
			var view = (TimelineControlView)sender;
			var point = e.GetPosition(view);
			var eventScope = view.HitTest(point);
			SetValue(SelectionProperty, eventScope);
		}

		void TimelineView_MouseMove(object sender, MouseEventArgs e)
		{
			if (_toolTip != null)
			{
				var view = (TimelineControlView)sender;
				var point = e.GetPosition(view);
				var eventScope = view.HitTest(point);

				if (eventScope == null)
				{
					_toolTip.IsOpen = false;
					_toolTip.DataContext = null;
				}
				else
				{
					if (_toolTip.DataContext != eventScope)
					{
						_toolTip.DataContext = eventScope;
						_toolTip.IsOpen = true;
					}

					var transform = view.TransformToAncestor(this);
					var localPoint = transform.Transform(point);

					if (_toolTip.HorizontalOffset != localPoint.X || _toolTip.VerticalOffset != localPoint.Y)
					{
						_toolTip.HorizontalOffset = localPoint.X + 16;
						_toolTip.VerticalOffset = localPoint.Y + 16;
					}
				}
			}
		}

		void TimelineView_MouseLeave(object sender, MouseEventArgs e)
		{
			if (_toolTip != null)
			{
				_toolTip.IsOpen = false;
				_toolTip.DataContext = null;
			}
		}

		void OnSourceChanged(DependencyPropertyChangedEventArgs e)
		{
			if (_timelineView != null)
			{
				_timelineView.Source = (IDataProvider)e.NewValue;
			}
		}

		void OnScaleChanged(DependencyPropertyChangedEventArgs e)
		{
			if (_timelineView != null)
			{
				_timelineView.Scale = TranslateScale((int)e.NewValue);
			}
		}

		void OnFadeOutChanged()
		{
			if (_timelineView != null)
			{
				_timelineView.FadeOut = FadeOut;
			}
		}

		void OnPaddingChanged(DependencyPropertyChangedEventArgs e)
		{
			if (_timelineView != null)
			{
				_timelineView.Padding = (Thickness)e.NewValue;
			}
		}

		void OnEventGlyphChanged(DependencyPropertyChangedEventArgs e)
		{
			if (_timelineView != null)
			{
				_timelineView.EventGlyph = (Geometry)e.NewValue;
			}
		}

		void OnSelectionChanged(DependencyPropertyChangedEventArgs e)
		{
			if (_timelineView != null)
			{
				_timelineView.Selection = e.NewValue as IEventScope;
			}
		}

		void OnTraceModeChanged(DependencyPropertyChangedEventArgs e)
		{
			if (_timelineView != null)
			{
				_timelineView.TraceMode = (TraceMode)e.NewValue;
			}
		}

		static double TranslateScale(int value)
		{
			const double A = 10d / 62208d;
			const double B = 1.2;
			return A * Math.Pow(B, value);
		}

		TimelineControlView _timelineView;
		Popup _toolTip;
	}
}
