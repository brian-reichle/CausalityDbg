// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Runtime.InteropServices;

namespace CausalityDbg.Main.Win32
{
	[StructLayout(LayoutKind.Sequential)]
	struct POINT
	{
		public int X;
		public int Y;
	}
}
