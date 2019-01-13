// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using CausalityDbg.IL;

namespace CausalityDbg.Core.MetaDataApi
{
	static class MetaDataExtensions
	{
		public static int CountGenericParams(this IMetaDataImport import, MetaDataToken token)
		{
			var import2 = import as IMetaDataImport2;
			return import2 == null ? 0 : import2.CountGenericParams(token);
		}

		public static int CountGenericParams(this IMetaDataImport2 import, MetaDataToken token)
		{
			var h = IntPtr.Zero;
			var result = 0;

			try
			{
				if (import.EnumGenericParams(ref h, token, out var tmp, 1))
				{
					result = import.CountEnum(h);
				}
				else
				{
					result = 0;
				}
			}
			finally
			{
				if (h != IntPtr.Zero)
				{
					import.CloseEnum(h);
				}
			}

			return result;
		}

		public static MetaDataToken GetAssemblyFromScope(this IMetaDataAssemblyImport import)
		{
			var hr = import.GetAssemblyFromScope(out var token);

			if (hr < 0)
			{
				if (hr == (int)HResults.CLDB_E_RECORD_NOTFOUND)
				{
					return MetaDataToken.Nil;
				}

				Marshal.ThrowExceptionForHR(hr);
			}

			return token;
		}
	}
}
