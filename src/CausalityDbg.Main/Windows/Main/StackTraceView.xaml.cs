// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CausalityDbg.Core;

namespace CausalityDbg.Main
{
	partial class StackTraceView : UserControl
	{
		public StackTraceView()
		{
			InitializeComponent();
		}

		void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			var frameWrappers = new FrameWrapper[stackTraceListBox.SelectedItems.Count];
			stackTraceListBox.SelectedItems.CopyTo(frameWrappers, 0);
			Array.Sort(frameWrappers, (f1, f2) => f1.Index.CompareTo(f2.Index));

			var builder = new StringBuilder();

			foreach (var wrapper in frameWrappers)
			{
				builder.Append(wrapper.Category != null ? "> " : "  ");
				AppendFrame(builder, wrapper.Frame);
				builder.AppendLine();
			}

			Clipboard.SetText(builder.ToString());
		}

		static void AppendFrame(StringBuilder builder, FrameData frame)
		{
			switch (frame)
			{
				case FrameILData ilFrame:
					AppendFrame(builder, ilFrame);
					return;

				case FrameInternalData internalFrame:
					AppendFrame(builder, internalFrame);
					break;

				default:
					builder.Append("???");
					break;
			}
		}

		static void AppendFrame(StringBuilder builder, FrameILData frame)
		{
			builder.Append(frame.ModuleName);
			builder.Append('!');
			builder.Append(frame.FrameText);
		}

		static void AppendFrame(StringBuilder builder, FrameInternalData frame)
		{
			builder.Append(frame.Text);
		}
	}
}
