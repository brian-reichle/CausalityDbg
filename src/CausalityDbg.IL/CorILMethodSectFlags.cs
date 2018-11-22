// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;

namespace CausalityDbg.IL
{
	[Flags]
	public enum CorILMethodSectFlags
	{
		CorILMethod_Sect_EHTable = 0x01,
		CorILMethod_Sect_OptILTable = 0x02,
		CorILMethod_Sect_FatFormat = 0x40,
		CorILMethod_Sect_MoreSects = 0x80,
	}
}
