// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace CausalityDbg.Metadata
{
	public sealed class MetaFormatter : IMetaCompoundVisitor
	{
		public static string Format(MetaFunction function, ImmutableArray<MetaCompound> genericArgs)
		{
			var builder = new StringBuilder();
			var declType = function.DeclaringType;
			var typeArgCount = declType == null ? 0 : declType.GenTypeArgs;
			var visitor = new MetaFormatter(builder, genericArgs, typeArgCount);

			if (declType != null)
			{
				visitor.AppendMetaType(declType, genericArgs);
				builder.Append('.');
			}

			builder.Append(function.Name);
			visitor.AppendGenericArgs(genericArgs, typeArgCount, function.GenTypeArgs);
			visitor.AppendParameters(function.Parameters);

			return builder.ToString();
		}

		public static string Format(MetaCompound compound)
		{
			var builder = new StringBuilder();
			compound.Apply(new MetaFormatter(builder, ImmutableArray<MetaCompound>.Empty, 0));
			return builder.ToString();
		}

		void AppendMetaType(MetaType type, IList<MetaCompound> overrideGenArgs)
		{
			var declType = type.DeclaringType;
			var baseArgCount = 0;

			if (declType != null)
			{
				AppendMetaType(declType, overrideGenArgs);
				_builder.Append('.');
				baseArgCount = declType.GenTypeArgs;
			}

			int index;

			if (baseArgCount < type.GenTypeArgs && (index = type.Name.LastIndexOf('`')) >= 0)
			{
				_builder.Append(type.Name, 0, index);
			}
			else
			{
				_builder.Append(type.Name);
			}

			AppendGenericArgs(overrideGenArgs, baseArgCount, type.GenTypeArgs - baseArgCount);
		}

		void AppendGenericArgs(IList<MetaCompound> overrideGenArgs, int start, int count)
		{
			if (count > 0)
			{
				_builder.Append('<');
				overrideGenArgs[start].Apply(this);

				for (var i = 1; i < count; i++)
				{
					_builder.Append(", ");
					overrideGenArgs[i + start].Apply(this);
				}

				_builder.Append('>');
			}
		}

		void AppendParameters(IList<MetaParameter> parameters)
		{
			_builder.Append('(');

			if (parameters.Count > 0)
			{
				AppendParameter(parameters, 0);

				for (var i = 1; i < parameters.Count; i++)
				{
					_builder.Append(", ");
					AppendParameter(parameters, i);
				}
			}

			_builder.Append(')');
		}

		void AppendParameter(IList<MetaParameter> parameters, int index)
		{
			var parameter = parameters[index];

			parameter.ParameterType.Apply(this);

			if (!string.IsNullOrEmpty(parameter.Name))
			{
				_builder.Append(' ');
				_builder.Append(parameter.Name);
			}
		}

		MetaFormatter(StringBuilder builder, ImmutableArray<MetaCompound> genericArgs, int methodArgsStart)
		{
			_builder = builder;
			_genericArgs = genericArgs;
			_methodArgsStart = methodArgsStart;
		}

		void IMetaCompoundVisitor.Visit(MetaCompoundArray type)
		{
			type.TargetType.Apply(this);
			_builder.Append('[');
			_builder.Append(',', type.Rank - 1);
			_builder.Append(']');
		}

		void IMetaCompoundVisitor.Visit(MetaCompoundByRef type)
		{
			type.TargetType.Apply(this);
			_builder.Append('&');
		}

		void IMetaCompoundVisitor.Visit(MetaCompoundClass type)
		{
			AppendMetaType(type.TargetType, type.GenericArgs);
		}

		void IMetaCompoundVisitor.Visit(MetaCompoundGenArg type)
		{
			var index = type.Method ? type.Index + _methodArgsStart : type.Index;
			_genericArgs[index].Apply(this);
		}

		void IMetaCompoundVisitor.Visit(MetaCompoundPointer type)
		{
			type.TargetType.Apply(this);
			_builder.Append('*');
		}

		readonly StringBuilder _builder;
		readonly ImmutableArray<MetaCompound> _genericArgs;
		readonly int _methodArgsStart;
	}
}
