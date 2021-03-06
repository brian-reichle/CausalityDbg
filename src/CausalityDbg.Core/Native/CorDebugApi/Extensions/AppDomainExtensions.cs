// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.IO;
using CausalityDbg.IL;

namespace CausalityDbg.Core.CorDebugApi
{
	static class AppDomainExtensions
	{
		public static ICorDebugClass ResolvePrimitiveClassRef(this ICorDebugAppDomain appDomain, CorElementType type)
		{
			var assEnum = appDomain.EnumerateAssemblies();
			var expectedName = "mscorlib.dll";

			while (assEnum.Next(1, out var ass))
			{
				var assName = ass.GetName();

				if (LastPathSegmentIs(assName, expectedName))
				{
					return ass.FindClass(type.GetFullName());
				}
			}

			return null;
		}

		static bool LastPathSegmentIs(string assName, string expectedName)
		{
			if (!assName.EndsWith(expectedName, StringComparison.Ordinal)) return false;
			if (assName.Length == expectedName.Length) return true;

			var index = assName.Length - 1 - expectedName.Length;
			var c = assName[index];

			if (c == Path.DirectorySeparatorChar) return true;
			if (c == Path.AltDirectorySeparatorChar) return true;

			return false;
		}
	}
}
