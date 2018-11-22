// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace CausalityDbg.Main
{
	sealed class WheelScrollViewer : ScrollViewer
	{
		protected override void OnMouseWheel(MouseWheelEventArgs e)
		{
			if (e.Handled)
			{
				return;
			}

			var info = ScrollInfo;

			if (info == null)
			{
				return;
			}

			var modifiers = InputManager.Current.PrimaryKeyboardDevice.Modifiers;

			if ((modifiers & ModifierKeys.Shift) != 0)
			{
				ScrollVertically(info, e.Delta);
				return;
			}

			if ((modifiers & ModifierKeys.Control) != 0)
			{
				Zoom(e.Delta);
				return;
			}

			ScrollHorizontally(info, e.Delta);
		}

		static void ScrollVertically(IScrollInfo info, int delta)
		{
			if (delta < 0)
			{
				info.MouseWheelDown();
			}
			else
			{
				info.MouseWheelUp();
			}
		}

		static void ScrollHorizontally(IScrollInfo info, int delta)
		{
			if (delta < 0)
			{
				info.MouseWheelLeft();
			}
			else
			{
				info.MouseWheelRight();
			}
		}

		void Zoom(int delta)
		{
			if (delta < 0)
			{
				TimelineControl.ZoomIn.Execute(null, this);
			}
			else
			{
				TimelineControl.ZoomOut.Execute(null, this);
			}
		}
	}
}
