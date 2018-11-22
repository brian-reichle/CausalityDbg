// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Globalization;
using System.Windows.Controls;

namespace CausalityDbg.Main
{
	sealed class MacroValidationRule : ValidationRule
	{
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			try
			{
				if (value != null)
				{
					foreach (var macro in TemplateProcessor.GetMacros((string)value))
					{
						if ((ToolLauncher.MacroFlags(macro) & ToolFlags.Invalid) != 0)
						{
							return new ValidationResult(false, string.Format(cultureInfo, "'{0}' is not a valid macro.", macro));
						}
					}
				}
			}
			catch (FormatException)
			{
				return new ValidationResult(false, "Invalid template.");
			}

			return ValidationResult.ValidResult;
		}
	}
}
