// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using CausalityDbg.Core.CorDebugApi;
using CausalityDbg.Core.MetaDataApi;
using CausalityDbg.IL;

namespace CausalityDbg.Core.MetaCache
{
	sealed class MetaProvider
	{
		public MetaProvider()
		{
			_primitiveLookup = new Dictionary<CorElementType, MetaType>();
			_moduleLookup = new Dictionary<ICorDebugModule, ModuleLevelCache>();
			_moduleNameLookup = new Dictionary<string, ModuleLevelCache>();
		}

		public MetaModule GetModule(ICorDebugModule module)
		{
			return module == null ? null : GetCache(module).Module;
		}

		public MetaType GetType(ICorDebugClass cl)
		{
			if (cl == null)
			{
				return null;
			}

			var module = cl.GetModule();
			var cache = GetCache(module);
			return cache.GetType(module, cl.GetToken());
		}

		public MetaType GetType(ICorDebugModule module, CorElementType elementType)
		{
			if (_primitiveLookup.TryGetValue(elementType, out var result))
			{
				return result;
			}

			var appDomain = module.GetAssembly().GetAppDomain();
			var cl = appDomain.ResolvePrimitiveClassRef(elementType);
			return GetType(cl);
		}

		public MetaCompound GetCompound(ICorDebugModule module, ICorDebugType type)
		{
			switch (type.GetType())
			{
				case CorElementType.ELEMENT_TYPE_VOID:
				case CorElementType.ELEMENT_TYPE_BOOLEAN:
				case CorElementType.ELEMENT_TYPE_CHAR:
				case CorElementType.ELEMENT_TYPE_I:
				case CorElementType.ELEMENT_TYPE_U:
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
				case CorElementType.ELEMENT_TYPE_OBJECT:
				case CorElementType.ELEMENT_TYPE_STRING:
				case CorElementType.ELEMENT_TYPE_TYPEDBYREF:
					return new MetaCompoundClass(
						GetType(module, type.GetType()),
						ImmutableArray<MetaCompound>.Empty);

				case CorElementType.ELEMENT_TYPE_PTR:
					return new MetaCompoundPointer(
						GetCompound(module, type.GetFirstTypeParameter()));

				case CorElementType.ELEMENT_TYPE_BYREF:
					return new MetaCompoundByRef(
						GetCompound(module, type.GetFirstTypeParameter()));

				case CorElementType.ELEMENT_TYPE_SZARRAY:
					return new MetaCompoundArray(
						GetCompound(module, type.GetFirstTypeParameter()),
						1);

				case CorElementType.ELEMENT_TYPE_ARRAY:
					return new MetaCompoundArray(
						GetCompound(module, type.GetFirstTypeParameter()),
						type.GetRank());

				case CorElementType.ELEMENT_TYPE_CLASS:
				case CorElementType.ELEMENT_TYPE_VALUETYPE:
					return new MetaCompoundClass(
						GetType(type.GetClass()),
						GetCompounds(module, type.EnumerateTypeParameters()));

				default:
					throw new TypeResolutionException();
			}
		}

		public ImmutableArray<MetaCompound> GetCompounds(ICorDebugModule module, ICorDebugTypeEnum types)
		{
			var count = types.GetCount();

			if (count == 0)
			{
				return ImmutableArray<MetaCompound>.Empty;
			}

			var result = ImmutableArray.CreateBuilder<MetaCompound>(count);
			result.Count = count;

			for (var i = 0; i < count; i++)
			{
				types.Next(1, out var type);
				result[i] = GetCompound(module, type);
			}

			return result.MoveToImmutable();
		}

		public MetaFunction GetFunction(ICorDebugFunction function)
		{
			if (function == null)
			{
				return null;
			}

			var module = function.GetModule();
			var cache = GetCache(module);
			return cache.GetFunction(module, function.GetToken());
		}

		public void UnloadModule(ICorDebugModule module)
		{
			_moduleLookup.Remove(module);
		}

		ModuleLevelCache GetCache(ICorDebugModule module)
		{
			if (_moduleLookup.TryGetValue(module, out var result))
			{
				return result;
			}

			var name = module.GetName();

			if (_moduleNameLookup.TryGetValue(name, out result))
			{
				_moduleLookup.Add(module, result);
				return result;
			}

			result = new ModuleLevelCache(this, new MetaModule(name, GetFlags(module)));
			_moduleNameLookup.Add(name, result);
			_moduleLookup.Add(module, result);
			return result;
		}

		static MetaModuleFlags GetFlags(ICorDebugModule module)
		{
			var result = MetaModuleFlags.None;

			if (module.IsDynamic())
			{
				result |= MetaModuleFlags.IsDynamic;
			}

			if (module.IsInMemory())
			{
				result |= MetaModuleFlags.IsInMemory;
			}

			return result;
		}

