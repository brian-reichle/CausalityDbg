// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using CausalityDbg.Core.CorDebugApi;
using CausalityDbg.Core.MetaDataApi;
using CausalityDbg.IL;
using CausalityDbg.Metadata;

namespace CausalityDbg.Core
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
					return GetType(module, type.GetType()).CreateCompound();

				case CorElementType.ELEMENT_TYPE_PTR:
					return GetCompound(module, type.GetFirstTypeParameter()).CreatePointer();

				case CorElementType.ELEMENT_TYPE_BYREF:
					return GetCompound(module, type.GetFirstTypeParameter()).CreateByRef();

				case CorElementType.ELEMENT_TYPE_SZARRAY:
					return GetCompound(module, type.GetFirstTypeParameter()).CreateArray(1);

				case CorElementType.ELEMENT_TYPE_ARRAY:
					return GetCompound(module, type.GetFirstTypeParameter()).CreateArray(type.GetRank());

				case CorElementType.ELEMENT_TYPE_CLASS:
				case CorElementType.ELEMENT_TYPE_VALUETYPE:
					return GetType(type.GetClass()).CreateCompound(GetCompounds(module, type.EnumerateTypeParameters()));

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

			result = new ModuleLevelCache(this, MetaFactory.CreateModule(name, GetFlags(module)));
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
				Translator = new SigTranslator(this);
			}

			public MetaModule Module { get; }
			public MetaProvider Provider { get; }
			public SigTranslator Translator { get; }
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

				var count = import.CountGenericParams(token);

				if (att.IsNested())
				{
					import.GetNestedClassProps(token, out var declaringToken);
					return MetaFactory.CreateType(CreateType(module, declaringToken), name, count);
				}
				else
				{
					return MetaFactory.CreateType(Module, name, count);
				}
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

				ImmutableArray<MetaParameter> parameters;

				if (sig.Parameters.Length > 0)
				{
					var builder = ImmutableArray.CreateBuilder<MetaParameter>(sig.Parameters.Length);
					builder.Count = builder.Capacity;

					for (var i = 0; i < builder.Count; i++)
					{
						builder[i] = Translator.Visit(sig.Parameters[i], module)
							.ToParam(GetParameterName(import, token, i + 1));
					}

					parameters = builder.MoveToImmutable();
				}
				else
				{
					parameters = ImmutableArray<MetaParameter>.Empty;
				}

				return GetType(module, declaringType)
					.CreateFunction(token, name, count, sig.CallingConvention, parameters);
			}

			static SigMethod Read(IntPtr sigBlob, int sigLen)
			{
				var sig = SignatureReader.ReadMethodDefSig(SpanUtils.Create<byte>(sigBlob, sigLen));

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

		sealed class SigTranslator : ISigTypeVisitor<ICorDebugModule, MetaCompound>
		{
			public SigTranslator(ModuleLevelCache cache)
			{
				_cache = cache;
			}

			public MetaCompound Visit(SigParameter type, ICorDebugModule module)
			{
				var result = type.ValueType.Apply(this, module);

				if (type.ByRef)
				{
					result = result.CreateByRef();
				}

				return result;
			}

			public MetaCompound Visit(SigTypePrimitive element, ICorDebugModule module)
				=> GetType(module, element.ElementType).CreateCompound();

			public MetaCompound Visit(SigTypeGen element, ICorDebugModule module)
			{
				bool method;

				switch (element.ElementType)
				{
					case CorElementType.ELEMENT_TYPE_MVAR:
						method = true;
						break;

					case CorElementType.ELEMENT_TYPE_VAR:
						method = false;
						break;

					default:
						throw new TypeResolutionException();
				}

				return MetaFactory.CreateTypeArg(method, (int)element.Index);
			}

			public MetaCompound Visit(SigTypeUserType element, ICorDebugModule module)
				=> GetType(module, element.Token).CreateCompound();

			public MetaCompound Visit(SigTypePointer element, ICorDebugModule module)
				=> element.BaseType.Apply(this, module).CreatePointer();

			public MetaCompound Visit(SigTypeGenericInst element, ICorDebugModule module)
			{
				var args = ImmutableArray.CreateBuilder<MetaCompound>(element.GenArguments.Length);
				args.Count = args.Capacity;

				var arguments = element.GenArguments;

				for (var i = 0; i < arguments.Length; i++)
				{
					args[i] = arguments[i].Apply(this, module);
				}

				return GetType(module, element.Template.Token).CreateCompound(args.MoveToImmutable());
			}

			public MetaCompound Visit(SigTypeArray element, ICorDebugModule module)
				=> element.BaseType.Apply(this, module).CreateArray((int)element.Rank);

			public MetaCompound Visit(SigTypeSZArray element, ICorDebugModule module)
				=> element.BaseType.Apply(this, module).CreateArray(1);

			public MetaCompound Visit(SigTypeFNPtr element, ICorDebugModule module)
				=> GetType(module, CorElementType.ELEMENT_TYPE_I).CreateCompound();

			MetaType GetType(ICorDebugModule module, MetaDataToken token) => _cache.GetType(module, token);
			MetaType GetType(ICorDebugModule module, CorElementType element) => _cache.Provider.GetType(module, element);

			readonly ModuleLevelCache _cache;
		}
	}
}
