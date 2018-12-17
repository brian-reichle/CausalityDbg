// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Immutable;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Xml;
using CausalityDbg.IL;

namespace CausalityDbg.Tests
{
	class SigFormatter : ISigTypeVisitor
	{
		public SigFormatter(XmlWriter writer)
		{
			_writer = writer;
		}

		public void Visit(SigMethod element)
		{
			_writer.WriteStartElement("Method");

			if (element.CallingConvention != CallingConventions.Standard)
			{
				_writer.WriteAttributeString("callingConvention", element.CallingConvention.ToString());
			}

			if (element.GenParamCount != 0)
			{
				_writer.WriteAttributeString("genericParameterCount", element.GenParamCount.ToString(CultureInfo.InvariantCulture));
			}

			_writer.WriteStartElement("Return");
			Visit(element.RetType);
			_writer.WriteEndElement();

			for (var i = 0; i < element.Parameters.Length; i++)
			{
				var param = element.Parameters[i];
				_writer.WriteStartElement(i < element.OrderedParamCount ? "Param" : "VarParam");
				Visit(param);
				_writer.WriteEndElement();
			}

			_writer.WriteEndElement();
		}

		public void Visit(SigParameter element)
		{
			_writer.WriteStartElement("TypeRef");

			if (element.ByRef)
			{
				_writer.WriteAttributeString("byRef", "True");
			}

			Visit(element.CustomModifiers, element.ByRefIndex);
			element.ValueType.Apply(this);
			_writer.WriteEndElement();
		}

		public void Visit(SigTypePrimitive element)
		{
			_writer.WriteElementString("Primitive", element.ElementType.ToString());
		}

		public void Visit(SigTypeGen element)
		{
			if (element.ElementType == CorElementType.ELEMENT_TYPE_VAR)
			{
				_writer.WriteStartElement("TypeGenArg");
			}
			else
			{
				_writer.WriteStartElement("MethodGenArg");
			}

			_writer.WriteString(element.Index.ToString(CultureInfo.InvariantCulture));
			_writer.WriteEndElement();
		}

		public void Visit(SigTypeUserType element)
		{
			if (element.ElementType == CorElementType.ELEMENT_TYPE_CLASS)
			{
				_writer.WriteStartElement("Class");
			}
			else
			{
				_writer.WriteStartElement("ValueType");
			}

			WriteToken(element.Token);

			_writer.WriteEndElement();
		}

		public void Visit(SigTypePointer element)
		{
			_writer.WriteStartElement("Pointer");
			Visit(element.CustomModifiers, element.CustomModifiers.Length);
			element.BaseType.Apply(this);
			_writer.WriteEndElement();
		}

		public void Visit(SigTypeGenericInst element)
		{
			_writer.WriteStartElement("GenericInst");
			_writer.WriteStartElement("Template");
			Visit(element.Template);
			_writer.WriteEndElement();

			foreach (var type in element.GenArguments)
			{
				_writer.WriteStartElement("TypeArg");
				type.Apply(this);
				_writer.WriteEndElement();
			}

			_writer.WriteEndElement();
		}

		public void Visit(SigTypeArray element)
		{
			_writer.WriteStartElement("Array");
			_writer.WriteAttributeString("rank", FormatBounds(element));
			element.BaseType.Apply(this);
			_writer.WriteEndElement();
		}

		public void Visit(SigTypeSZArray element)
		{
			_writer.WriteStartElement("SZArray");
			Visit(element.CustomModifiers, element.CustomModifiers.Length);
			element.BaseType.Apply(this);
			_writer.WriteEndElement();
		}

		public void Visit(SigTypeFNPtr element)
		{
			_writer.WriteStartElement("FNPtr");
			Visit(element.Method);
			_writer.WriteEndElement();
		}

		void Visit(ImmutableArray<SigCustomModifier> modifiers, int byRefIndex)
		{
			foreach (var modifier in modifiers)
			{
				if (byRefIndex == 0)
				{
					_writer.WriteComment(" ByRef Marker ");
				}

				byRefIndex--;

				if (modifier.ElementType == CorElementType.ELEMENT_TYPE_CMOD_OPT)
				{
					_writer.WriteStartElement("ModOpt");
				}
				else
				{
					_writer.WriteStartElement("ModReq");
				}

				WriteToken(modifier.Token);
				_writer.WriteEndElement();
			}
		}

		void WriteToken(MetaDataToken token)
		{
			_writer.WriteString(token.ToString());
		}

		static string FormatBounds(SigTypeArray array)
		{
			var builder = new StringBuilder();
			builder.Append('[');
			WriteBounds(builder, array, 0);

			for (var i = 1; i < array.Rank; i++)
			{
				builder.Append(',');
				WriteBounds(builder, array, i);
			}

			builder.Append(']');
			return builder.ToString();
		}

		static void WriteBounds(StringBuilder builder, SigTypeArray array, int index)
		{
			var lowerBound = index < array.LowerBounds.Length ? array.LowerBounds[index] : 0;
			var size = index < array.Sizes.Length ? array.Sizes[index] : 0;

			if (lowerBound > 0)
			{
				builder.Append(lowerBound);
				builder.Append("...");

				if (size > 0)
				{
					builder.Append(lowerBound + size - 1);
				}
			}
			else if (size > 0)
			{
				builder.Append(size);
			}
		}

		readonly XmlWriter _writer;
	}
}
