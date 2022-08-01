﻿using Common.Classes;
using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTokenizer
{
    public class CSharpTokenize : ITokenizer
    {
        public List<(string, TokenType)> Keywords {
            get
            {
                return new List<(string, TokenType)>
                {
                    ("using", TokenType.keyword),
                    ("abstract", TokenType.keyword),
                    ("bool", TokenType.keyword),
                    ("byte", TokenType.keyword),
                    ("char", TokenType.keyword),
                    ("class", TokenType.keyword),
                    ("const", TokenType.keyword),
                    ("decimal", TokenType.keyword),
                    ("double", TokenType.keyword),
                    ("extern", TokenType.keyword),
                    ("float", TokenType.keyword),
                    ("int", TokenType.keyword),
                    ("internal", TokenType.keyword),
                    ("long", TokenType.keyword),
                    ("namespace", TokenType.keyword),
                    ("null", TokenType.keyword),
                    ("object", TokenType.keyword),
                    ("private", TokenType.keyword),
                    ("protected", TokenType.keyword),
                    ("public", TokenType.keyword),
                    ("readonly", TokenType.keyword),
                    ("sbyte", TokenType.keyword),
                    ("short", TokenType.keyword),
                    ("string", TokenType.keyword),
                    ("DateTime", TokenType.keyword),
                    ("get", TokenType.keyword),
                    ("set", TokenType.keyword)
                };
            }
            set { }
        }
        public List<(string, TokenType)> Brackets
        {
            get
            {
                return new List<(string, TokenType)>
                {
                    ("(", TokenType.bracket),
                    (")", TokenType.bracket),
                    ("[", TokenType.bracket),
                    ("]", TokenType.bracket),
                    ("{", TokenType.bracket),
                    ("}", TokenType.bracket)
                };
            }
            set { }
        }
        public List<(string, TokenType)> Separators
        {
            get
            {
                return new List<(string, TokenType)>
                {
                    (".", TokenType.separator),
                    (";", TokenType.separator),
                    (".", TokenType.separator)
                };
            }
            set { }
        }
        public List<(string, TokenType)> Qoutations
        {
            get
            {
                return new List<(string, TokenType)>
                {
                    ("\"", TokenType.quotation),
                    ("'", TokenType.quotation)
                };
            }
            set { }
        }

        public string CurrentChar(string line, int location)
        {
            return line[location].ToString();
        }

		public string NextChar(string line, int location)
        {
            return location + 1 < line.Length ? line[location + 1].ToString() : "";
        }

        public List<Token> GetTokens(string code)
        {
            var codeTokens = new List<Token>();
            var codeLines = code.Split('\n');

            var lineNumber = 1;

            while (lineNumber <= codeLines.Length)
            {
                var lineTokens = TokenizLine(codeLines[lineNumber - 1].Trim(), lineNumber);

                codeTokens.AddRange(lineTokens);

                ++lineNumber;
            }

            PrepareHtmlTokens(codeTokens);

            return codeTokens;
        }

        public List<Token> TokenizLine(string line, int lineNumber)
        {
            var tokens = new List<Token>();

            var i = 0;
            var symbolStart = 0;
            var symbol = string.Empty;
            var letteral = string.Empty;

            while (i < line.Length)
            {
                if (string.IsNullOrWhiteSpace(CurrentChar(line, i)))
                {
                    ++i;
                    continue;
                }

                if (Brackets.Any(b => b.Item1 == CurrentChar(line, i)))
                {
                    tokens.Add(new Token { Type = TokenType.bracket, LineNumber = lineNumber, PositionStart = i - 1, SymbolLength = 1, Symbol = CurrentChar(line, i) });

                    ++i;
                    continue;
                }

                if (Separators.Any(b => b.Item1 == CurrentChar(line, i)))
                {
                    tokens.Add(new Token { Type = TokenType.separator, LineNumber = lineNumber, PositionStart = i - 1, SymbolLength = 1, Symbol = CurrentChar(line, i) });

                    ++i;
                    continue;
                }

                if (Qoutations.Any(b => b.Item1 == CurrentChar(line, i)))
                {
                    letteral = CurrentChar(line, i);

                    while (NextChar(line, i) != "\"")
                    {
                        letteral += NextChar(line, i);

                        ++i;
                    }

                    letteral += CurrentChar(line, ++i);

                    tokens.Add(new Token { Type = TokenType.quotedString, LineNumber = lineNumber, PositionStart = i, SymbolLength = 1, Symbol = letteral });

                    ++i;
                    continue;
                }

                symbol += CurrentChar(line, i);

                if (
                        Qoutations.Any(b => b.Item1 == NextChar(line, i))
                        ||
                        Separators.Any(b => b.Item1 == NextChar(line, i))
                        ||
                        Brackets.Any(b => b.Item1 == NextChar(line, i))
                        ||
                        NextChar(line, i) == " "
                    )

                {
                    if (Keywords.Any(k => k.Item1 == symbol))
                    {
                        tokens.Add(new Token { Type = TokenType.keyword, LineNumber = lineNumber, PositionStart = symbolStart - 1, SymbolLength = symbol.Length - 1, Symbol = symbol });

                        symbolStart = i;
                        symbol = string.Empty;
                    }
                    else
                    {
                        tokens.Add(new Token { Type = TokenType.identifier, LineNumber = lineNumber, PositionStart = symbolStart - 1, SymbolLength = symbol.Length - 1, Symbol = symbol });

                        symbolStart = i;
                        symbol = string.Empty;
                    }
                }

                ++i;
            }

            return tokens;
        }

		public void PrepareHtmlTokens(List<Token> tokens)
		{
            tokens
            .ForEach(t =>
            {
                if (t.Type == TokenType.keyword || t.Type == TokenType.identifier || t.Type == TokenType.quotedString)
                {
                    t.HtmlSymbol = $"<span class=\"{t.Type}\">{t.Symbol}</span>";
                }
                if (t.Type == TokenType.separator || t.Type == TokenType.quotation || t.Type == TokenType.bracket)
                {
                    t.HtmlSymbol = $"{t.Symbol}";
                }
            });
        }
	}
}
