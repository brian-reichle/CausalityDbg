// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using CausalityDbg.Core.CorDebugApi;

namespace CausalityDbg.Core
{
	static class ProcessIOExtensions
	{
		public static byte[] ReadBytes(this ICorDebugProcess process, CORDB_ADDRESS address, int size)
		{
			var result = new byte[size];

			if (size == 0)
			{
				return result;
			}

			var buffer = process.ReadBuffer(address, size);

			try
			{
				Marshal.Copy(buffer, result, 0, size);
			}
			finally
			{
				Marshal.FreeCoTaskMem(buffer);
			}

			return result;
		}

		public static byte ReadByte(this ICorDebugProcess process, CORDB_ADDRESS address)
		{
			var buffer = process.ReadBuffer(address, 1);

			try
			{
				return Marshal.ReadByte(buffer);
			}
			finally
			{
				Marshal.FreeCoTaskMem(buffer);
			}
		}

		public static short ReadShort(this ICorDebugProcess process, CORDB_ADDRESS address)
		{
			var buffer = process.ReadBuffer(address, 2);

			try
			{
				return Marshal.ReadInt16(buffer);
			}
			finally
			{
				Marshal.FreeCoTaskMem(buffer);
			}
		}

		public static int ReadInt(this ICorDebugProcess processor, CORDB_ADDRESS address)
		{
			var buffer = processor.ReadBuffer(address, 4);

			try
			{
				return Marshal.ReadInt32(buffer);
			}
			finally
			{
				Marshal.FreeCoTaskMem(buffer);
			}
		}

		public static long ReadLong(this ICorDebugProcess process, CORDB_ADDRESS address)
		{
			var buffer = process.ReadBuffer(address, 8);

			try
			{
				return Marshal.ReadInt64(buffer);
			}
			finally
			{
				Marshal.FreeCoTaskMem(buffer);
			}
		}

		static IntPtr ReadBuffer(this ICorDebugProcess process, CORDB_ADDRESS address, int size)
		{
			var ptr = Marshal.AllocCoTaskMem(size);

			try
			{
				process.ReadMemory(address, size, ptr, out var read);
			}
			catch
			{
				Marshal.FreeCoTaskMem(ptr);
				throw;
			}

			return ptr;
		}
	}
}
