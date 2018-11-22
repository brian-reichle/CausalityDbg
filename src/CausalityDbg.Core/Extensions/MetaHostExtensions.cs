// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Runtime.InteropServices;
using CausalityDbg.Core.CLRHostApi;

namespace CausalityDbg.Core
{
	static class MetaHostExtensions
	{
		public static ICLRRuntimeInfo GetRequiredRuntime(this ICLRMetaHost host, string path)
		{
			var size = 0u;
			var hr = host.GetVersionFromFile(path, null, ref size);

			if (hr < 0 && hr != (int)HResults.E_BUFFER_TOO_SMALL)
			{
				throw Marshal.GetExceptionForHR(hr);
			}
			else if (size == 0)
			{
				return null;
			}

			var buffer = new char[size];
			hr = host.GetVersionFromFile(path, buffer, ref size);

			if (hr < 0)
			{
				throw Marshal.GetExceptionForHR(hr);
			}

			var version = new string(buffer, 0, (int)size);

			return (ICLRRuntimeInfo)host.GetRuntime(version, typeof(ICLRRuntimeInfo).GUID);
		}
	}
}
