// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;

namespace CausalityDbg.Main
{
	[Flags]
	enum ToolFlags
	{
		None = 0,
		NeedsSource = 1,
		Invalid = 2,
	}
}
