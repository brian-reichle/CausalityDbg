// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Immutable;
using CausalityDbg.Core.CorDebugApi;

namespace CausalityDbg.Core
{
	public sealed class TraceData
	{
		public static readonly TraceData Empty = new TraceData(CORDB_ADDRESS.Null, ImmutableArray<FrameData>.Empty);

		internal TraceData(CORDB_ADDRESS topAddress, TraceData containingTrace, ImmutableArray<FrameData> frames)
		{
			if (containingTrace == null) throw new ArgumentNullException(nameof(containingTrace));
			if (frames.Length > 0 && topAddress.IsNull) throw new ArgumentException("has frames but no topAddress", nameof(topAddress));

			if (containingTrace.Frames.Length == 0)
			{
				TotalDepth = frames.Length;
			}
			else
			{
				TotalDepth = containingTrace.TotalDepth + frames.Length - 1;
			}

			TopAddress = topAddress;
			ContainingTrace = containingTrace;
			Frames = frames;
		}

		internal TraceData(CORDB_ADDRESS topAddress, ImmutableArray<FrameData> frames)
		{
			if (frames == null) throw new ArgumentNullException(nameof(frames));
			if (frames.Length > 0 && topAddress.IsNull) throw new ArgumentException("has frames but no topAddress", nameof(topAddress));

			TopAddress = topAddress;
			TotalDepth = frames.Length;
			ContainingTrace = null;
			Frames = frames;
		}

		public int TotalDepth { get; }
		public TraceData ContainingTrace { get; }
		public ImmutableArray<FrameData> Frames { get; }
		internal CORDB_ADDRESS TopAddress { get; }
	}
}
