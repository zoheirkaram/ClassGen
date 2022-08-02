using Common.Classes;
using Common.Enums;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using System.Runtime.InteropServices;

namespace SimpleTokenizer
{
	public class CSharpTokenizer : ITokenizer, IDisposable
    {
        public List<(string, TokenType)> Keywords
        {
            get
            {
                return new List<(string, TokenType)>
                {
                    ("public", TokenType.Keyword),
                    ("private", TokenType.Keyword),
                    ("internal", TokenType.Keyword),
                    ("protected", TokenType.Keyword),
                    ("abstract", TokenType.Keyword),
                    ("virtual", TokenType.Keyword),

                    ("using", TokenType.Keyword),
                    ("namespace", TokenType.Keyword),
                    ("class", TokenType.Keyword),
                    ("get", TokenType.Keyword),
                    ("set", TokenType.Keyword),
                    ("const", TokenType.Keyword),
                    ("null", TokenType.Keyword),

                    ("bool", TokenType.Keyword),
                    ("byte", TokenType.Keyword),
                    ("char", TokenType.Keyword),
                    ("decimal", TokenType.Keyword),
                    ("double", TokenType.Keyword),
                    ("extern", TokenType.Keyword),
                    ("float", TokenType.Keyword),
                    ("int", TokenType.Keyword),
                    ("long", TokenType.Keyword),
                    ("object", TokenType.Keyword),
                    ("readonly", TokenType.Keyword),
                    ("sbyte", TokenType.Keyword),
                    ("short", TokenType.Keyword),
                    ("string", TokenType.Keyword),
                    ("TimeSpan", TokenType.Keyword),
                    ("DateTime", TokenType.Keyword),
                    ("DateTimeOffset", TokenType.Keyword),
                    ("Guid", TokenType.Keyword),

                };
            }
            set { }
        }
        public List<(char, TokenType)> Brackets
        {
            get
            {
                return new List<(char, TokenType)>
                {
                    ('(', TokenType.Bracket),
                    (')', TokenType.Bracket),
                    ('[', TokenType.Bracket),
                    (']', TokenType.Bracket),
                    ('{', TokenType.Bracket),
                    ('}', TokenType.Bracket),
                    ('<', TokenType.Bracket),
                    ('>', TokenType.Bracket)
                };
            }
            set { }
        }
        public List<(char, TokenType)> Separators
        {
            get
            {
                return new List<(char, TokenType)>
                {
                    ('.', TokenType.Separator),
                    (';', TokenType.Separator),
                    ('.', TokenType.Separator),
                    (',', TokenType.Separator),
                    (':', TokenType.Separator)
                };
            }
            set { }
        }
        public List<(char, TokenType)> Quotations
        {
            get
            {
                return new List<(char, TokenType)>
                {
                    ('"', TokenType.Quotation),
                    ('\'', TokenType.Quotation)
                };
            }
            set { }
        }
        public List<(char, TokenType)> Nullable
        {
            get
            {
                return new List<(char, TokenType)>
                {
                    ('?', TokenType.Quotation)
                };
            }
            set { }
        }

        public string Highlight(List<Token> CodeTokens)
        {
            var lineNumber = CodeTokens?.First()?.LineNumber;
            var stringBuilder = new StringBuilder();

            foreach (var token in CodeTokens)
            {
                if (lineNumber != token.LineNumber)
                {
                    lineNumber = token.LineNumber;
                    stringBuilder.Append(Environment.NewLine);
                }

                if (token.Type == TokenType.Space)
                {
                    stringBuilder.Append(" ");
                }

                if (token.Type == TokenType.Tab)
				{
                    stringBuilder.Append('\t');
				}

                stringBuilder.Append(token.HtmlSymbol);

            }

            return stringBuilder.ToString();
        }

        public char CurrentChar(string line, int location)
        {
            return line[location];
        }

        public char NextChar(string line, int location)
        {
            return location + 1 < line.Length ? line[location + 1] : '\0';
        }

