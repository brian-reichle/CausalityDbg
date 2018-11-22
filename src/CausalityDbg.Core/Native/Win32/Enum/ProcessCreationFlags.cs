// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;

namespace CausalityDbg.Core
{
	static partial class NativeMethods
	{
		[Flags]
		public enum ProcessCreationFlags
		{
			None = 0x00000000,
			CREATE_UNICODE_ENVIRONMENT = 0x00000400,
		}
	}
}
