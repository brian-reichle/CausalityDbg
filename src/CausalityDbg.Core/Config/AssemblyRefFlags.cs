// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace CausalityDbg.Core
{
	public enum AssemblyRefFlags
	{
		None = 0x00,

		MajorMinorVersion = 1,
		ToRevisionVersion = 2,
		FullVersion = 3,

		VersionMask = 0x07,

		HasCulture = 0x10,
		HasProcessorArch = 0x20,
		HasPublicKeyToken = 0x40,
	}
}
