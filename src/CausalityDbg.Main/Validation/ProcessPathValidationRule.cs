// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Globalization;
using System.IO;
using System.Windows.Controls;

namespace CausalityDbg.Main
{
	sealed class ProcessPathValidationRule : ValidationRule
	{
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			var path = (string)value;

			if (!File.Exists(path))
			{
				return new ValidationResult(false, "File not found.");
			}

			return ValidationResult.ValidResult;
		}
	}
}
