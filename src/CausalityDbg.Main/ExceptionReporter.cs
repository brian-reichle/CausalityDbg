// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;

namespace CausalityDbg.Main
{
	sealed class ExceptionReporter
	{
		public static void Report(Exception ex)
		{
			ErrorLogWindow.Log(
				new ErrorItemModel(
					ex.Message,
					ex.ToString()));

			WriteError(DateTimeOffset.Now, new StackTrace(1, true), ex);
		}

		static FileStream CreateNew(string filename)
		{
			var offset = 0;
			string baseName = null;
			string ext = null;

			do
			{
				if (!File.Exists(filename))
				{
					try
					{
						return new FileStream(filename, FileMode.CreateNew, FileAccess.Write);
					}
					catch (IOException)
					{
					}
				}

				if (offset == 0)
				{
					baseName = Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename) + "-");
					ext = Path.GetExtension(filename);
				}

				offset++;

				filename = baseName + offset + ext;
			}
			while (true);
		}

		static void WriteError(DateTimeOffset timestamp, StackTrace stackTrace, Exception ex)
		{
			const string ErrorLogPathName = "ErrorLog";

			if (Directory.Exists(ErrorLogPathName))
			{
				var filename = Path.Combine(ErrorLogPathName, string.Format(CultureInfo.InvariantCulture, "{0:yyyyMMddhhmmss}.xml", timestamp));

				using (var stream = CreateNew(filename))
				{
					WriteError(stream, timestamp, stackTrace, ex);
				}
			}
		}

		static void WriteError(Stream stream, DateTimeOffset timestamp, StackTrace stackTrace, Exception ex)
		{
			var settings = new XmlWriterSettings()
			{
				Encoding = Encoding.UTF8,
				Indent = true,
				IndentChars = "  ",
				NamespaceHandling = NamespaceHandling.OmitDuplicates,
			};

			using (var writer = XmlWriter.Create(stream, settings))
			{
				WriteError(writer, timestamp, stackTrace, ex);
			}
		}

		static void WriteError(XmlWriter writer, DateTimeOffset timestamp, StackTrace stackTrace, Exception ex)
		{
			writer.WriteStartDocument();
			writer.WriteStartElement("Error");
			writer.WriteElementString("Timestamp", timestamp.ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture));
			writer.WriteElementString("Offset", timestamp.Offset.ToString("hh\\:mm", CultureInfo.InvariantCulture));

			var moduleIDs = new Dictionary<Module, int>();

			writer.WriteStartElement("Environment");
			WriteEnvironment(writer);
			writer.WriteEndElement();

			writer.WriteStartElement("Assemblies");
			WriteAssemblies(writer, moduleIDs);
			writer.WriteEndElement();

			writer.WriteStartElement("StackTrace");
			WriteStackTrace(writer, stackTrace, moduleIDs);
			writer.WriteEndElement();

			writer.WriteStartElement("Exception");
			WriteException(writer, ex, moduleIDs);
			writer.WriteEndElement();

			writer.WriteEndElement();
			writer.WriteEndDocument();
		}

		static void WriteEnvironment(XmlWriter writer)
		{
			writer.WriteElementString("Is64BitOS", Environment.Is64BitOperatingSystem.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("Is64BitProcess", Environment.Is64BitProcess.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("OsVersion", Environment.OSVersion.ToString());
			writer.WriteElementString("ClrVersion", Environment.Version.ToString());
			writer.WriteElementString("Culture", CultureInfo.CurrentCulture.Name);
		}

		static void WriteAssemblies(XmlWriter writer, Dictionary<Module, int> moduleIDs)
		{
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				writer.WriteStartElement("Assembly");
				writer.WriteAttributeString("name", assembly.FullName);

				var modules = assembly.GetLoadedModules();

				for (var i = 0; i < modules.Length; i++)
				{
					var id = moduleIDs.Count;
					var module = modules[i];

					moduleIDs.Add(module, id);

					writer.WriteStartElement("Module");
					writer.WriteAttributeString("id", id.ToString(CultureInfo.InvariantCulture));
					writer.WriteString(module.Name);
					writer.WriteEndElement();
				}

				writer.WriteEndElement();
			}
		}

		static void WriteException(XmlWriter writer, Exception ex, Dictionary<Module, int> moduleIDs)
		{
			writer.WriteElementString("Message", ex.Message);
			writer.WriteElementString("Type", ex.GetType().ToString());
			WriteExceptionSpecificDetails(writer, ex);
			writer.WriteStartElement("StackTrace");

			var trace = new StackTrace(ex, true);
			var lines = ex.StackTrace.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

			WriteStackTrace(writer, lines, lines.Length - trace.FrameCount);
			WriteStackTrace(writer, trace, moduleIDs);
			writer.WriteEndElement();

			if (ex.InnerException != null)
			{
				writer.WriteStartElement("InnerException");
				WriteException(writer, ex.InnerException, moduleIDs);
				writer.WriteEndElement();
			}
		}

		static void WriteExceptionSpecificDetails(XmlWriter writer, Exception ex)
		{
			switch (ex)
			{
				case Win32Exception win32Ex:
					WriteWin32ExceptionSpecificDetails(writer, win32Ex);
					break;

				case ExternalException exEx:
					WriteExternalExceptionSpecificDetails(writer, exEx);
					break;
			}
		}

		static void WriteExternalExceptionSpecificDetails(XmlWriter writer, ExternalException ex)
		{
			writer.WriteElementString("ErrorCode", ex.ErrorCode.ToString("X8", CultureInfo.InvariantCulture));
		}

		static void WriteWin32ExceptionSpecificDetails(XmlWriter writer, Win32Exception ex)
		{
			WriteExternalExceptionSpecificDetails(writer, ex);
			writer.WriteElementString("NativeErrorCode", ex.NativeErrorCode.ToString("X8", CultureInfo.InvariantCulture));
		}

		static void WriteStackTrace(XmlWriter writer, string[] stackTrace, int count)
		{
			for (var i = 0; i < count; i++)
			{
				writer.WriteStartElement("Frame");
				var line = stackTrace[i];

				if (line.StartsWith("   at ", StringComparison.Ordinal))
				{
					line = line.Substring(6);
				}

				writer.WriteString(line);
				writer.WriteEndElement();
			}
		}

		static void WriteStackTrace(XmlWriter writer, StackTrace stackTrace, Dictionary<Module, int> moduleIDs)
		{
			for (var i = 0; i < stackTrace.FrameCount; i++)
			{
				writer.WriteStartElement("Frame");
				WriteStackFrame(writer, stackTrace.GetFrame(i), moduleIDs);
				writer.WriteEndElement();
			}
		}

		static void WriteStackFrame(XmlWriter writer, StackFrame frame, Dictionary<Module, int> moduleIDs)
		{
			var info = frame.GetMethod();
			var iloffset = frame.GetILOffset();

			if (moduleIDs.TryGetValue(info.Module, out var moduleId))
			{
				writer.WriteAttributeString("mid", moduleId.ToString(CultureInfo.InvariantCulture));
			}

			writer.WriteAttributeString("token", info.MetadataToken.ToString("X8", CultureInfo.InvariantCulture));

			if (iloffset != StackFrame.OFFSET_UNKNOWN)
			{
				writer.WriteAttributeString("il", iloffset.ToString(CultureInfo.InvariantCulture));
			}

			var builder = new StringBuilder();
			AppendMethod(builder, info);

			var filename = frame.GetFileName();

			if (!string.IsNullOrEmpty(filename))
			{
				builder.Append(' ');
				builder.Append(filename);
				builder.Append(":line ");
				builder.Append(frame.GetFileLineNumber());
			}

			writer.WriteString(builder.ToString());
		}

		static void AppendMethod(StringBuilder builder, MethodBase info)
		{
			if (info.DeclaringType != null)
			{
				AppendType(builder, info.DeclaringType, true);
				builder.Append('.');
			}

			builder.Append(info.Name);
			builder.Append('(');

			var parameters = info.GetParameters();

			if (parameters.Length > 0)
			{
				AppendParameter(builder, parameters[0]);

				for (var i = 1; i < parameters.Length; i++)
				{
					builder.Append(", ");
					AppendParameter(builder, parameters[i]);
				}
			}

			builder.Append(')');
		}

		static void AppendParameter(StringBuilder builder, ParameterInfo info)
		{
			AppendType(builder, info.ParameterType, false);

			if (!string.IsNullOrEmpty(info.Name))
			{
				builder.Append(' ');
				builder.Append(info.Name);
			}
		}

		static void AppendType(StringBuilder builder, Type type, bool includeNamespace)
		{
			if (type.IsPointer)
			{
				AppendType(builder, type.GetElementType(), includeNamespace);
				builder.Append('*');
			}
			else if (type.IsByRef)
			{
				AppendType(builder, type.GetElementType(), includeNamespace);
				builder.Append('&');
			}
			else if (type.IsArray)
			{
				var ranks = new Stack<int>();

				do
				{
					ranks.Push(type.GetArrayRank());
					type = type.GetElementType();
				}
				while (type.IsArray);

				AppendType(builder, type, includeNamespace);

				while (ranks.Count > 0)
				{
					builder.Append('[');
					builder.Append(',', ranks.Pop() - 1);
					builder.Append(']');
				}
			}
			else if (type.IsGenericParameter)
			{
				builder.Append(type.Name);
			}
			else
			{
				AppendTypeCore(builder, type, includeNamespace);
			}
		}

		static void AppendTypeCore(StringBuilder builder, Type type, bool includeNamespace)
		{
			if (type.IsNested)
			{
				AppendTypeCore(builder, type.DeclaringType, includeNamespace);
				builder.Append('.');
				builder.Append(type.Name);
			}
			else if (includeNamespace)
			{
				builder.Append(type.FullName);
			}
			else
			{
				builder.Append(type.Name);
			}
		}
	}
}
