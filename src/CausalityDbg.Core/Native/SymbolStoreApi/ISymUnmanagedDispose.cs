// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;

namespace CausalityDbg.Core.SymbolStoreApi
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("969708D2-05E5-4861-A3B0-96E473CDF63F")]
	interface ISymUnmanagedDispose
	{
		// HRESULT Destroy();
		void Destroy();
	}
}
