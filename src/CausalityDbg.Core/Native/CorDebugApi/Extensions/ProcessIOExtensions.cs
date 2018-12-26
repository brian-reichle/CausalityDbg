// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;

namespace CausalityDbg.Core.CorDebugApi
{
	static class ProcessIOExtensions
	{
		public static unsafe void ReadBytes(this ICorDebugProcess process, CORDB_ADDRESS address, byte[] buffer, int offset, int size)
		{
			if (buffer == null) throw new ArgumentNullException(nameof(buffer));
			if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
			if (size < 0) throw new ArgumentOutOfRangeException(nameof(size));
			if (offset + size > buffer.Length) throw new ArgumentOutOfRangeException(nameof(offset));

			if (size == 0)
			{
				return;
			}

			fixed (byte* resultPtr = &buffer[offset])
			{
				process.ReadMemory(address, size, (IntPtr)resultPtr, out var read);
			}
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
