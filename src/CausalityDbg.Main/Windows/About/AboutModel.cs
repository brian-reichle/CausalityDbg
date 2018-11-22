// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Globalization;
using System.Reflection;

namespace CausalityDbg.Main
{
	sealed class AboutModel
	{
		public AboutModel()
		{
			var assembly = Assembly.GetExecutingAssembly();
			var version = assembly.GetName().Version;

			AssemblyProduct = GetAssemblyAttribute<AssemblyProductAttribute>(assembly).Product;
			AssemblyVersion = version.ToString();
			AssemblyCopyright = GetAssemblyAttribute<AssemblyCopyrightAttribute>(assembly).Copyright;
			BuildDate = GetDateTime(version);
			BuildDateFormatted = BuildDate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
		}

		public string AssemblyProduct { get; }
		public string AssemblyVersion { get; }
		public string AssemblyCopyright { get; }
		public DateTime BuildDate { get; }
		public string BuildDateFormatted { get; }

		static DateTime GetDateTime(Version version)
		{
			return new DateTime(2000, 1, 1)
				.AddDays(version.Build)
				.AddSeconds(version.Revision << 1);
		}

		static T GetAssemblyAttribute<T>(Assembly assembly)
			where T : Attribute
		{
			return (T)Attribute.GetCustomAttribute(assembly, typeof(T));
		}
	}
}
