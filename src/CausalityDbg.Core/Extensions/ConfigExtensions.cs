// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Reflection;
using CausalityDbg.Configuration;
using CausalityDbg.Core.CorDebugApi;
using CausalityDbg.Core.MetaDataApi;

namespace CausalityDbg.Core
{
	static class ConfigExtensions
	{
		public static ConfigAssembly FindAssembly(this Config config, ICorDebugModule module)
		{
			if (!module.GetAssemblyProps(out var name, out var aMetadata, out var publicKeyPtr, out var publicKeySize))
			{
				return null;
			}

			long? publicKeyToken = default;

			foreach (var ass in config.Assemblies)
			{
				var assRef = ass.AssemblyRef;
				if (assRef.Name != name) continue;

				if (!MatchesVersion(ref aMetadata, assRef.Version, assRef.Flags & AssemblyRefFlags.VersionMask))
				{
					continue;
				}

				if ((assRef.Flags & AssemblyRefFlags.HasProcessorArch) != 0)
				{
					if (!SupportsProcessorArch(ref aMetadata, assRef.ProcessorArch)) continue;
				}

				if ((assRef.Flags & AssemblyRefFlags.HasCulture) != 0)
				{
					if (!SupportsCulture(ref aMetadata, assRef.Culture)) continue;
				}

				if ((assRef.Flags & AssemblyRefFlags.HasPublicKeyToken) != 0)
				{
					if (!publicKeyToken.HasValue)
					{
						publicKeyToken = CryptFunctions.GetPublicKeyToken(publicKeyPtr, publicKeySize);
					}

					if (publicKeyToken.Value != assRef.PublicKeyToken) continue;
				}

				return ass;
			}

			return null;
		}

		static bool MatchesVersion(ref ASSEMBLYMETADATA metadata, Version version, AssemblyRefFlags versionLevel)
		{
			switch (versionLevel)
			{
				case AssemblyRefFlags.FullVersion:
					if (version.Build != metadata.usBuildNumber) return false;
					goto case AssemblyRefFlags.ToRevisionVersion;

				case AssemblyRefFlags.ToRevisionVersion:
					if (version.Revision != metadata.usRevisionNumber) return false;
					goto case AssemblyRefFlags.MajorMinorVersion;

				case AssemblyRefFlags.MajorMinorVersion:
					if (version.Major != metadata.usMajorVersion) return false;
					if (version.Minor != metadata.usMinorVersion) return false;
					break;
			}

			return true;
		}

		static bool SupportsProcessorArch(ref ASSEMBLYMETADATA metadata, ProcessorArchitecture arch)
		{
			var span = SpanUtils.Create<int>(metadata.rdwProcessor, metadata.ulProcessor);

			for (var i = 0; i < span.Length; i++)
			{
				if (span[i] == (int)arch)
				{
					return true;
				}
			}

			return false;
		}

		static bool SupportsCulture(ref ASSEMBLYMETADATA metadata, string culture)
		{
			return string.Equals(metadata.szLocale, culture, StringComparison.OrdinalIgnoreCase);
		}
	}
}
