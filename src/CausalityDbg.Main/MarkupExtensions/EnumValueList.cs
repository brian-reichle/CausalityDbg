// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Windows.Markup;

namespace CausalityDbg.Main
{
	[ContentProperty("EnumType")]
	[MarkupExtensionReturnType(typeof(Array))]
	sealed class EnumValueList : MarkupExtension
	{
		public EnumValueList()
		{
		}

		public EnumValueList(Type enumType)
		{
			EnumType = enumType;
		}

		[ConstructorArgument("enumType")]
		public Type EnumType { get; set; }

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
