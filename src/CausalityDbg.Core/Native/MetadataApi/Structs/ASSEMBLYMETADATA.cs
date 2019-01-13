// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;

namespace CausalityDbg.Core.MetaDataApi
{
	[StructLayout(LayoutKind.Sequential)]
	struct ASSEMBLYMETADATA
	{
		// USHORT  usMajorVersion;
		public ushort usMajorVersion;

		// USHORT  usMinorVersion;
		public ushort usMinorVersion;

		// USHORT  usBuildNumber;
		public ushort usBuildNumber;

		// USHORT  usRevisionNumber;
		public ushort usRevisionNumber;

		// LPWSTR  szLocale;
		public string szLocale;

		// ULONG   cbLocale;
		public uint cbLocale;

		// DWORD*  rdwProcessor[];
		public IntPtr rdwProcessor;

		// ULONG   ulProcessor
		public int ulProcessor;

		// OSINFO* rOS[];
		public IntPtr rOS;

		// ULONG   ulOS;
		public uint ulOS;
	}
}
