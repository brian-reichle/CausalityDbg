// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Generic;
using System.Text;

namespace CausalityDbg.Metadata
{
	public sealed class MetaIDStringFormatter : IMetaCompoundVisitor
	{
		public MetaIDStringFormatter()
		{
			_builder = new StringBuilder();
		}

		public override string ToString()
		{
			return _builder.ToString();
		}

		public void AppendFunction(MetaFunction function)
		{
			_builder.Append("M:");

			if (function.DeclaringType != null)
			{
				AppendType(function.DeclaringType);
				_builder.Append('.');
			}

			AppendEscaped(function.Name);

			if (function.GenTypeArgs > 0)
			{
				_builder.Append("``");
				_builder.Append(function.GenTypeArgs);
			}

			if (function.Parameters.Length > 0)
			{
				_builder.Append('(');

				function.Parameters[0].ParameterType.Apply(this);

				for (var i = 1; i < function.Parameters.Length; i++)
				{
					_builder.Append(',');
					function.Parameters[i].ParameterType.Apply(this);
				}

				_builder.Append(')');
			}
		}

		void AppendType(MetaType type)
		{
			var usedGenCount = 0;

			if (type.DeclaringType != null)
			{
				usedGenCount = type.DeclaringType.GenTypeArgs;
				AppendType(type.DeclaringType);
				_builder.Append('.');
			}

			if (_typeArgs == null)
			{
				_builder.Append(type.Name);
			}
			else
			{
				var index = type.Name.LastIndexOf('`');
				_builder.Append(type.Name, 0, index < 0 ? type.Name.Length : index);

				if (usedGenCount < type.GenTypeArgs)
				{
					_builder.Append('{');

					_typeArgs[usedGenCount++].Apply(this);

					for (; usedGenCount < type.GenTypeArgs; usedGenCount++)
					{
						_builder.Append(',');
						_typeArgs[usedGenCount++].Apply(this);
					}

					_builder.Append('}');
				}
			}
		}

		void AppendEscaped(string text)
		{
			AppendEscaped(text, 0, text.Length);
		}

		void AppendEscaped(string text, int start, int length)
		{
			_builder.EnsureCapacity(_builder.Length + length);

			var end = start + length;

			for (var i = start; i < end; i++)
			{
				var c = text[i];

				if (c == '.')
				{
					if (start < i)
					{
						_builder.Append(text, start, i - start);
					}

					_builder.Append('#');
					start = i + 1;
				}
			}

			if (start < end)
			{
				_builder.Append(text, start, end - start);
			}
		}

		#region IMetaCompoundVisitor Members

		void IMetaCompoundVisitor.Visit(MetaCompoundArray type)
		{
			type.TargetType.Apply(this);
			_builder.Append('[');

			if (type.Rank > 1)
			{
				_builder.Append("0:");

				for (var i = 1; i < type.Rank; i++)
				{
					_builder.Append(",0:");
				}
			}

			_builder.Append(']');
		}

		void IMetaCompoundVisitor.Visit(MetaCompoundByRef type)
		{
			type.TargetType.Apply(this);
			_builder.Append('@');
		}

		void IMetaCompoundVisitor.Visit(MetaCompoundClass type)
		{
			var tmp = _typeArgs;

			try
			{
				_typeArgs = type.GenericArgs;
				AppendType(type.TargetType);
			}
			finally
			{
				_typeArgs = tmp;
			}
		}

		void IMetaCompoundVisitor.Visit(MetaCompoundGenArg type)
		{
			_builder.Append(type.Method ? "``" : "`");
			_builder.Append(type.Index);
		}

		void IMetaCompoundVisitor.Visit(MetaCompoundPointer type)
		{
			type.TargetType.Apply(this);
			_builder.Append('*');
		}

		#endregion

		IList<MetaCompound> _typeArgs;
		readonly StringBuilder _builder;
	}
}
