// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Immutable;
using System.Reflection;

namespace CausalityDbg.IL
{
	public static class SignatureReader
	{
		public static SigMethod ReadMethodDefSig(ReadOnlySpan<byte> blob)
		{
			var index = 0;
			var result = ReadMethodSig(blob, ref index, false);
			if (index != blob.Length) throw new InvalidSignatureException();
			return result;
		}

		static SigMethod ReadMethodRefSig(ReadOnlySpan<byte> blob, ref int index)
		{
			return ReadMethodSig(blob, ref index, true);
		}

		static SigMethod ReadMethodSig(ReadOnlySpan<byte> blob, ref int index, bool allowSentinel)
		{
			const CallingConventions mask =
				CallingConventions.HasThis |
				CallingConventions.ExplicitThis;

			const byte Default = 0x00;
			const byte VarArgs = 0x05;
			const byte Generic = 0x10;

			var preamble = blob[index++];
			var genParamCount = 0u;
			CallingConventions callingConvention;

			switch (unchecked((byte)~mask & preamble))
			{
				case VarArgs:
					callingConvention = CallingConventions.VarArgs;
					break;

				case Generic:
					genParamCount = Decompressor.ReadCompressedUInt(blob, ref index);
					goto case Default;

				case Default:
					callingConvention = CallingConventions.Standard;
					allowSentinel = false;
					break;

				default:
					throw new InvalidSignatureException();
			}

			callingConvention |= mask & (CallingConventions)preamble;

			var paramCount = Decompressor.ReadCompressedUInt(blob, ref index);
			var orderedParamCount = -1;
			var retType = ReadTypeRef(blob, ref index);
			var parameters = ImmutableArray.CreateBuilder<SigParameter>();

			while (paramCount > 0)
			{
				if (allowSentinel && (CorElementType)blob[index] == CorElementType.ELEMENT_TYPE_SENTINEL)
				{
					index++;
					allowSentinel = false;
					orderedParamCount = parameters.Count;
				}
				else
				{
					paramCount--;
					parameters.Add(ReadTypeRef(blob, ref index));
				}
			}

			if (orderedParamCount < 0)
			{
				orderedParamCount = parameters.Count;
			}

			var method = new SigMethod(
				callingConvention,
				genParamCount,
				orderedParamCount,
				retType,
				parameters.ToImmutable());

			return method;
		}

		static SigType ReadTypeCore(ReadOnlySpan<byte> blob, ref int index)
		{
			var elementType = (CorElementType)blob[index++];

			switch (elementType)
			{
				case CorElementType.ELEMENT_TYPE_BOOLEAN:
				case CorElementType.ELEMENT_TYPE_CHAR:
				case CorElementType.ELEMENT_TYPE_I1:
				case CorElementType.ELEMENT_TYPE_U1:
				case CorElementType.ELEMENT_TYPE_I2:
				case CorElementType.ELEMENT_TYPE_U2:
				case CorElementType.ELEMENT_TYPE_I4:
				case CorElementType.ELEMENT_TYPE_U4:
				case CorElementType.ELEMENT_TYPE_I8:
				case CorElementType.ELEMENT_TYPE_U8:
				case CorElementType.ELEMENT_TYPE_R4:
				case CorElementType.ELEMENT_TYPE_R8:
				case CorElementType.ELEMENT_TYPE_I:
				case CorElementType.ELEMENT_TYPE_U:
				case CorElementType.ELEMENT_TYPE_OBJECT:
				case CorElementType.ELEMENT_TYPE_STRING:
				case CorElementType.ELEMENT_TYPE_VOID:
					return new SigTypePrimitive(elementType);

				case CorElementType.ELEMENT_TYPE_ARRAY:
					return ReadArray(blob, ref index);

				case CorElementType.ELEMENT_TYPE_CLASS:
				case CorElementType.ELEMENT_TYPE_VALUETYPE:
					return ReadClass(elementType, blob, ref index);

				case CorElementType.ELEMENT_TYPE_MVAR:
				case CorElementType.ELEMENT_TYPE_VAR:
					return ReadVar(elementType, blob, ref index);

				case CorElementType.ELEMENT_TYPE_PTR:
					return ReadPointer(blob, ref index);

				case CorElementType.ELEMENT_TYPE_GENERICINST:
					return ReadGenericInst(blob, ref index);

				case CorElementType.ELEMENT_TYPE_SZARRAY:
					return ReadSZArray(blob, ref index);

				case CorElementType.ELEMENT_TYPE_FNPTR:
					return ReadFunctionPTR(blob, ref index);

				default:
					throw new InvalidSignatureException();
			}
		}

