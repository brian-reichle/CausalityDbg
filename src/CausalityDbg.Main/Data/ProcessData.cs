// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Diagnostics;

namespace CausalityDbg.Main
{
	sealed class ProcessData
	{
		public static ProcessData FromPID(int pid)
		{
			try
			{
				using (var process = Process.GetProcessById(pid))
				{
					return new ProcessData(pid, process.ProcessName);
				}
			}
			catch (ArgumentException)
			{
				return null;
			}
		}

		ProcessData(int pid, string processName)
		{
			PID = pid;
			ProcessName = processName;
		}

		public int PID { get; }
		public string ProcessName { get; }
	}
}
