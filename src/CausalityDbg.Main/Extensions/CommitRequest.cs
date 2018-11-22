// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace CausalityDbg.Main
{
	static class CommitRequest
	{
		public static readonly RoutedEvent CommitRequestEvent = EventManager.RegisterRoutedEvent(
			nameof(CommitRequest),
			RoutingStrategy.Bubble,
			typeof(RoutedEventHandler),
			typeof(CommitRequest));

		static CommitRequest()
		{
			EventManager.RegisterClassHandler(typeof(TextBox), CommitRequestEvent, (RoutedEventHandler)CommitRequest_TextBox);
		}

		public static void Commit(UIElement scope)
		{
			if (InputManager.Current.PrimaryKeyboardDevice.FocusedElement is UIElement element &&
				(scope == null || scope.IsAncestorOf(element)))
			{
				CommitControl(element);
			}
		}

		public static void CommitControl(UIElement element)
		{
			if (element == null) throw new ArgumentNullException(nameof(element));

			element.RaiseEvent(new RoutedEventArgs(CommitRequestEvent));
		}

		static void CommitRequest_TextBox(object sender, RoutedEventArgs e)
		{
			var textBox = (TextBox)sender;
			var bindingExpression = BindingOperations.GetBindingExpressionBase(textBox, TextBox.TextProperty);

			if (bindingExpression != null)
			{
				bindingExpression.UpdateSource();
			}
		}
	}
}
