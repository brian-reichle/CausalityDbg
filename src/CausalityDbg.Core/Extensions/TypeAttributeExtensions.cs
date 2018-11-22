// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Reflection;

namespace CausalityDbg.Core
{
	static class TypeAttributeExtensions
	{
		public static bool IsNested(this TypeAttributes att)
		{
			switch (att & TypeAttributes.VisibilityMask)
			{
				case TypeAttributes.NotPublic:
				case TypeAttributes.Public:
					return false;

				default:
					return true;
			}
		}
	}
}
