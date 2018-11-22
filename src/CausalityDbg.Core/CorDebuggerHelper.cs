// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using CausalityDbg.Core.CLRHostApi;
using CausalityDbg.Core.CorDebugApi;
using static CausalityDbg.Core.NativeMethods;

namespace CausalityDbg.Core
{
	static class CorDebuggerHelper
	{
		public static int GetCurrentPID()
		{
			using (var process = Process.GetCurrentProcess())
			{
				return process.Id;
			}
		}

		public static ICorDebug CreateDebuggingInterfaceForProcess(int pid)
		{
			const ProcessAccessOptions access =
				ProcessAccessOptions.PROCESS_VM_READ |
				ProcessAccessOptions.PROCESS_QUERY_INFORMATION |
				ProcessAccessOptions.PROCESS_DUP_HANDLE |
				ProcessAccessOptions.SYNCHRONIZE;

			using (var ph = NativeMethods.OpenProcess(access, false, pid))
			{
				if (ph.IsInvalid)
				{
					var inner = new Win32Exception(Marshal.GetLastWin32Error());
					throw new AttachException(AttachErrorType.ProcessNotFound, inner);
				}

				var host = CLRCreateMetaHost();
				var runtimes = host.EnumerateLoadedRuntimes(ph);
				var runtime = GetFirstSupportedRuntime(runtimes);
				return runtime.GetCorDebug();
			}
		}

		public static ICorDebug CreateDebuggingInterfaceForProcess(string process)
		{
			var host = CLRCreateMetaHost();
			var runtime = host.GetRequiredRuntime(process);
			return runtime.GetCorDebug();
		}

		static ICLRMetaHost CLRCreateMetaHost()
		{
			NativeMethods.CLRCreateInstance(
				CLSID.CLSID_CLRMetaHost,
				typeof(ICLRMetaHost).GUID,
				out var result);

			return (ICLRMetaHost)result;
		}

		static ICLRRuntimeInfo GetFirstSupportedRuntime(IEnumUnknown runtimes)
		{
			if (runtimes.Next(1, out var tmp))
			{
				do
				{
					var runtime = (ICLRRuntimeInfo)tmp;

					if (runtime.IsSupportedVersion())
					{
						return runtime;
					}
				}
				while (runtimes.Next(1, out tmp));

				throw new AttachException(AttachErrorType.UnsupportedCLRVersion);
			}
			else
			{
				throw new AttachException(AttachErrorType.FrameworkNotLoaded);
			}
		}
	}
}