		readonly Dictionary<CorElementType, MetaType> _primitiveLookup;
		readonly Dictionary<ICorDebugModule, ModuleLevelCache> _moduleLookup;
		readonly Dictionary<string, ModuleLevelCache> _moduleNameLookup;

		sealed class ModuleLevelCache
		{
			public ModuleLevelCache(MetaProvider provider, MetaModule module)
			{
				Provider = provider;
				Module = module;
				TypeLookup = new Dictionary<MetaDataToken, MetaType>();
				FunctionLookup = new Dictionary<MetaDataToken, MetaFunction>();
			}

			public MetaModule Module { get; }
			public MetaProvider Provider { get; }
			public Dictionary<MetaDataToken, MetaType> TypeLookup { get; }
			public Dictionary<MetaDataToken, MetaFunction> FunctionLookup { get; }

			public MetaType GetType(ICorDebugModule module, MetaDataToken token)
			{
				if (TypeLookup.TryGetValue(token, out var result))
				{
					return result;
				}

				result = CreateType(module, token);
				TypeLookup.Add(token, result);
				return result;
			}

			public MetaFunction GetFunction(ICorDebugModule module, MetaDataToken token)
			{
				if (FunctionLookup.TryGetValue(token, out var result))
				{
					return result;
				}

				result = CreateFunction(module, token);
				FunctionLookup.Add(token, result);
				return result;
			}

			MetaType CreateType(ICorDebugModule module, MetaDataToken token)
			{
				if (token.TokenType == TokenType.TypeDef)
				{
					return CreateTypeDef(module, token);
				}
				else if (token.TokenType == TokenType.TypeRef)
				{
					return CreateTypeRef(module, token);
				}
				else
				{
					throw new TypeResolutionException();
				}
			}

			MetaType CreateTypeDef(ICorDebugModule module, MetaDataToken token)
			{
				var import = module.GetMetaDataImport();

				import.GetTypeDefProps(
					token,
					null,
					0,
					out var size,
					out var att,
					out var baseToken);

				var buffer = ArrayPool<char>.Shared.Rent(size);

				import.GetTypeDefProps(
					token,
					buffer,
					size,
					out size,
					out att,
					out baseToken);

				var name = new string(buffer, 0, size - 1);
				ArrayPool<char>.Shared.Return(buffer);

				MetaType declaringType = null;

				if (att.IsNested())
				{
					import.GetNestedClassProps(token, out var declaringToken);

					if (!declaringToken.IsNil)
					{
						declaringType = CreateType(module, declaringToken);
					}
				}

				var count = import.CountGenericParams(token);

				return new MetaType(Module, declaringType, name, count);
			}

			MetaType CreateTypeRef(ICorDebugModule module, MetaDataToken token)
			{
				return Provider.GetType(module.ResolveClassRef(token));
			}

			MetaFunction CreateFunction(ICorDebugModule module, MetaDataToken token)
			{
				var import = module.GetMetaDataImport();

				import.GetMethodProps(
					token,
					out var declaringType,
					null,
					0,
					out var size,
					IntPtr.Zero,
					out var sigBlob,
					out var sigLen,
					out var rva,
					IntPtr.Zero);

				var buffer = ArrayPool<char>.Shared.Rent(size);

				import.GetMethodProps(
					token,
					out declaringType,
					buffer,
					size,
					out size,
					IntPtr.Zero,
					out sigBlob,
					out sigLen,
					out rva,
					IntPtr.Zero);

				var name = new string(buffer, 0, size - 1);
				ArrayPool<char>.Shared.Return(buffer);

				var count = import.CountGenericParams(token);
				var sig = Read(sigBlob, sigLen);
				var converter = new SigTranslator(this, module);

				ImmutableArray<MetaParameter> parameters;

				if (sig.Parameters.Length > 0)
				{
					var builder = ImmutableArray.CreateBuilder<MetaParameter>(sig.Parameters.Length);
					builder.Count = builder.Capacity;

					for (var i = 0; i < builder.Count; i++)
					{
						builder[i] = new MetaParameter(
							GetParameterName(import, token, i + 1),
							converter.Visit(sig.Parameters[i]));
					}

					parameters = builder.MoveToImmutable();
				}
				else
				{
					parameters = ImmutableArray<MetaParameter>.Empty;
				}

				return new MetaFunction(Module, GetType(module, declaringType), token, name, count, sig.CallingConvention, parameters);
			}

