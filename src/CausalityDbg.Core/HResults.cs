// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace CausalityDbg.Core
{
	enum HResults
	{
		E_PDB_NOT_FOUND = unchecked((int)0x806D0005),
		E_BUFFER_TOO_SMALL = unchecked((int)0x8007007A),

		CLDB_E_RECORD_NOTFOUND = unchecked((int)0x80131130),

		CORDBG_E_BAD_REFERENCE_VALUE = unchecked((int)0x80131305),
		CORDBG_E_OBJECT_NEUTERED = unchecked((int)0x8013134F),
		CORDBG_E_CANNOT_BE_ON_ATTACH = unchecked((int)0x80131C13),
		CORDBG_E_CANT_CHANGE_JIT_SETTING_FOR_ZAP_MODULE = unchecked((int)0x8013131D),
		CORDBG_E_DEBUGGER_ALREADY_ATTACHED = unchecked((int)0x8013132e),
		CORDBG_E_IL_VAR_NOT_AVAILABLE = unchecked((int)0x80131304),
	}
}