        public List<Token> GetTokens(string code)
        {
            var codeTokens = new List<Token>();
            var codeLines = code.Split('\n');

            var lineNumber = 1;

            while (lineNumber <= codeLines.Length)
            {
                var lineTokens = this.TokenizLine(codeLines[lineNumber - 1], lineNumber);

                codeTokens.AddRange(lineTokens);

                ++lineNumber;
            }

            this.PrepareHtmlTokens(codeTokens);

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
                if (string.IsNullOrWhiteSpace(this.CurrentChar(line, i).ToString()))
                {
                    TokenType tokenType;

                    switch (this.CurrentChar(line, i))
					{
                        case '\t':
                            tokenType = TokenType.Tab;
                            break;

                        case '\r':
                        case '\n':
                            tokenType = TokenType.NewLine;
                            break;

                        case ' ':
                            tokenType = TokenType.Space;
                            break;

                        default:
                            tokenType = TokenType.None;
                            break;
					}

                    tokens.Add(item: new Token { Type = tokenType, LineNumber = lineNumber, PositionStart = i, SymbolLength = 1, Symbol = this.CurrentChar(line, i).ToString() });

                    ++i;
                    continue;
                }

                if (this.Brackets.Any(b => b.Item1 == this.CurrentChar(line, i)))
                {
                    tokens.Add(item: new Token { Type = TokenType.Bracket, LineNumber = lineNumber, PositionStart = i, SymbolLength = 1, Symbol = this.CurrentChar(line, i).ToString() });

                    ++i;
                    continue;
                }

                if (this.Separators.Any(b => b.Item1 == this.CurrentChar(line, i)))
                {
                    tokens.Add(new Token { Type = TokenType.Separator, LineNumber = lineNumber, PositionStart = i, SymbolLength = 1, Symbol = this.CurrentChar(line, i).ToString() });

                    ++i;
                    continue;
                }

                if (this.Nullable.Any(b => b.Item1 == this.CurrentChar(line, i)))
                {
                    tokens.Add(new Token { Type = TokenType.Nullable, LineNumber = lineNumber, PositionStart = i, SymbolLength = 1, Symbol = this.CurrentChar(line, i).ToString() });

                    ++i;
                    continue;
                }

                if (this.Quotations.Any(b => b.Item1 == this.CurrentChar(line, i)))
                {
                    quotedString = this.CurrentChar(line, i).ToString();

                    while (this.NextChar(line, i).ToString() != "\"")
                    {
                        quotedString += this.NextChar(line, i);

                        ++i;
                    }

                    quotedString += this.CurrentChar(line, ++i);

                    tokens.Add(new Token { Type = TokenType.QuotedString, LineNumber = lineNumber, PositionStart = i, SymbolLength = quotedString.Length, Symbol = quotedString });

                    ++i;
                    continue;
                }

                symbol += this.CurrentChar(line, i);

                if (
                        this.Quotations.Any(b => b.Item1 == this.NextChar(line, i))
                        ||
                        this.Separators.Any(b => b.Item1 == this.NextChar(line, i))
                        ||
                        this.Brackets.Any(b => b.Item1 == this.NextChar(line, i))
                        ||
                        this.Nullable.Any(b => b.Item1 == this.NextChar(line, i))
                        ||
                        string.IsNullOrWhiteSpace(this.NextChar(line, i).ToString())
                    )

                {
                    if (this.Keywords.Any(k => k.Item1 == symbol))
                    {
                        tokens.Add(new Token { Type = TokenType.Keyword, LineNumber = lineNumber, PositionStart = symbolStart, SymbolLength = symbol.Length, Symbol = symbol });

                        symbolStart = i + 1;
                        symbol = string.Empty;
                    }
                    else
                    {
                        tokens.Add(new Token { Type = Util.IsNumeric(symbol) ? TokenType.Number : TokenType.Identifier, LineNumber = lineNumber, PositionStart = symbolStart, SymbolLength = symbol.Length, Symbol = symbol });

                        symbolStart = i + 1;
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
                if (t.Type == TokenType.Keyword || t.Type == TokenType.Identifier || t.Type == TokenType.QuotedString || t.Type == TokenType.Number)
                {
                    t.HtmlSymbol = $"<span class=\"{t.Type}\">{t.Symbol}</span>";
                }
                if (t.Type == TokenType.Separator || t.Type == TokenType.Quotation || t.Type == TokenType.Bracket || t.Type == TokenType.Nullable)
                {
                    t.HtmlSymbol = $"<span class=\"Identifier\">{t.Symbol}</span>";
                }
            });
        }

		public void Dispose()
		{
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Keywords = null;
                this.Brackets = null;
                this.Separators = null;
                this.Quotations = null;
                this.Nullable = null;
            }
        }
    }
}
