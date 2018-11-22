// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Xml;

namespace CausalityDbg.Core
{
	public static class ConfigParser
	{
		internal const string Namespace = "CausalityDbg";

		public static Config Load(string path)
		{
			using (var file = new StreamReader(path))
			{
				return ConfigParser.Load(file);
			}
		}

		static Config Load(TextReader reader)
		{
			var settings = new XmlReaderSettings()
			{
				CloseInput = false,
				ConformanceLevel = ConformanceLevel.Document,
				IgnoreWhitespace = true,
				IgnoreComments = true,
			};

			try
			{
				using (var xmlReader = XmlReader.Create(reader, settings))
				{
					xmlReader.MoveToContent();

					if (xmlReader.LocalName == "config" && xmlReader.NamespaceURI == Namespace)
					{
						return ConfigParser.LoadConfig(xmlReader);
					}
					else
					{
						throw new ConfigFormatException("Invalid root eleemnt");
					}
				}
			}
			catch (XmlException ex)
			{
				throw new ConfigFormatException(ex.Message, ex);
			}
		}

		static Config LoadConfig(XmlReader reader)
		{
			if (reader.IsEmptyElement)
			{
				reader.Skip();

				var builder = ImmutableArray.CreateBuilder<ConfigCategory>((int)ConfigCategoryType.Custom);
				builder.Count = (int)ConfigCategoryType.Custom;
				PopulateDefaultCategories(builder);

				return new Config(
					ImmutableArray<ConfigAssembly>.Empty,
					ImmutableArray<ConfigCategory>.Empty,
					builder.MoveToImmutable());
			}
			else
			{
				reader.ReadStartElement();

				var categoryLookup = ReadCategories(reader);
				var systemTriggers = ReadSystemTriggers(reader, categoryLookup);
				var assemblies = ImmutableArray.CreateBuilder<ConfigAssembly>();

				while (reader.LocalName == "assembly" && reader.NamespaceURI == Namespace)
				{
					assemblies.Add(LoadAssembly(reader, categoryLookup));
				}

				var categories = new ConfigCategory[categoryLookup.Count];
				categoryLookup.Values.CopyTo(categories, 0);
				Array.Sort(categories, (c1, c2) => string.Compare(c1.Code, c2.Code, StringComparison.Ordinal));

				reader.ReadEndElement();

				return new Config(
					assemblies.ToImmutable(),
					categories.ToImmutableArray(),
					systemTriggers);
			}
		}

		static Dictionary<string, ConfigCategory> ReadCategories(XmlReader reader)
		{
			var categories = new Dictionary<string, ConfigCategory>();

			while (reader.LocalName == "category" && reader.NamespaceURI == Namespace)
			{
				var category = LoadCategory(reader);
				categories.Add(category.Code, category);
			}

			return categories;
		}

		static ImmutableArray<ConfigCategory> ReadSystemTriggers(XmlReader reader, Dictionary<string, ConfigCategory> categories)
		{
			const int count = (int)ConfigCategoryType.Custom;
			var systemTriggers = ImmutableArray.CreateBuilder<ConfigCategory>(count);
			systemTriggers.Count = count;

			while (reader.LocalName == "systemTrigger" && reader.NamespaceURI == Namespace)
			{
				var type = (ConfigCategoryType)Enum.Parse(typeof(ConfigCategoryType), reader.GetAttribute("type"));
				var categoryName = reader.GetAttribute("category");
				var category = categories[categoryName];

				if (systemTriggers[(int)type] != null)
				{
					throw new ConfigFormatException();
				}
				else
				{
					systemTriggers[(int)type] = category;
				}

				reader.Skip();
			}

			PopulateDefaultCategories(systemTriggers);

			return systemTriggers.MoveToImmutable();
		}

		static void PopulateDefaultCategories(ImmutableArray<ConfigCategory>.Builder systemTriggers)
		{
			for (var i = 0; i < systemTriggers.Count; i++)
			{
				if (systemTriggers[i] == null)
				{
					systemTriggers[i] = new ConfigCategory((ConfigCategoryType)i, 0x000000, 0xDDDDDD);
				}
			}
		}

		static ConfigAssembly LoadAssembly(XmlReader reader, Dictionary<string, ConfigCategory> categories)
		{
			var assemblyRef = AssemblyRef.Parse(reader.GetAttribute("id"));

			if (reader.IsEmptyElement)
			{
				reader.Skip();
				return new ConfigAssembly(
					assemblyRef,
					ImmutableArray<ConfigClass>.Empty);
			}
			else
			{
				reader.ReadStartElement();

				var lookup = new ClassLookup();

				while (reader.NamespaceURI == Namespace)
				{
					if (reader.LocalName == "event")
					{
						LoadMethods(reader, ConfigHookType.Event, lookup, categories);
					}
					else if (reader.LocalName == "scope")
					{
						LoadMethods(reader, ConfigHookType.Scope, lookup, categories);
					}
					else
					{
						break;
					}
				}

				reader.ReadEndElement();

				return new ConfigAssembly(assemblyRef, AssembleClasses(lookup.RootClasses));
			}
		}

