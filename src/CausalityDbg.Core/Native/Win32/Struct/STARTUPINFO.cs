// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace CausalityDbg.Core
{
#pragma warning disable SA1401 // Fields should be private
	static partial class NativeMethods
	{
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 8)]
		public sealed class STARTUPINFO
		{
			/// <summary>
			/// The size of the structure, in bytes.
			/// </summary>
			public int cb;

			/// <summary>
			/// Reserved; must be NULL.
			/// </summary>
			public IntPtr lpReserved;

			/// <summary>
			/// The name of the desktop, or the name of both the desktop and window station for this process. A backslash in the
			/// string indicates that the string includes both the desktop and window station names. For more information, see
			/// Thread Connection to a Desktop.
			/// </summary>
			public string lpDesktop;

			/// <summary>
			/// For console processes, this is the title displayed in the title bar if a new console window is created. If NULL,
			/// the name of the executable file is used as the window title instead. This parameter must be NULL for GUI or console
			/// processes that do not create a new console window.
			/// </summary>
			public string lpTitle;

			/// <summary>
			/// If dwFlags specifies STARTF_USEPOSITION, this member is the x offset of the upper left corner of a window if a new
			/// window is created, in pixels. Otherwise, this member is ignored.
			/// </summary>
			public int dwX;

			/// <summary>
			/// If dwFlags specifies STARTF_USEPOSITION, this member is the y offset of the upper left corner of a window if a new
			/// window is created, in pixels. Otherwise, this member is ignored.
			/// </summary>
			public int dwY;

			/// <summary>
			/// If dwFlags specifies STARTF_USESIZE, this member is the width of the window if a new window is created, in pixels.
			/// Otherwise, this member is ignored.
			/// </summary>
			public int dwXSize;

			/// <summary>
			/// If dwFlags specifies STARTF_USESIZE, this member is the height of the window if a new window is created, in pixels.
			/// Otherwise, this member is ignored.
			/// </summary>
			public int dwYSize;

			/// <summary>
			/// If dwFlags specifies STARTF_USECOUNTCHARS, if a new console window is created in a console process, this member
			/// specifies the screen buffer width, in character columns. Otherwise, this member is ignored.
			/// </summary>
			public int dwXCountChars;

			/// <summary>
			/// If dwFlags specifies STARTF_USECOUNTCHARS, if a new console window is created in a console process, this member
			/// specifies the screen buffer height, in character rows. Otherwise, this member is ignored.
			/// </summary>
			public int dwYCountChars;

			/// <summary>
			/// If dwFlags specifies STARTF_USEFILLATTRIBUTE, this member is the initial text and background colors if a new
			/// console window is created in a console application. Otherwise, this member is ignored.
			/// </summary>
			public int dwFillAttribute;

			/// <summary>
			/// A bitfield that determines whether certain STARTUPINFO members are used when the process creates a window.
			/// </summary>
			public int dwFlags;

			/// <summary>
			/// If dwFlags specifies STARTF_USESHOWWINDOW, this member can be any of the values that can be specified in the
			/// nCmdShow parameter for the ShowWindow function, except for SW_SHOWDEFAULT. Otherwise, this member is ignored.
			/// </summary>
			public short wShowWindow;

			/// <summary>
			/// Reserved for use by the C Run-time; must be zero.
			/// </summary>
			public short cbReserved2;

			/// <summary>
			/// Reserved for use by the C Run-time; must be NULL.
			/// </summary>
			public IntPtr lpReserved2;

			/// <summary>
			/// If dwFlags specifies STARTF_USESTDHANDLES, this member is the standard input handle for the process.
			/// If STARTF_USESTDHANDLES is not specified, the default for standard input is the keyboard buffer.
			/// </summary>
			public SafeFileHandle hStdInput;

			/// <summary>
			/// If dwFlags specifies STARTF_USESTDHANDLES, this member is the standard output handle for the process.
			/// Otherwise, this member is ignored and the default for standard output is the console window's buffer.
			/// </summary>
			public SafeFileHandle hStdOutput;

			/// <summary>
			/// If dwFlags specifies STARTF_USESTDHANDLES, this member is the standard error handle for the process.
			/// Otherwise, this member is ignored and the default for standard error is the console window's buffer.
			/// </summary>
			public SafeFileHandle hStdError;
		}
	}
#pragma warning restore SA1401 // Fields should be private
}
