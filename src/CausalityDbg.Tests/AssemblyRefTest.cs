// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using CausalityDbg.Configuration;
using NUnit.Framework;

namespace CausalityDbg.Tests
{
	[TestFixture]
	public class AssemblyRefTest
	{
		[TestCaseSource(nameof(AssemblyReferences))]
		public void Parse(string name)
		{
			var tupple = ReadTupple(name);
			var aref = AssemblyRef.Parse(tupple.Key);
			var actual = Format(aref);

			try
			{
				Assert.That(actual, Is.EqualTo(tupple.Value));
			}
			catch
			{
				Trace.Write(actual);
				throw;
			}
		}

		#region Implementation

		static TestCaseData[] AssemblyReferences()
		{
			const string prefix = "CausalityDbg.Tests.TestFiles.AssemblyReferences.";
			const string suffix = ".txt";
			return TestHelper.GetResourceBasedTests(prefix, suffix);
		}

		static string Format(AssemblyRef assembly)
		{
			if (assembly == null)
			{
				return null;
			}

			var builder = new StringBuilder();

			builder.Append("Full Name:        ");
			builder.AppendLine(assembly.FullyQualifiedName);
			builder.Append("Name:             ");
			builder.AppendLine(assembly.Name);

			if ((assembly.Flags & AssemblyRefFlags.VersionMask) != 0)
			{
				var version = assembly.Version;
				var order = 1 + (int)(assembly.Flags & AssemblyRefFlags.VersionMask);

				builder.Append("Version:          ");
				builder.AppendLine(order == 4 ? version.ToString() : version.ToString(order));
			}

			if ((assembly.Flags & AssemblyRefFlags.HasCulture) != 0)
			{
				builder.Append("Culture:          ");
				AppendStringValue(builder, assembly.Culture);
				builder.AppendLine();
			}

			if ((assembly.Flags & AssemblyRefFlags.HasProcessorArch) != 0)
			{
				builder.Append("Processor Arch:   ");
				builder.AppendLine(assembly.ProcessorArch.ToString());
			}

			if ((assembly.Flags & AssemblyRefFlags.HasPublicKeyToken) != 0)
			{
				builder.Append("Public Key Token: ");
				builder.AppendLine(assembly.PublicKeyToken.ToString("x16", CultureInfo.InvariantCulture));
			}

			return builder.ToString();
		}

		static void AppendStringValue(StringBuilder builder, string value)
		{
			if (value == null)
			{
				builder.Append("<null>");
			}
			else
			{
				builder.Append(value);
			}
		}

		static string GetResource(string name)
		{
			using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
			using var reader = new StreamReader(stream);
			return reader.ReadToEnd();
		}

		static KeyValuePair<string, string> ReadTupple(string name)
		{
			var source = GetResource(name);
			var match = regex.Match(source);

			if (!match.Success)
			{
				throw new ArgumentException("Name references invalid resource.", nameof(name));
			}

			return new KeyValuePair<string, string>(
				source.Substring(0, match.Index),
				source.Substring(match.Index + match.Length + 1));
		}

		static readonly Regex regex = new Regex(@"(\r|\r?\n)--+(\r|\r?\n)");

		#endregion
	}
}
