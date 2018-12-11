// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using CausalityDbg.Core.CorDebugApi;

namespace CausalityDbg.Core.CLRHostApi
{
	static class RuntimeInfoExtensions
	{
		public static string GetVersionString(this ICLRRuntimeInfo runtime)
		{
			var size = 0;
			var hr = runtime.GetVersionString(null, ref size);

			if (hr < 0 && hr != (int)HResults.E_BUFFER_TOO_SMALL)
			{
				Marshal.ThrowExceptionForHR(hr);
			}
			else if (size == 0)
			{
				return null;
			}

			var buffer = new char[size];

			hr = runtime.GetVersionString(buffer, ref size);

			if (hr < 0)
			{
				Marshal.ThrowExceptionForHR(hr);
			}

			return new string(buffer, 0, size);
		}

		public static bool IsSupportedVersion(this ICLRRuntimeInfo runtime)
		{
			var version = runtime.GetVersionString();
			return VersionsMatch(version, "v4.0") || VersionsMatch(version, "v2.0");
		}

		public static ICorDebug GetCorDebug(this ICLRRuntimeInfo runtime)
		{
			return (ICorDebug)runtime.GetInterface(new Guid("DF8395B5-A4BA-450b-A77C-A9A47762C520"), typeof(ICorDebug).GUID);
		}

		static bool VersionsMatch(string actual, string expected)
		{
			return actual.StartsWith(expected, StringComparison.Ordinal) &&
				(actual.Length == expected.Length || actual[expected.Length] == '.');
		}
	}
}
