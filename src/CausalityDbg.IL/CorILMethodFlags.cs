// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;

namespace CausalityDbg.IL
{
	[Flags]
	public enum CorILMethodFlags
	{
		CorILMethod_FatFormat = 0x03,
		CorILMethod_TinyFormat = 0x02,
		CorILMethod_MoreSects = 0x08,
		CorILMethod_InitLocals = 0x10,
	}
}
