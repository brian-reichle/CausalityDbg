// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CausalityDbg.Configuration
{
	[DebuggerDisplay("ConfigAssembly: {FullyQualifiedName}")]
	public sealed class AssemblyRef
	{
		const string Neutral = "neutral";

		public static AssemblyRef Parse(string fullyQualifiedName)
		{
			var match = _regex.Match(fullyQualifiedName);

			if (!match.Success) throw new ArgumentException("Invalid assembly reference format", nameof(fullyQualifiedName));

			var nameGroup = match.Groups["Name"];
			var cultureGroup = match.Groups["Culture"];
			var archGroup = match.Groups["Arch"];
			var tokenGroup = match.Groups["Token"];
			var majorGroup = match.Groups["Major"];

			var flags = AssemblyRefFlags.None;
			var name = nameGroup.Value;
			Version version = null;
			string culture = null;
			var arch = ProcessorArchitecture.None;
			var publicKeyToken = 0L;

			if (majorGroup.Success)
			{
				var minorGroup = match.Groups["Minor"];

				var revisionGroup = match.Groups["Revision"];

				if (!revisionGroup.Success)
				{
					flags = AssemblyRefFlags.MajorMinorVersion;
					version = new Version(
						int.Parse(majorGroup.Value, CultureInfo.InvariantCulture),
						int.Parse(minorGroup.Value, CultureInfo.InvariantCulture));
				}
				else
				{
					var buildGroup = match.Groups["Build"];

					if (!buildGroup.Success)
					{
						flags = AssemblyRefFlags.ToRevisionVersion;
						version = new Version(
							int.Parse(majorGroup.Value, CultureInfo.InvariantCulture),
							int.Parse(minorGroup.Value, CultureInfo.InvariantCulture),
							int.Parse(revisionGroup.Value, CultureInfo.InvariantCulture));
					}
					else
					{
						flags = AssemblyRefFlags.FullVersion;
						version = new Version(
							int.Parse(majorGroup.Value, CultureInfo.InvariantCulture),
							int.Parse(minorGroup.Value, CultureInfo.InvariantCulture),
							int.Parse(revisionGroup.Value, CultureInfo.InvariantCulture),
							int.Parse(buildGroup.Value, CultureInfo.InvariantCulture));
					}
				}
			}

			if (cultureGroup.Success)
			{
				culture = cultureGroup.Value;

				if (culture == Neutral)
				{
					culture = null;
				}

				flags |= AssemblyRefFlags.HasCulture;
			}

			if (archGroup.Success)
			{
				arch = (ProcessorArchitecture)Enum.Parse(typeof(ProcessorArchitecture), archGroup.Value, true);
				flags |= AssemblyRefFlags.HasProcessorArch;
			}

			if (tokenGroup.Success)
			{
				flags |= AssemblyRefFlags.HasPublicKeyToken;
				publicKeyToken = ReadToken(fullyQualifiedName, tokenGroup.Index);
			}

			return new AssemblyRef(
				fullyQualifiedName,
				flags,
				name,
				version,
				culture,
				arch,
				publicKeyToken);
		}

		AssemblyRef(
			string fullyQualifiedName,
			AssemblyRefFlags flags,
			string name,
			Version version,
			string culture,
			ProcessorArchitecture processorArch,
			long publicKeyToken)
		{
			FullyQualifiedName = fullyQualifiedName;
			Flags = flags;
			Name = name;
			Version = version;
			Culture = culture;
			ProcessorArch = processorArch;
			PublicKeyToken = publicKeyToken;
		}

		public string FullyQualifiedName { get; }
		public AssemblyRefFlags Flags { get; }
		public string Name { get; }
		public Version Version { get; }
		public string Culture { get; }
		public ProcessorArchitecture ProcessorArch { get; }
		public long PublicKeyToken { get; }

		static long ReadToken(string str, int start)
		{
			ulong result = 0;

			var end = start + 16;

			for (var n = start; n < end; n++)
			{
				result = (result << 4) | Nibble(str[n]);
			}

			return unchecked((long)result);
		}

		static uint Nibble(char c)
		{
			unchecked
			{
				if (c >= '0' && c <= '9')
				{
					return (uint)c - '0';
				}
				else if (c >= 'A' && c <= 'F')
				{
					return 10u + c - 'A';
				}
				else if (c >= 'a' && c <= 'f')
				{
					return 10u + c - 'a';
				}
				else
				{
					throw new ArgumentException("Invalid char.", nameof(c));
				}
			}
		}

		static readonly Regex _regex = new Regex(
			@"^(?<Name>[\p{L}\p{N}.]+)" +
			@"(,\s*(" +
				"Version=(?<Major>[0-9]+).(?<Minor>[0-9]+)(.(?<Revision>[0-9]+)(.(?<Build>[0-9]+))?)?" +
				"|" +
				"Culture=(?<Culture>[a-z0-9_-]+)" +
				"|" +
				"ProcessorArchitecture=(?<Arch>[a-z0-9_]+)" +
				"|" +
				"PublicKeyToken=(?<Token>[0-9a-z]+)" +
			"))*$",
			RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
	}
}
