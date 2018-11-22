// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;

namespace CausalityDbg.Core.CorDebugApi
{
	[Flags]
	enum CorDebugMappingResult
	{
		/// <summary>
		/// The native code is in the prolog, so the value of the IP is 0.
		/// </summary>
		MAPPING_PROLOG = 0x01,

		/// <summary>
		/// The native code is in an epilog, so the value of the IP is the
		/// address of the last instruction of the method.
		/// </summary>
		MAPPING_EPILOG = 0x02,

		/// <summary>
		/// No mapping information is available for the method, so the
		/// value of the IP is 0.
		/// </summary>
		MAPPING_NO_INFO = 0x04,

		/// <summary>
		/// Although there is mapping information for the method, the
		/// current address cannot be mapped to Microsoft intermediate
		/// language (MSIL) code. The value of the IP is 0.
		/// </summary>
		MAPPING_UNMAPPED_ADDRESS = 0x08,

		/// <summary>
		/// Either the method maps exactly to MSIL code or the frame has
		/// been interpreted, so the value of the IP is accurate.
		/// </summary>
		MAPPING_EXACT = 0x10,

		/// <summary>
		/// The method was successfully mapped, but the value of the IP
		/// may be approximate.
		/// </summary>
		MAPPING_APPROXIMATE = 0x20,
	}
}
