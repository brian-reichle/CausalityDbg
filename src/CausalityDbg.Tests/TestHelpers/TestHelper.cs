// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace CausalityDbg.Tests
{
	static class TestHelper
	{
		public static TestCaseData[] GetResourceBasedTests(string prefix, string suffix)
		{
			var prefixLen = prefix.Length;
			var suffixLen = suffix.Length;

			var result = new List<TestCaseData>();

			foreach (var name in Assembly.GetExecutingAssembly().GetManifestResourceNames())
			{
				if (name.StartsWith(prefix, StringComparison.Ordinal))
				{
					result.Add(new TestCaseData(name)
						.SetName(name.Substring(prefixLen, name.Length - prefixLen - suffixLen)));
				}
			}

			result.Sort((x, y) => string.CompareOrdinal(x.TestName, y.TestName));

			return result.ToArray();
		}

		public static KeyValuePair<byte[], string> ReadBinaryTextPair(string name)
		{
			var source = GetResource(name);
			var index = 0;
			var buffer = ReadHex(source, ref index);
			return new KeyValuePair<byte[], string>(buffer, source.Substring(index));
		}

		static byte[] ReadHex(string source, ref int index)
		{
			var buffer = new List<byte>();
			var halfByte = false;
			var readingComment = false;
			var sol = true;
			byte tmp = 0;

			while (index < source.Length)
			{
				var c = source[index++];
				byte x;

				if (c == '\r' || c == '\n')
				{
					readingComment = false;
					sol = true;
					continue;
				}
				else if (c == '-' && sol)
				{
					var newLine = false;

					while (index < source.Length && source[index] == '-')
					{
						index++;
					}

					if (index < source.Length && source[index] == '\r')
					{
						newLine = true;
						index++;
					}

					if (index < source.Length && source[index] == '\n')
					{
						newLine = true;
						index++;
					}

					if (!newLine)
					{
						throw new ArgumentException("Invalid format.", nameof(source));
					}

					break;
				}

				sol = false;

				if (char.IsWhiteSpace(c) || readingComment)
				{
					continue;
				}
				else if (c >= '0' && c <= '9')
				{
					x = unchecked((byte)(c - '0'));
				}
				else if (c >= 'a' && c <= 'f')
				{
					x = unchecked((byte)(c - 'a' + 10));
				}
				else if (c >= 'A' && c <= 'F')
				{
					x = unchecked((byte)(c - 'A' + 10));
				}
				else if (c == '/' && index < source.Length && source[index] == '/')
				{
					readingComment = true;
					index++;
					continue;
				}
				else
				{
					throw new ArgumentException("Invalid format.", nameof(source));
				}

				if (halfByte)
				{
					buffer.Add(unchecked((byte)(tmp | x)));
					halfByte = false;
				}
				else
				{
					tmp = unchecked((byte)(x << 4));
					halfByte = true;
				}
			}

			if (halfByte)
			{
				buffer.Add(tmp);
			}

			return buffer.ToArray();
		}

		static string GetResource(string name)
		{
			using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
			using var reader = new StreamReader(stream);
			return reader.ReadToEnd();
		}
	}
}
