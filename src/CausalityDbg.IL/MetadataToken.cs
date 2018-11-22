// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace CausalityDbg.IL
{
	[StructLayout(LayoutKind.Sequential)]
	public readonly struct MetaDataToken : IEquatable<MetaDataToken>
	{
		public static readonly MetaDataToken Nil = new MetaDataToken();

		public MetaDataToken(uint tokenId) => _tokenId = tokenId;
		public TokenType TokenType => unchecked((TokenType)(_tokenId & 0xFF000000));
		public bool IsNil => (_tokenId & 0x00FFFFFF) == 0;
		public override int GetHashCode() => _tokenId.GetHashCode();
		public override bool Equals(object obj) => obj is MetaDataToken && this == (MetaDataToken)obj;
		public override string ToString() => _tokenId.ToString("X8", CultureInfo.InvariantCulture);
		public bool Equals(MetaDataToken other) => this == other;
		public static bool operator ==(MetaDataToken first, MetaDataToken second) => first._tokenId == second._tokenId;
		public static bool operator !=(MetaDataToken first, MetaDataToken second) => first._tokenId != second._tokenId;

		readonly uint _tokenId;
	}
}
