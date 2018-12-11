// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace CausalityDbg.Core.CorDebugApi
{
	enum CorDebugChainReason
	{
		/// <summary>
		/// No call chain has been initiated.
		/// </summary>
		CHAIN_NONE = 0x000,

		/// <summary>
		/// The chain was initiated by a constructor.
		/// </summary>
		CHAIN_CLASS_INIT = 0x001,

		/// <summary>
		/// The chain was initiated by an exception filter.
		/// </summary>
		CHAIN_EXCEPTION_FILTER = 0x002,

		/// <summary>
		/// The chain was initiated by code that enforces security.
		/// </summary>
		CHAIN_SECURITY = 0x004,

		/// <summary>
		/// The chain was initiated by a context policy.
		/// </summary>
		CHAIN_CONTEXT_POLICY = 0x008,

		/// <summary>
		/// Not used.
		/// </summary>
		CHAIN_INTERCEPTION = 0x010,

		/// <summary>
		/// Not used.
		/// </summary>
		CHAIN_PROCESS_START = 0x020,

		/// <summary>
		/// The chain was initiated by the start of a thread execution.
		/// </summary>
		CHAIN_THREAD_START = 0x040,

		/// <summary>
		/// The chain was initiated by entry into managed code.
		/// </summary>
		CHAIN_ENTER_MANAGED = 0x080,

		/// <summary>
		/// The chain was initiated by entry into unmanaged code.
		/// </summary>
		CHAIN_ENTER_UNMANAGED = 0x100,

		/// <summary>
		/// Not used.
		/// </summary>
		CHAIN_DEBUGGER_EVAL = 0x200,

		/// <summary>
		/// Not used.
		/// </summary>
		CHAIN_CONTEXT_SWITCH = 0x400,

		/// <summary>
		/// The chain was initiated by a function evaluation.
		/// </summary>
		CHAIN_FUNC_EVAL = 0x800
	}
}
