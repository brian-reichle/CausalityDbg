// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace CausalityDbg.Core.CorDebugApi
{
	enum CorDebugJITCompilerFlags
	{
		CORDEBUG_JIT_DEFAULT = 0x1,
		CORDEBUG_JIT_DISABLE_OPTIMIZATION = 0x3,
		CORDEBUG_JIT_ENABLE_ENC = 0x7,
	}
}
