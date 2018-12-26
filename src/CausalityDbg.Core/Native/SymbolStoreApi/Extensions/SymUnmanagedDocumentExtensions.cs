// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Buffers;

namespace CausalityDbg.Core.SymbolStoreApi
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

			var buffer = ArrayPool<char>.Shared.Rent(size);

			document.GetURL(
				buffer.Length,
				out size,
				buffer);

			var url = new string(buffer, 0, size - 1);
			ArrayPool<char>.Shared.Return(buffer);
			return url;
		}
	}
}
