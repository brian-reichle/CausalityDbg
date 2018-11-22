// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CausalityDbg.Main
{
	abstract class ViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected void SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
			where T : IEquatable<T>
		{
			var match = (field == null) ? (value == null) : field.Equals(value);

			if (!match)
			{
				field = value;
				OnPropertyChanged(propertyName);
			}
		}

		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
