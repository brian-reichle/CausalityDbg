// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System.Collections.Immutable;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Xml;
using CausalityDbg.IL;

namespace CausalityDbg.Tests
{
	sealed class SigFormatter : ISigTypeVisitor<XmlWriter, XmlWriter>
	{
		public static SigFormatter Instance { get; } = new SigFormatter();

		SigFormatter()
		{
		}

		public XmlWriter Visit(SigMethod element, XmlWriter writer)
		{
			writer.WriteStartElement("Method");

			if (element.CallingConvention != CallingConventions.Standard)
			{
				writer.WriteAttributeString("callingConvention", element.CallingConvention.ToString());
			}

			if (element.GenParamCount != 0)
			{
				writer.WriteAttributeString("genericParameterCount", element.GenParamCount.ToString(CultureInfo.InvariantCulture));
			}

			writer.WriteStartElement("Return");
			Visit(element.RetType, writer);
			writer.WriteEndElement();

			for (var i = 0; i < element.Parameters.Length; i++)
			{
				var param = element.Parameters[i];
				writer.WriteStartElement(i < element.OrderedParamCount ? "Param" : "VarParam");
				Visit(param, writer);
				writer.WriteEndElement();
			}

			writer.WriteEndElement();
			return writer;
		}

		public XmlWriter Visit(SigParameter element, XmlWriter writer)
		{
			writer.WriteStartElement("TypeRef");

			if (element.ByRef)
			{
				writer.WriteAttributeString("byRef", "True");
			}

			Visit(writer, element.CustomModifiers, element.ByRefIndex);
			element.ValueType.Apply(this, writer);
			writer.WriteEndElement();
			return writer;
		}

		public XmlWriter Visit(SigTypePrimitive element, XmlWriter writer)
		{
			writer.WriteElementString("Primitive", element.ElementType.ToString());
			return writer;
		}

		public XmlWriter Visit(SigTypeGen element, XmlWriter writer)
		{
			if (element.ElementType == CorElementType.ELEMENT_TYPE_VAR)
			{
				writer.WriteStartElement("TypeGenArg");
			}
			else
			{
				writer.WriteStartElement("MethodGenArg");
			}

			writer.WriteString(element.Index.ToString(CultureInfo.InvariantCulture));
			writer.WriteEndElement();
			return writer;
		}

		public XmlWriter Visit(SigTypeUserType element, XmlWriter writer)
		{
			if (element.ElementType == CorElementType.ELEMENT_TYPE_CLASS)
			{
				writer.WriteStartElement("Class");
			}
			else
			{
				writer.WriteStartElement("ValueType");
			}

			WriteToken(writer, element.Token);

			writer.WriteEndElement();
			return writer;
		}

		public XmlWriter Visit(SigTypePointer element, XmlWriter writer)
		{
			writer.WriteStartElement("Pointer");
			Visit(writer, element.CustomModifiers, element.CustomModifiers.Length);
			element.BaseType.Apply(this, writer);
			writer.WriteEndElement();
			return writer;
		}

		public XmlWriter Visit(SigTypeGenericInst element, XmlWriter writer)
		{
			writer.WriteStartElement("GenericInst");
			writer.WriteStartElement("Template");
			Visit(element.Template, writer);
			writer.WriteEndElement();

			foreach (var type in element.GenArguments)
			{
				writer.WriteStartElement("TypeArg");
				type.Apply(this, writer);
				writer.WriteEndElement();
			}

			writer.WriteEndElement();
			return writer;
		}

		public XmlWriter Visit(SigTypeArray element, XmlWriter writer)
		{
			writer.WriteStartElement("Array");
			writer.WriteAttributeString("rank", FormatBounds(element));
			element.BaseType.Apply(this, writer);
			writer.WriteEndElement();
			return writer;
		}

		public XmlWriter Visit(SigTypeSZArray element, XmlWriter writer)
		{
			writer.WriteStartElement("SZArray");
			Visit(writer, element.CustomModifiers, element.CustomModifiers.Length);
			element.BaseType.Apply(this, writer);
			writer.WriteEndElement();
			return writer;
		}

		public XmlWriter Visit(SigTypeFNPtr element, XmlWriter writer)
		{
			writer.WriteStartElement("FNPtr");
			Visit(element.Method, writer);
			writer.WriteEndElement();
			return writer;
		}

		static void Visit(XmlWriter writer, ImmutableArray<SigCustomModifier> modifiers, int byRefIndex)
		{
			foreach (var modifier in modifiers)
			{
				if (byRefIndex == 0)
				{
					writer.WriteComment(" ByRef Marker ");
				}

				byRefIndex--;

				if (modifier.ElementType == CorElementType.ELEMENT_TYPE_CMOD_OPT)
				{
					writer.WriteStartElement("ModOpt");
				}
				else
				{
					writer.WriteStartElement("ModReq");
				}

				WriteToken(writer, modifier.Token);
				writer.WriteEndElement();
			}
		}

		static void WriteToken(XmlWriter writer, MetaDataToken token)
		{
			writer.WriteString(token.ToString());
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
	}
}
