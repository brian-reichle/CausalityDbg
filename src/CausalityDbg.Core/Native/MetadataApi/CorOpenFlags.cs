// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;

namespace CausalityDbg.Core.MetaDataApi
{
	[Flags]
	enum CorOpenFlags
	{
		OfRead = 0x00000000,
		OfWrite = 0x00000001,
		OfReadWriteMask = 0x00000001,
		OfCopyMemory = 0x00000002,
		OfCacheImage = 0x00000004,
		OfManifestMetadata = 0x00000008,
		OfReadOnly = 0x00000010,
		OfTakeOwnership = 0x00000020,
		OfNoTypeLib = 0x00000080,
		OfNoTransform = 0x00001000,
	}
}
