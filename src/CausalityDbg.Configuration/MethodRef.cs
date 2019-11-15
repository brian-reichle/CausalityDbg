// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

namespace CausalityDbg.Configuration
{
	public sealed class MethodRef
	{
		public static MethodRef Parse(string text)
		{
			using var enumerator = GetScanner(text).GetEnumerator();
			enumerator.MoveNext();

			var name = ReadToken(enumerator, TokenType.Text);

			if (enumerator.Current.Type == TokenType.OpenParen)
			{
				ReadToken(enumerator, TokenType.OpenParen);

				var arguments = ImmutableArray.CreateBuilder<ArgumentType>();

				if (enumerator.Current.Type == TokenType.Text)
				{
					arguments.Add(ReadTypeRef(enumerator));

					while (enumerator.Current.Type == TokenType.Comma)
					{
						ReadToken(enumerator, TokenType.Comma);
						arguments.Add(ReadTypeRef(enumerator));
					}
				}

				ReadToken(enumerator, TokenType.CloseParen);
				ReadToken(enumerator, TokenType.EOF);
				return new MethodRef(text, name, true, arguments.ToImmutable());
			}
			else
			{
				ReadToken(enumerator, TokenType.EOF);
				return new MethodRef(text, name, false, ImmutableArray<ArgumentType>.Empty);
			}
		}

		static ArgumentType ReadTypeRef(IEnumerator<Token> enumerator)
		{
			var name = ReadToken(enumerator, TokenType.Text);
			var byRef = false;

			if (enumerator.Current.Type == TokenType.ByRef)
			{
				ReadToken(enumerator, TokenType.ByRef);
				byRef = true;
			}

			return new ArgumentType(name, byRef);
		}

		static string ReadToken(IEnumerator<Token> enumerator, TokenType type)
		{
			if (enumerator.Current.Type != type)
			{
				throw new ArgumentException("Expected token of type " + type + " but got " + enumerator.Current.Type);
			}

			var text = enumerator.Current.Text;
			enumerator.MoveNext();
			return text;
		}

		static IEnumerable<Token> GetScanner(string text)
		{
			var i = 0;

			while (i < text.Length)
			{
				var c = text[i];

				if (c == '(')
				{
					yield return new Token(TokenType.OpenParen);
				}
				else if (c == ')')
				{
					yield return new Token(TokenType.CloseParen);
				}
				else if (c == ',')
				{
					yield return new Token(TokenType.Comma);
				}
				else if (c == '&')
				{
					yield return new Token(TokenType.ByRef);
				}
				else if (IsIdentifierToken(c))
				{
					var start = i;

					while (i < text.Length)
					{
						if (IsIdentifierToken(text[i]))
						{
							i++;
						}
						else if (text[i] == ':' && i + 3 < text.Length && text[i + 1] == ':' && IsIdentifierToken(text[i + 2]))
						{
							i += 3;
						}
						else
						{
							break;
						}
					}

					yield return new Token(text.Substring(start, i - start));

					continue;
				}
				else if (!char.IsWhiteSpace(c))
				{
					throw new ArgumentException("Invalid assembly reference format");
				}

				i++;
			}

			yield return new Token(TokenType.EOF);
		}

		static bool IsIdentifierToken(char c)
		{
			return char.IsLetterOrDigit(c)
				|| Array.BinarySearch(_idCompatableSymbols, c) >= 0;
		}

		MethodRef(
			string text,
			string name,
			bool specifiesArgTypes,
			ImmutableArray<ArgumentType> argTypes)
		{
			Text = text;
			Name = name;
			SpecifiesArgTypes = specifiesArgTypes;
			ArgTypes = argTypes;
		}

		public string Text { get; }
		public string Name { get; }
		public bool SpecifiesArgTypes { get; }
		public ImmutableArray<ArgumentType> ArgTypes { get; }

		/// <summary>
		/// MUST remain sort.
		/// </summary>
		static readonly char[] _idCompatableSymbols = new[]
		{
			'$', '-', '.', '<', '>', '_',
		};

		enum TokenType
		{
			EOF,
			Text,
			OpenParen,
			CloseParen,
			Comma,
			ByRef,
		}

		[DebuggerDisplay("{Type}: {Text}")]
		readonly struct Token
		{
			public Token(TokenType type)
			{
				Type = type;
				Text = null;
			}

			public Token(string text)
			{
				Type = TokenType.Text;
				Text = text;
			}

			public TokenType Type { get; }
			public string Text { get; }
		}
	}
}
