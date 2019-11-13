using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CausalityDbg.Configuration;

namespace CausalityDbg.Main
{
	class CategoryMarker : Control
	{
		public static readonly DependencyProperty CategoryProperty = DependencyProperty.Register(
			nameof(Category),
			typeof(ConfigCategory),
			typeof(CategoryMarker),
			new PropertyMetadata(null, OnCategoryChanged));

		static readonly DependencyPropertyKey FillPropertyKey = DependencyProperty.RegisterReadOnly(
			nameof(Fill),
			typeof(Brush),
			typeof(CategoryMarker),
			new PropertyMetadata(null));

		public static readonly DependencyProperty FillProperty = FillPropertyKey.DependencyProperty;

		static readonly DependencyPropertyKey StrokePropertyKey = DependencyProperty.RegisterReadOnly(
			nameof(Stroke),
			typeof(Brush),
			typeof(CategoryMarker),
			new PropertyMetadata(null));

		public static readonly DependencyProperty StrokeProperty = StrokePropertyKey.DependencyProperty;

		static CategoryMarker()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(CategoryMarker), new FrameworkPropertyMetadata(typeof(CategoryMarker)));
		}

		public ConfigCategory Category
		{
			get => (ConfigCategory)GetValue(CategoryProperty);
			set => SetValue(CategoryProperty, value);
		}

		public Brush Fill
		{
			get => (Brush)GetValue(FillProperty);
			private set => SetValue(FillPropertyKey, value);
		}

		public Brush Stroke
		{
			get => (Brush)GetValue(StrokeProperty);
			private set => SetValue(StrokePropertyKey, value);
		}

		static void OnCategoryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var category = (ConfigCategory)e.NewValue;
			var control = (CategoryMarker)d;

			if (category == null)
			{
				control.Fill = null;
				control.Stroke = null;
			}
			else
			{
				control.Fill = TimelineColors.GetBrush(category.BackgroundColor);
				control.Stroke = TimelineColors.GetBrush(category.ForegroundColor);
			}
		}
	}
}
