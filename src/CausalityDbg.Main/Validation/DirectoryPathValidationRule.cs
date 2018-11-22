// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Globalization;
using System.IO;
using System.Windows.Controls;

namespace CausalityDbg.Main
{
	sealed class DirectoryPathValidationRule : ValidationRule
	{
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			var path = (string)value;

			if (!string.IsNullOrEmpty(path) && !Directory.Exists(path))
			{
				return new ValidationResult(false, "Directory not found.");
			}

			return ValidationResult.ValidResult;
		}
	}
}