		static SigTypeArray ReadArray(ReadOnlySpan<byte> blob, ref int index)
		{
			var baseType = ReadTypeCore(blob, ref index);
			var rank = Decompressor.ReadCompressedUInt(blob, ref index);
			var numSizes = Decompressor.ReadCompressedUInt(blob, ref index);
			var sizes = ImmutableArray.CreateBuilder<uint>((int)numSizes);
			sizes.Count = (int)numSizes;

			for (var i = 0; i < sizes.Count; i++)
			{
				sizes[i] = Decompressor.ReadCompressedUInt(blob, ref index);
			}

			var numLoBounds = Decompressor.ReadCompressedUInt(blob, ref index);
			var lowerBounds = ImmutableArray.CreateBuilder<int>((int)numLoBounds);
			lowerBounds.Count = (int)numLoBounds;

			for (var i = 0; i < lowerBounds.Count; i++)
			{
				lowerBounds[i] = Decompressor.ReadCompressedInt(blob, ref index);
			}

			return new SigTypeArray(baseType, rank, sizes.MoveToImmutable(), lowerBounds.MoveToImmutable());
		}

		static SigTypeSZArray ReadSZArray(ReadOnlySpan<byte> blob, ref int index)
		{
			var modifiers = ReadCMODList(blob, ref index);
			var baseType = ReadTypeCore(blob, ref index);
			return new SigTypeSZArray(baseType, modifiers);
		}

		static SigTypeUserType ReadClass(CorElementType elementType, ReadOnlySpan<byte> blob, ref int index)
		{
			var token = Decompressor.ReadTypeDefOrRefOrSpecEncoded(blob, ref index);
			return new SigTypeUserType(elementType, token);
		}

		static SigTypePointer ReadPointer(ReadOnlySpan<byte> blob, ref int index)
		{
			var modifiers = ReadCMODList(blob, ref index);
			var baseType = ReadTypeCore(blob, ref index);
			return new SigTypePointer(baseType, modifiers);
		}

		static SigTypeGenericInst ReadGenericInst(ReadOnlySpan<byte> blob, ref int index)
		{
			var innerPreamble = (CorElementType)blob[index++];

			switch (innerPreamble)
			{
				case CorElementType.ELEMENT_TYPE_CLASS:
				case CorElementType.ELEMENT_TYPE_VALUETYPE:
					break;

				default:
					throw new InvalidSignatureException();
			}

			var baseType = ReadClass(innerPreamble, blob, ref index);
			var genArgCount = Decompressor.ReadCompressedUInt(blob, ref index);
			var genArguments = ImmutableArray.CreateBuilder<SigType>((int)genArgCount);
			genArguments.Count = (int)genArgCount;

			for (var i = 0; i < genArgCount; i++)
			{
				genArguments[i] = ReadTypeCore(blob, ref index);
			}

			return new SigTypeGenericInst(baseType, genArguments.MoveToImmutable());
		}

		static SigTypeFNPtr ReadFunctionPTR(ReadOnlySpan<byte> blob, ref int index)
		{
			var method = ReadMethodRefSig(blob, ref index);
			return new SigTypeFNPtr(method);
		}

		static SigTypeGen ReadVar(CorElementType elementType, ReadOnlySpan<byte> blob, ref int index)
		{
			var genIndex = Decompressor.ReadCompressedUInt(blob, ref index);
			return new SigTypeGen(elementType, genIndex);
		}

		static SigParameter ReadTypeRef(ReadOnlySpan<byte> blob, ref int index)
		{
			var modifiers = ImmutableArray.CreateBuilder<SigCustomModifier>();
			ReadCMODList(blob, ref index, modifiers);
			var byRefIndex = modifiers.Count;

			var byRef = false;
			SigType type = null;

			var elementType = (CorElementType)blob[index];

			if (elementType == CorElementType.ELEMENT_TYPE_TYPEDBYREF)
			{
				index++;
				type = new SigTypePrimitive(CorElementType.ELEMENT_TYPE_TYPEDBYREF);
			}
			else
			{
				if (elementType == CorElementType.ELEMENT_TYPE_BYREF)
				{
					index++;
					byRef = true;
				}

				ReadCMODList(blob, ref index, modifiers);
				type = ReadTypeCore(blob, ref index);
			}

			return new SigParameter(type, byRef, byRefIndex, modifiers.ToImmutable());
		}

		static ImmutableArray<SigCustomModifier> ReadCMODList(ReadOnlySpan<byte> blob, ref int index)
		{
			var modifiers = ImmutableArray.CreateBuilder<SigCustomModifier>();
			ReadCMODList(blob, ref index, modifiers);
			return modifiers.ToImmutable();
		}

		static void ReadCMODList(ReadOnlySpan<byte> blob, ref int index, ImmutableArray<SigCustomModifier>.Builder modifiers)
		{
			while (true)
			{
				var elementType = (CorElementType)blob[index];

				switch (elementType)
				{
					case CorElementType.ELEMENT_TYPE_CMOD_OPT:
					case CorElementType.ELEMENT_TYPE_CMOD_REQD:
						index++;
						var token = Decompressor.ReadTypeDefOrRefOrSpecEncoded(blob, ref index);
						modifiers.Add(new SigCustomModifier(elementType, token));
						break;

					default:
						return;
				}
			}
		}
	}
}
