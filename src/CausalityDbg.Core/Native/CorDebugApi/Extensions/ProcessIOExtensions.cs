// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;

namespace CausalityDbg.Core.CorDebugApi
{
	static class ProcessIOExtensions
	{
		public static unsafe byte[] ReadBytes(this ICorDebugProcess process, CORDB_ADDRESS address, int size)
		{
			var result = new byte[size];

			if (size == 0)
			{
				return result;
			}

			fixed (byte* resultPtr = &result[0])
			{
				process.ReadMemory(address, size, (IntPtr)resultPtr, out var read);
			}

			return result;
		}

		public static unsafe T ReadValue<T>(this ICorDebugProcess process, CORDB_ADDRESS address)
			where T : unmanaged
		{
			T value = default;

			process.ReadMemory(address, sizeof(T), (IntPtr)(&value), out var read);

			return value;
		}
	}
}