		static ImmutableArray<ConfigClass> AssembleClasses(IReadOnlyCollection<ClassData> classDataList)
		{
			if (classDataList.Count == 0)
			{
				return ImmutableArray<ConfigClass>.Empty;
			}

			var result = ImmutableArray.CreateBuilder<ConfigClass>(classDataList.Count);

			foreach (var data in classDataList)
			{
				var methods = AssembleMethods(data.Methods.Values);
				var classes = AssembleClasses(data.NestedClasses);
				result.Add(new ConfigClass(data.FullName, data.Name, methods, classes));
			}

			return result.MoveToImmutable();
		}

		static ImmutableArray<ConfigMethodRef> AssembleMethods(IReadOnlyCollection<MethodData> methodDataList)
		{
			if (methodDataList.Count == 0)
			{
				return ImmutableArray<ConfigMethodRef>.Empty;
			}

			var result = ImmutableArray.CreateBuilder<ConfigMethodRef>(methodDataList.Count);

			foreach (var data in methodDataList)
			{
				result.Add(new ConfigMethodRef(data.Name, data.Hooks.ToImmutableArray()));
			}

			return result.MoveToImmutable();
		}

		static ImmutableArray<ConfigCategory> CreateDefaultCategorySet()
		{
			const int count = (int)ConfigCategoryType.Custom;
			var systemTriggers = ImmutableArray.CreateBuilder<ConfigCategory>(count);

			for (var i = 0; i < systemTriggers.Count; i++)
			{
				if (systemTriggers[i] == null)
				{
					systemTriggers[i] = new ConfigCategory((ConfigCategoryType)i, 0x000000, 0xDDDDDD);
				}
			}

			return systemTriggers.MoveToImmutable();
		}

		static void LoadMethods(XmlReader reader, ConfigHookType type, ClassLookup lookup, IDictionary<string, ConfigCategory> categories)
		{
			var category = categories[reader.GetAttribute("category")];

			if (reader.IsEmptyElement)
			{
				reader.Skip();
			}
			else
			{
				reader.ReadStartElement();

				while (reader.LocalName == "methodRef" && reader.NamespaceURI == Namespace)
				{
					LoadMethodRef(reader, type, category, lookup);
				}

				reader.ReadEndElement();
			}
		}

		static ConfigCategory LoadCategory(XmlReader reader)
		{
			var code = reader.GetAttribute("code");
			var name = reader.GetAttribute("name");
			var foregroundColor = Color(reader.GetAttribute("fgColor"));
			var backgroundColor = Color(reader.GetAttribute("bgColor"));
			reader.Skip();

			return new ConfigCategory(code, name, foregroundColor, backgroundColor);
		}

		static void LoadMethodRef(XmlReader reader, ConfigHookType type, ConfigCategory category, ClassLookup lookup)
		{
			var className = reader.GetAttribute("class");
			var methodName = MethodRef.Parse(reader.GetAttribute("method"));
			string key = null;

			if (reader.IsEmptyElement)
			{
				reader.Skip();
			}
			else
			{
				reader.ReadStartElement();

				if (reader.LocalName == "key" && reader.NamespaceURI == Namespace)
				{
					key = reader.ReadElementString();
				}

				reader.ReadEndElement();
			}

			lookup
				.GetClass(className)
				.GetMethod(methodName)
				.Hooks
				.Add(new ConfigHook(type, category, key));
		}

		static int Color(string code)
		{
			var val = 0;

			for (var i = 0; i < code.Length; i++)
			{
				int nibble;
				var c = code[i];

				if (c >= '0' && c <= '9')
				{
					nibble = c - '0';
				}
				else if (c >= 'A' && c <= 'F')
				{
					nibble = 10 + c - 'A';
				}
				else if (c >= 'a' && c <= 'f')
				{
					nibble = 10 + c - 'a';
				}
				else
				{
					throw new ConfigFormatException();
				}

				val = (val << 4) | nibble;
			}

			return val;
		}

		sealed class ClassLookup
		{
			public ClassLookup()
			{
				RootClasses = new List<ClassData>();
				Lookup = new Dictionary<string, ClassData>();
			}

			public List<ClassData> RootClasses { get; }
			public Dictionary<string, ClassData> Lookup { get; }

			public ClassData GetClass(string className)
			{
				if (Lookup.TryGetValue(className, out var result))
				{
					return result;
				}

				var index = className.LastIndexOf("::", StringComparison.Ordinal);

				if (index < 0)
				{
					result = new ClassData(className, className);
					RootClasses.Add(result);
				}
				else
				{
					var parent = GetClass(className.Substring(0, index));
					result = new ClassData(className, className.Substring(index + 2));
					parent.NestedClasses.Add(result);
				}

				Lookup.Add(className, result);
				return result;
			}
		}

		sealed class ClassData
		{
			public ClassData(string fullName, string name)
			{
				FullName = fullName;
				Name = name;
				Methods = new Dictionary<string, MethodData>();
				NestedClasses = new List<ClassData>();
			}

			public string FullName { get; }
			public string Name { get; }
			public Dictionary<string, MethodData> Methods { get; }
			public List<ClassData> NestedClasses { get; }

			public MethodData GetMethod(MethodRef name)
			{
				if (!Methods.TryGetValue(name.Text, out var result))
				{
					result = new MethodData(name);
					Methods.Add(name.Text, result);
				}

				return result;
			}
		}

		sealed class MethodData
		{
			public MethodData(MethodRef name)
			{
				Name = name;
				Hooks = new List<ConfigHook>();
			}

			public MethodRef Name { get; }
			public List<ConfigHook> Hooks { get; }
		}
	}
}
