// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace CausalityDbg.Main.Win32
{
	static class SafeWin32
	{
		public static int? GetProcessIDAtPoint(int x, int y)
		{
			var p = new POINT();
			p.X = x;
			p.Y = y;

			var hWnd = NativeMethods.WindowFromPoint(p);

			if (hWnd == IntPtr.Zero)
			{
				return null;
			}

			if (NativeMethods.GetWindowThreadProcessId(hWnd, out var pid) == 0)
			{
				var error = Marshal.GetLastWin32Error();

				if (error != 0)
				{
					throw new Win32Exception(error);
				}
			}

			return pid;
		}
	}
}
