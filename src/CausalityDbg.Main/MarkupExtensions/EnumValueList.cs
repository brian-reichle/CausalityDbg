// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Windows.Markup;

namespace CausalityDbg.Main
{
	[MarkupExtensionReturnType(typeof(Array))]
	sealed class EnumValueList : MarkupExtension
	{
		public EnumValueList(Type enumType)
		{
			EnumType = enumType;
		}

		[ConstructorArgument("enumType")]
		public Type EnumType { get; }

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			if (EnumType == null || !EnumType.IsEnum)
			{
				return null;
			}

			return Enum.GetValues(EnumType);
		}
	}
}
