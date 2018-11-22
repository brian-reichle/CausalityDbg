// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using CausalityDbg.Core.SymbolStoreApi;

namespace CausalityDbg.Core
{
	static class SymUnmanagedDocumentExtensions
	{
		public static string GetUrl(this ISymUnmanagedDocument document)
		{
			if (document == null) return null;

			document.GetURL(
				0,
				out var size,
				null);

			var buffer = new char[size];

			document.GetURL(
				buffer.Length,
				out size,
				buffer);

			return new string(buffer, 0, size - 1);
		}
	}
}
