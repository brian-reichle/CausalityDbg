// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.ComponentModel;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using static CausalityDbg.Core.NativeMethods;

namespace CausalityDbg.Core
{
	static class BitnessHelper
	{
		public static bool IsProcess64Bit(int pid)
		{
			if (!Environment.Is64BitOperatingSystem)
			{
				return false;
			}

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

				return !IsWow64Process(ph.DangerousGetHandle());
			}
		}

		public static bool IsExecutable64Bit(string path)
		{
			var machine = GetMachineType(path);

			switch (machine)
			{
				case MachineType.x86:
					return false;

				case MachineType.x64:
					return true;

				case MachineType.AnyCPU:
					return Environment.Is64BitOperatingSystem;
			}

			if (NativeMethods.GetBinaryType(path, out var type))
			{
				return type != BinaryType.SCS_32BIT_BINARY;
			}
			else
			{
				throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
			}
		}

		static bool IsWow64Process(IntPtr hProcess)
		{
			if (NativeMethods.IsWow64Process(hProcess, out var tmp))
			{
				return tmp;
			}
			else
			{
				throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
			}
		}

		static MachineType GetMachineType(string path)
		{
			var file = MemoryMappedFile.CreateFromFile(
				path,
				FileMode.Open,
				null,
				0,
				MemoryMappedFileAccess.Read);

			using (var reader = new CilReader(file))
			{
				if (!reader.IsValid)
				{
					return MachineType.Invalid;
				}

				return reader.GetMachineType();
			}
		}

		enum MachineType
		{
			Invalid = 0x00,
			AnyCPU = 0x01,
			x86 = 0x02,
			x64 = 0x03,
		}

		sealed class CilReader : IDisposable
		{
			const short MZ = 0x5A4D;
			const int PE = 0x4550;
			const int Magic_x86 = 0x010B;
			const int Magic_x64 = 0x020B;

			const int PointerOffset = 60;

			const int PEHEADER_Size = 20;
			const int PEHEADER_NumberOfSections = 2;
			const int PEHEADER_OptionalHeaderSize = 16;

			const int PEOPTHEADER_Magic = 0;
			const int PEOPTHEADER_CilHeaderRVA = 208;

			const int SECTION_RowSize = 40;
			const int SECTION_VirtualSize = 8;
			const int SECTION_VirtualAddress = 12;
			const int SECTION_PointerToRawData = 20;

			const int CILHEADER_Flags = 16;

			const int COMIMAGE_FLAGS_32BITREQUIRED = 0x02;

			public CilReader(MemoryMappedFile file)
			{
				_file = file;
				_accessor = file.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);

				if (ReadInitialOffsets(_accessor, out _headerOffset, out _optionalHeaderOffset, out _sectionTableOffset))
				{
					IsValid = true;
					_numberOfSections = _accessor.ReadInt16(PEHEADER_NumberOfSections);
				}
				else
				{
					IsValid = false;
				}
			}

			public MachineType GetMachineType()
			{
				switch (_accessor.ReadInt16(_optionalHeaderOffset + PEOPTHEADER_Magic))
				{
					case Magic_x64:
						return MachineType.x64;

					case Magic_x86:
						break;

					default:
						return MachineType.Invalid;
				}

				if (!TryTranslateMemoryToFileRVA(_accessor.ReadInt32(_optionalHeaderOffset + PEOPTHEADER_CilHeaderRVA), out var cilHeaderOffset))
				{
					return MachineType.Invalid;
				}

				var flags = _accessor.ReadInt32(cilHeaderOffset + CILHEADER_Flags);

				if ((flags & COMIMAGE_FLAGS_32BITREQUIRED) != 0)
				{
					return MachineType.x86;
				}

				return MachineType.AnyCPU;
			}

			public bool IsValid { get; }

			static bool ReadInitialOffsets(MemoryMappedViewAccessor accessor, out int headerOffset, out int optionalHeaderOffset, out int sectionTableOffset)
			{
				if (accessor.ReadInt16(0) != MZ)
				{
					headerOffset = 0;
					optionalHeaderOffset = 0;
					sectionTableOffset = 0;
					return false;
				}

				var tmp = accessor.ReadInt32(PointerOffset);

				if (accessor.ReadInt32(tmp) != PE)
				{
					headerOffset = 0;
					optionalHeaderOffset = 0;
					sectionTableOffset = 0;
					return false;
				}

				headerOffset = tmp + 4;
				optionalHeaderOffset = headerOffset + PEHEADER_Size;
				sectionTableOffset = accessor.ReadInt16(headerOffset + PEHEADER_OptionalHeaderSize) + optionalHeaderOffset;
				return true;
			}

			bool TryTranslateMemoryToFileRVA(int memoryRVA, out int fileRVA)
			{
				if (!IsValid)
				{
					throw new InvalidOperationException();
				}

				var startOfSectionRow = _sectionTableOffset;

				for (var i = 0; i < _numberOfSections; i++)
				{
					var virtualSize = _accessor.ReadInt32(startOfSectionRow + SECTION_VirtualSize);
					var virtualAddress = _accessor.ReadInt32(startOfSectionRow + SECTION_VirtualAddress);

					if (memoryRVA >= virtualAddress && memoryRVA < virtualAddress + virtualSize)
					{
						var pointerToRawData = _accessor.ReadInt32(startOfSectionRow + SECTION_PointerToRawData);

						fileRVA = memoryRVA - virtualAddress + pointerToRawData;
						return true;
					}

					startOfSectionRow += SECTION_RowSize;
				}

				fileRVA = 0;
				return false;
			}

			readonly int _headerOffset;
			readonly int _optionalHeaderOffset;
			readonly int _sectionTableOffset;
			readonly int _numberOfSections;

			readonly MemoryMappedFile _file;
			readonly MemoryMappedViewAccessor _accessor;

			public void Dispose()
			{
				_accessor.Dispose();
				_file.Dispose();
			}
		}
	}
}