			static SigMethod Read(IntPtr sigBlob, int sigLen)
			{
				var blob = ArrayPool<byte>.Shared.Rent(sigLen);
				Marshal.Copy(sigBlob, blob, 0, sigLen);

				var sig = SignatureReader.ReadMethodDefSig(new ArraySegment<byte>(blob, 0, sigLen));
				ArrayPool<byte>.Shared.Return(blob);

				if (sig.Parameters.Length > sig.OrderedParamCount)
				{
					// Optional parameters should only be specified at call sites, NOT in the definition.
					throw new InvalidOperationException();
				}

				return sig;
			}

			static string GetParameterName(IMetaDataImport import, MetaDataToken token, int index)
			{
				var hr = import.GetParamForMethodIndex(token, (uint)index, out var paramToken);

				if (hr == (int)HResults.CLDB_E_RECORD_NOTFOUND)
				{
					return null;
				}
				else if (hr < 0)
				{
					throw Marshal.GetExceptionForHR(hr);
				}

				import.GetParamProps(
					paramToken,
					IntPtr.Zero,
					IntPtr.Zero,
					null,
					0,
					out var size,
					IntPtr.Zero,
					IntPtr.Zero,
					IntPtr.Zero,
					IntPtr.Zero);

				if (size <= 1)
				{
					return null;
				}

				var buffer = ArrayPool<char>.Shared.Rent(size);

				import.GetParamProps(
					paramToken,
					IntPtr.Zero,
					IntPtr.Zero,
					buffer,
					size,
					out size,
					IntPtr.Zero,
					IntPtr.Zero,
					IntPtr.Zero,
					IntPtr.Zero);

				var name = new string(buffer, 0, size - 1);
				ArrayPool<char>.Shared.Return(buffer);
				return name;
			}
		}

		sealed class SigTranslator : ISigTypeVisitor
		{
			public SigTranslator(ModuleLevelCache cache, ICorDebugModule module)
			{
				_cache = cache;
				_module = module;
			}

			public MetaCompound Visit(SigParameter type)
			{
				var result = Visit(type.ValueType);
				return type.ByRef ? new MetaCompoundByRef(result) : result;
			}

			public MetaCompound Visit(SigType type)
			{
				_result = null;

				try
				{
					type.Apply(this);
					return _result;
				}
				finally
				{
					_result = null;
				}
			}

			void ISigTypeVisitor.Visit(SigTypePrimitive element)
			{
				_result = new MetaCompoundClass(
					_cache.Provider.GetType(_module, element.ElementType),
					ImmutableArray<MetaCompound>.Empty);
			}

			void ISigTypeVisitor.Visit(SigTypeGen element)
			{
				switch (element.ElementType)
				{
					case CorElementType.ELEMENT_TYPE_MVAR:
						_result = new MetaCompoundGenArg(true, (int)element.Index);
						break;

					case CorElementType.ELEMENT_TYPE_VAR:
						_result = new MetaCompoundGenArg(false, (int)element.Index);
						break;

					default:
						throw new TypeResolutionException();
				}
			}

			void ISigTypeVisitor.Visit(SigTypeUserType element)
			{
				_result = new MetaCompoundClass(
					_cache.GetType(_module, element.Token),
					ImmutableArray<MetaCompound>.Empty);
			}

			void ISigTypeVisitor.Visit(SigTypePointer element)
			{
				element.BaseType.Apply(this);
				_result = new MetaCompoundPointer(_result);
			}

			void ISigTypeVisitor.Visit(SigTypeGenericInst element)
			{
				var args = ImmutableArray.CreateBuilder<MetaCompound>(element.GenArguments.Length);
				args.Count = args.Capacity;

				var arguments = element.GenArguments;

				for (var i = 0; i < arguments.Length; i++)
				{
					arguments[i].Apply(this);
					args[i] = _result;
				}

				_result = new MetaCompoundClass(
					_cache.GetType(_module, element.Template.Token),
					args.MoveToImmutable());
			}

			void ISigTypeVisitor.Visit(SigTypeArray element)
			{
				element.Apply(this);
				_result = new MetaCompoundArray(_result, (int)element.Rank);
			}

			void ISigTypeVisitor.Visit(SigTypeSZArray element)
			{
				element.BaseType.Apply(this);
				_result = new MetaCompoundArray(_result, 1);
			}

			void ISigTypeVisitor.Visit(SigTypeFNPtr element)
			{
				_result = new MetaCompoundClass(
					_cache.Provider.GetType(_module, CorElementType.ELEMENT_TYPE_I),
					ImmutableArray<MetaCompound>.Empty);
			}

			readonly ModuleLevelCache _cache;
			readonly ICorDebugModule _module;
			MetaCompound _result;
		}
	}
}
