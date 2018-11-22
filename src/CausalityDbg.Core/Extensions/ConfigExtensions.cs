// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using CausalityDbg.Core.CorDebugApi;
using CausalityDbg.Core.MetaDataApi;

namespace CausalityDbg.Core
{
	static class ConfigExtensions
	{
		public static ConfigAssembly FindAssembly(this Config config, ICorDebugModule module)
		{
			var aImport = module.GetMetaDataAssemblyImport();
			var token = aImport.GetAssemblyFromScope();

			if (token.IsNil)
			{
				Marshal.ReleaseComObject(aImport);
				var assembly = module.GetAssembly();
				aImport = assembly.GetMetaDataAssemblyImport(out token);
				Marshal.ReleaseComObject(assembly);

				if (aImport == null)
				{
					return null;
				}
			}

			var aMetadata = default(ASSEMBLYMETADATA);
			long? publicKeyToken = null;

			aImport.GetAssemblyProps(
				token,
				out var publicKeyPtr,
				out var publicKeySize,
				IntPtr.Zero,
				null,
				0,
				out var size,
				IntPtr.Zero,
				IntPtr.Zero);

			var buffer = new char[size];

			var pMetadata = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(ASSEMBLYMETADATA)));
			Marshal.StructureToPtr(aMetadata, pMetadata, false);

			try
			{
				aImport.GetAssemblyProps(
					token,
					out publicKeyPtr,
					out publicKeySize,
					IntPtr.Zero,
					buffer,
					buffer.Length,
					out size,
					pMetadata,
					IntPtr.Zero);

				aMetadata = (ASSEMBLYMETADATA)Marshal.PtrToStructure(pMetadata, typeof(ASSEMBLYMETADATA));
			}
			finally
			{
				Marshal.FreeCoTaskMem(pMetadata);
			}

			var name = new string(buffer, 0, size - 1);

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
			for (var i = 0; i < metadata.ulProcessor; i++)
			{
				var supported = Marshal.ReadInt32(metadata.rdwProcessor, i << 2);

				if (supported == (int)arch)
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
