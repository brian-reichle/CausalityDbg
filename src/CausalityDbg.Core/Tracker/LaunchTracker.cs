// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using CausalityDbg.Configuration;
using CausalityDbg.Core.CorDebugApi;
using Microsoft.Win32.SafeHandles;
using static CausalityDbg.Core.NativeMethods;

namespace CausalityDbg.Core
{
	sealed class LaunchTracker : Tracker
	{
		public LaunchTracker(Config config, ITrackerCallback callback, LaunchArguments launchArguments)
			: base(config, callback)
		{
			_launchArguments = launchArguments ?? throw new ArgumentNullException(nameof(launchArguments));

			if (!File.Exists(launchArguments.Process)) throw new AttachException(AttachErrorType.FileNotFound);
			if (!Directory.Exists(_launchArguments.Directory)) throw new AttachException(AttachErrorType.DirectoryNotFound);

			if (BitnessHelper.IsExecutable64Bit(_launchArguments.Process) != Environment.Is64BitProcess) throw new AttachException(AttachErrorType.IncompatiblePlatforms);

			_useDebugNGENImages = launchArguments.UseDebugNGENImages;
		}

		protected override ICorDebug CreateDebugger()
			=> CorDebuggerHelper.CreateDebuggingInterfaceForProcess(_launchArguments.Process, _launchArguments.RuntimeVersion);

		protected override ICorDebugProcess CreateProcess(ICorDebug debugger)
		{
			var info = new STARTUPINFO();
			info.cb = Marshal.SizeOf(info);
			info.hStdInput = new SafeFileHandle(IntPtr.Zero, false);
			info.hStdOutput = new SafeFileHandle(IntPtr.Zero, false);
			info.hStdError = new SafeFileHandle(IntPtr.Zero, false);

			var proc = new PROCESS_INFORMATION();

			var environmentBlock = IntPtr.Zero;
			var flags = ProcessCreationFlags.None;

			if (_launchArguments.Environment != null)
			{
				var chars = ConstructEnvironmentBlock(_launchArguments.Environment);
				flags |= ProcessCreationFlags.CREATE_UNICODE_ENVIRONMENT;
				environmentBlock = Marshal.AllocCoTaskMem(chars.Length << 1);
				Marshal.Copy(chars, 0, environmentBlock, chars.Length);
			}

			try
			{
				debugger.CreateProcess(
					_launchArguments.Process,
					ConstructCommandLine(_launchArguments.Process, _launchArguments.Arguments),
					IntPtr.Zero,
					IntPtr.Zero,
					true,
					flags,
					environmentBlock,
					_launchArguments.Directory,
					info,
					proc,
					0,
					out var process);

				return process;
			}
			finally
			{
				if (environmentBlock != IntPtr.Zero)
				{
					Marshal.FreeCoTaskMem(environmentBlock);
				}

				CloseHandle(proc.hProcess);
				CloseHandle(proc.hThread);
			}
		}

		protected override void Start(ICorDebugProcess process)
		{
			if (_useDebugNGENImages)
			{
				var hr = ((ICorDebugProcess2)process).SetDesiredNGENCompilerFlags(CorDebugJITCompilerFlags.CORDEBUG_JIT_DISABLE_OPTIMIZATION);

				if (hr != (int)HResults.CORDBG_E_CANNOT_BE_ON_ATTACH && hr < 0)
				{
					throw Marshal.GetExceptionForHR(hr);
				}
			}

			base.Start(process);
		}

		static char[] ConstructEnvironmentBlock(IDictionary block)
		{
			var characters = 1;

			foreach (DictionaryEntry pair in block)
			{
				var key = (string)pair.Key;
				var value = (string)pair.Value;
				characters += key.Length + value.Length + 2;
			}

			var result = new char[characters];
			var current = 0;

			foreach (DictionaryEntry pair in block)
			{
				var key = (string)pair.Key;
				var value = (string)pair.Value;

				key.CopyTo(0, result, current, key.Length);
				current += key.Length;
				result[current++] = '=';
				value.CopyTo(0, result, current, value.Length);
				current += value.Length;
				result[current++] = '\0';
			}

			result[current] = '\0';

			return result;
		}

		static string ConstructCommandLine(string process, string arguments)
		{
			if (string.IsNullOrEmpty(arguments))
			{
				return "\"" + process + "\"";
			}
			else
			{
				return "\"" + process + "\" " + arguments;
			}
		}

		readonly bool _useDebugNGENImages;
		readonly LaunchArguments _launchArguments;
	}
}
