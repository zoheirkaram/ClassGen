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
                    ("public", TokenType.keyword),
                    ("private", TokenType.keyword),
                    ("internal", TokenType.keyword),
                    ("protected", TokenType.keyword),
                    ("abstract", TokenType.keyword),
                    ("virtual", TokenType.keyword),

                    ("using", TokenType.keyword),
                    ("namespace", TokenType.keyword),
                    ("class", TokenType.keyword),
                    ("get", TokenType.keyword),
                    ("set", TokenType.keyword),
                    ("const", TokenType.keyword),
                    ("null", TokenType.keyword),

                    ("bool", TokenType.keyword),
                    ("byte", TokenType.keyword),
                    ("char", TokenType.keyword),
                    ("decimal", TokenType.keyword),
                    ("double", TokenType.keyword),
                    ("extern", TokenType.keyword),
                    ("float", TokenType.keyword),
                    ("int", TokenType.keyword),
                    ("long", TokenType.keyword),
                    ("object", TokenType.keyword),
                    ("readonly", TokenType.keyword),
                    ("sbyte", TokenType.keyword),
                    ("short", TokenType.keyword),
                    ("string", TokenType.keyword),
                    ("TimeSpan", TokenType.keyword),
                    ("DateTime", TokenType.keyword),
                    ("DateTimeOffset", TokenType.keyword),
                    ("Guid", TokenType.keyword),

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
                    ("}", TokenType.bracket),
                    ("<", TokenType.bracket),
                    (">", TokenType.bracket)
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
                    (".", TokenType.separator),
                    (",", TokenType.separator),
                    (":", TokenType.separator)
                };
            }
            set { }
        }
        public List<(string, TokenType)> Quotations
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
        public List<(string, TokenType)> Nullable
        {
            get
            {
                return new List<(string, TokenType)>
                {
                    ("?", TokenType.quotation)
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
            var quotedString = string.Empty;

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

                if (Nullable.Any(b => b.Item1 == CurrentChar(line, i)))
                {
                    tokens.Add(new Token { Type = TokenType.nullable, LineNumber = lineNumber, PositionStart = i, SymbolLength = 1, Symbol = CurrentChar(line, i) });

                    ++i;
                    continue;
                }

                if (Quotations.Any(b => b.Item1 == CurrentChar(line, i)))
                {
                    quotedString = CurrentChar(line, i);

                    while (NextChar(line, i) != "\"")
                    {
                        quotedString += NextChar(line, i);

                        ++i;
                    }

                    quotedString += CurrentChar(line, ++i);

                    tokens.Add(new Token { Type = TokenType.quotedString, LineNumber = lineNumber, PositionStart = i, SymbolLength = quotedString.Length, Symbol = quotedString });

                    ++i;
                    continue;
                }

                symbol += CurrentChar(line, i);

                if (
                        Quotations.Any(b => b.Item1 == NextChar(line, i))
                        ||
                        Separators.Any(b => b.Item1 == NextChar(line, i))
                        ||
                        Brackets.Any(b => b.Item1 == NextChar(line, i))
                        ||
                        string.IsNullOrWhiteSpace(NextChar(line, i))
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
                if (t.Type == TokenType.separator || t.Type == TokenType.quotation || t.Type == TokenType.bracket || t.Type == TokenType.nullable)
                {
                    t.HtmlSymbol = $"{t.Symbol}";
                }
            });
        }
	}
}
