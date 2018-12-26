// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Buffers;
using System.Runtime.InteropServices;

namespace CausalityDbg.Core.CLRHostApi
{
	static class MetaHostExtensions
	{
		public static string GetVersionFromFile(this ICLRMetaHost host, string path)
		{
			var size = 0;
			var hr = host.GetVersionFromFile(path, null, ref size);

			if (hr < 0 && hr != (int)HResults.E_BUFFER_TOO_SMALL)
			{
				throw Marshal.GetExceptionForHR(hr);
			}
			else if (size == 0)
			{
				return null;
			}

			var buffer = ArrayPool<char>.Shared.Rent(size);
			hr = host.GetVersionFromFile(path, buffer, ref size);

			if (hr < 0)
			{
				throw Marshal.GetExceptionForHR(hr);
			}

			var version = new string(buffer, 0, size);
			ArrayPool<char>.Shared.Return(buffer);

			return version;
		}

		public static ICLRRuntimeInfo GetRuntime(this ICLRMetaHost host, string version)
		{
			var hr = host.GetRuntime(version, typeof(ICLRRuntimeInfo).GUID, out var result);

			if (hr < 0)
			{
				var ex = Marshal.GetExceptionForHR(hr);

				if (hr == (int)HResults.CLR_E_SHIM_RUNTIMELOAD)
				{
					throw new AttachException(AttachErrorType.MissingCLRVersion, ex);
				}

				throw ex;
			}

			return (ICLRRuntimeInfo)result;
		}
	}
}
