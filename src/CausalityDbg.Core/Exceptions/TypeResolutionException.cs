// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Runtime.Serialization;

namespace CausalityDbg.Core
{
	[Serializable]
	public sealed class TypeResolutionException : Exception
	{
		public TypeResolutionException()
		{
		}

		public TypeResolutionException(string message)
			: base(message)
		{
		}

		TypeResolutionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public TypeResolutionException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
