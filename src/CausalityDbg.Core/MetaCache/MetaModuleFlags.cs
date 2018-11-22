// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;

namespace CausalityDbg.Core.MetaCache
{
	[Flags]
	enum MetaModuleFlags
	{
		None = 0x00,
		IsDynamic = 0x01,
		IsInMemory = 0x02,
	}
}
