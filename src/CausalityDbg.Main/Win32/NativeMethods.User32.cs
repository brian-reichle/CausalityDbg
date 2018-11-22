// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;

namespace CausalityDbg.Main.Win32
{
	static class NativeMethods
	{
		// HWND WindowFromPoint(
		//     [in]  POINT Point
		// );
		[DllImport("user32.dll")]
		public static extern IntPtr WindowFromPoint(
			POINT Point);

		// DWORD GetWindowThreadProcessId(
		//     [in]  HWND    hWnd,
		//     [out] LPDWORD lpdwProcessId
		// );
		[DllImport("user32.dll", SetLastError = true)]
		public static extern uint GetWindowThreadProcessId(
			IntPtr hWnd,
			out int lpdwProcessId);
	}
}
