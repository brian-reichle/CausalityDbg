// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Generic;
using System.Collections.Immutable;
using CausalityDbg.Core.CorDebugApi;
using CausalityDbg.DataStore;
using CausalityDbg.Metadata;

namespace CausalityDbg.Core
{
	sealed class FrameDataFactory
	{
		public FrameDataFactory(MetaProvider cache)
		{
			_cache = cache;
		}

		public TraceData GetTrace(TraceData containingTrace, ICorDebugFrame topFrame)
		{
			var result = ImmutableArray.CreateBuilder<FrameData>();
			var topAddress = CORDB_ADDRESS.Null;
			CORDB_ADDRESS endAddress;

			foreach (var frame in GetFrames(topFrame))
			{
				if (result.Count == 0)
				{
					frame.GetStackRange(out topAddress, out endAddress);
				}

				result.Add(GetFrame(frame));

				if (containingTrace != null)
				{
					frame.GetStackRange(out var startAddress, out endAddress);

					if ((long)startAddress == containingTrace.TopAddress)
					{
						return new TraceData((long)topAddress, containingTrace, result.ToImmutable());
					}
				}
			}

			return new TraceData((long)topAddress, result.ToImmutable());
		}

		static IEnumerable<ICorDebugFrame> GetFrames(ICorDebugFrame topFrame)
		{
			var frame = topFrame;
			var chain = frame.GetChain();

			while (true)
			{
				for (; frame != null; frame = frame.GetCaller())
				{
					yield return frame;
				}

				if (!IsDebuggeeInitiated(chain.GetReason()))
				{
					yield break;
				}

				chain = chain.GetCaller();

				if (chain == null)
				{
					yield break;
				}

				frame = chain.GetActiveFrame();
			}
		}

		FrameData GetFrame(ICorDebugFrame frame)
		{
			return frame switch
			{
				ICorDebugILFrame ilFrame => GetFrame(ilFrame),
				ICorDebugInternalFrame internalFrame => GetFrame(internalFrame),
				_ => _frameTypeUnknown,
			};
		}

		FrameILData GetFrame(ICorDebugILFrame ilFrame)
		{
			var function = ilFrame.GetFunction();
			var module = function.GetModule();

			ImmutableArray<MetaCompound> genArgs;

			if (ilFrame is ICorDebugILFrame2 frame2)
			{
				genArgs = _cache.GetCompounds(module, frame2.EnumerateTypeParameters());
			}
			else
			{
				genArgs = ImmutableArray<MetaCompound>.Empty;
			}

			return new FrameILData(
				_cache.GetFunction(function),
				ilFrame.GetIP(),
				genArgs);
		}

		static FrameInternalData GetFrame(ICorDebugInternalFrame internalFrame)
		{
			return internalFrame.GetFrameType() switch
			{
				CorDebugInternalFrameType.STUBFRAME_APPDOMAIN_TRANSITION => _frameAppDomainTransition,
				CorDebugInternalFrameType.STUBFRAME_INTERNALCALL => _frameInternalCall,
				CorDebugInternalFrameType.STUBFRAME_LIGHTWEIGHT_FUNCTION => _frameLightWeightFunction,
				CorDebugInternalFrameType.STUBFRAME_M2U => _frameM2U,
				CorDebugInternalFrameType.STUBFRAME_U2M => _frameU2M,
				CorDebugInternalFrameType.STUBFRAME_NONE => _frameNone,
				_ => _frameInternalUnknown,
			};
		}

		static bool IsDebuggeeInitiated(CorDebugChainReason reason)
		{
			switch (reason)
			{
				case CorDebugChainReason.CHAIN_CLASS_INIT:
				case CorDebugChainReason.CHAIN_EXCEPTION_FILTER:
				case CorDebugChainReason.CHAIN_CONTEXT_POLICY:
				case CorDebugChainReason.CHAIN_NONE:
				case CorDebugChainReason.CHAIN_SECURITY:
				case CorDebugChainReason.CHAIN_THREAD_START:
				case CorDebugChainReason.CHAIN_ENTER_MANAGED:
				case CorDebugChainReason.CHAIN_ENTER_UNMANAGED:
					return true;

				default:
					return false;
			}
		}

		static readonly FrameInternalData _frameAppDomainTransition = new FrameInternalData("[AppDomain Transition]");
		static readonly FrameInternalData _frameInternalCall = new FrameInternalData("[Internal Call]");
		static readonly FrameInternalData _frameLightWeightFunction = new FrameInternalData("[Lightweight Function]");
		static readonly FrameInternalData _frameM2U = new FrameInternalData("[Managed to Unmanaged]");
		static readonly FrameInternalData _frameU2M = new FrameInternalData("[Unmanaged to Managed]");
		static readonly FrameInternalData _frameNone = new FrameInternalData("[None]");
		static readonly FrameInternalData _frameInternalUnknown = new FrameInternalData("[Unknown Internal Frame Type]");
		static readonly FrameInternalData _frameTypeUnknown = new FrameInternalData("[Unknown Frame Type]");
		readonly MetaProvider _cache;
	}
}
