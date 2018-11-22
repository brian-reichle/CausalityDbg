// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Runtime.InteropServices;
using CausalityDbg.Core.CorDebugApi;

namespace CausalityDbg.Core
{
	static class ValueExtensions
	{
		public static bool IsNull(this ICorDebugReferenceValue value)
		{
			var hr = value.IsNull(out var isNull);

			if (hr < 0)
			{
				throw Marshal.GetExceptionForHR(hr);
			}

			return isNull;
		}

		public static bool IsNullOrCollected(this ICorDebugHandleValue handle)
		{
			var hr = handle.IsNull(out var isNull);

			if (hr == (int)HResults.CORDBG_E_BAD_REFERENCE_VALUE ||
				hr == (int)HResults.CORDBG_E_OBJECT_NEUTERED)
			{
				// The referenced object was collected.
				isNull = true;
			}
			else if (hr < 0)
			{
				throw Marshal.GetExceptionForHR(hr);
			}

			return isNull;
		}

		public static string GetStringValue(this ICorDebugStringValue objectValue)
		{
			var len = objectValue.GetLength();
			var buffer = new char[len];
			objectValue.GetString(len, out len, buffer);

			return new string(buffer, 0, (int)len);
		}
	}
}
