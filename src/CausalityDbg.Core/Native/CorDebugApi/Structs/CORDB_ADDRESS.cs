// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace CausalityDbg.Core.CorDebugApi
{
	[StructLayout(LayoutKind.Sequential)]
	readonly struct CORDB_ADDRESS : IEquatable<CORDB_ADDRESS>
	{
		public static readonly CORDB_ADDRESS Null = new CORDB_ADDRESS(0);

		public static CORDB_ADDRESS operator +(CORDB_ADDRESS address, int offset) => new CORDB_ADDRESS(address._value + offset);
		public static bool operator ==(CORDB_ADDRESS address1, CORDB_ADDRESS address2) => address1._value == address2._value;
		public static bool operator !=(CORDB_ADDRESS address1, CORDB_ADDRESS address2) => address1._value != address2._value;

		public static explicit operator long(CORDB_ADDRESS value) => value._value;

		CORDB_ADDRESS(long address) => _value = address;

		public bool IsNull => _value == 0;
		public CORDB_ADDRESS AlignToWord() => new CORDB_ADDRESS((_value + 3) & ~0x3L);

		public bool Equals(CORDB_ADDRESS other) => _value == other._value;
		public override bool Equals(object obj) => obj is CORDB_ADDRESS && Equals((CORDB_ADDRESS)obj);
		public override int GetHashCode() => _value.GetHashCode();

		public override string ToString() => _value.ToString("X16", CultureInfo.InvariantCulture);

		readonly long _value;
	}
}
