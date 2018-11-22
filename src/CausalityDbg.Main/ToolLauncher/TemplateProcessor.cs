// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Text;

namespace CausalityDbg.Main
{
	static class TemplateProcessor
	{
		public static string ProcessTemplate(string template, Func<string, string> macroProcessor)
		{
			if (macroProcessor == null) throw new ArgumentNullException(nameof(macroProcessor));

			if (template == null)
			{
				return null;
			}

			const int ReadingText = 0;
			const int ReadingMacro = 1;

			StringBuilder builder = null;
			var state = ReadingText;
			var start = 0;

			for (var i = 0; i < template.Length; i++)
			{
				var c = template[i];

				switch (c)
				{
					case '{':
						{
							if (state != ReadingText) throw new FormatException();

							if (builder == null)
							{
								builder = new StringBuilder();
							}

							if (i > start)
							{
								builder.Append(template, start, i - start);
							}

							var n = i + 1;

							if (n < template.Length && template[n] == '{')
							{
								builder.Append('{');
								i = n;
							}
							else
							{
								state = ReadingMacro;
							}

							start = i + 1;
						}
						break;

					case '}':
						{
							if (builder == null)
							{
								builder = new StringBuilder();
							}

							if (state == ReadingMacro)
							{
								var label = template.Substring(start, i - start);
								builder.Append(macroProcessor(label));
								state = ReadingText;
							}
							else
							{
								if (i > start)
								{
									builder.Append(template, start, i - start);
								}

								var n = i + 1;

								if (n < template.Length && template[n] == '}')
								{
									builder.Append('}');
									i = n;
								}
								else
								{
									throw new FormatException();
								}
							}

							start = i + 1;
						}
						break;
				}
			}

			if (state != ReadingText)
			{
				throw new FormatException();
			}

			if (start == 0)
			{
				return template;
			}

			if (start < template.Length)
			{
				builder.Append(template, start, template.Length - start);
			}

			return builder.ToString();
		}

		public static IEnumerable<string> GetMacros(string template)
		{
			if (template == null) throw new ArgumentNullException(nameof(template));

			const int ReadingText = 0;
			const int ReadingMacro = 1;

			var state = ReadingText;
			var start = 0;

			for (var i = 0; i < template.Length; i++)
			{
				var c = template[i];

				switch (c)
				{
					case '{':
						{
							if (state != ReadingText) throw new FormatException();

							var n = i + 1;

							if (n < template.Length && template[n] == '{')
							{
								i = n;
							}
							else
							{
								state = ReadingMacro;
							}

							start = i + 1;
						}
						break;

					case '}':
						{
							if (state == ReadingMacro)
							{
								yield return template.Substring(start, i - start);
								state = ReadingText;
							}
							else
							{
								var n = i + 1;

								if (n < template.Length && template[n] == '}')
								{
									i = n;
								}
								else
								{
									throw new FormatException();
								}
							}

							start = i + 1;
						}
						break;
				}
			}

			if (state != ReadingText)
			{
				throw new FormatException();
			}
		}
	}
}
