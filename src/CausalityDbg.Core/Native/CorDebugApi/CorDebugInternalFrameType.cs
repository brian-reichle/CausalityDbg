// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace CausalityDbg.Core.CorDebugApi
{
	enum CorDebugInternalFrameType
	{
		/// <summary>
		/// Mimics a null value. This value is never returned from the
		/// ICorDebugInternalFrame::GetFrameType method.
		/// </summary>
		STUBFRAME_NONE = 0x00,

		/// <summary>
		/// Specifies a managed-to-unmanaged stub frame.
		/// </summary>
		STUBFRAME_M2U = 0x01,

		/// <summary>
		/// Specifies an unmanaged-to-managed stub frame.
		/// </summary>
		STUBFRAME_U2M = 0x02,

		/// <summary>
		/// Specifies a transition between application domains.
		/// </summary>
		STUBFRAME_APPDOMAIN_TRANSITION = 0x03,

		/// <summary>
		/// Specifies a lightweight method call.
		/// </summary>
		STUBFRAME_LIGHTWEIGHT_FUNCTION = 0x04,

		/// <summary>
		/// Specifies the start of function evaluation.
		/// </summary>
		STUBFRAME_FUNC_EVAL = 0x05,

		/// <summary>
		/// Specifies an internal call into the common language runtime.
		/// </summary>
		STUBFRAME_INTERNALCALL = 0x06,
	}
}
