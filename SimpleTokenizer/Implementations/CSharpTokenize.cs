using Common.Classes;
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
        public List<(string, TokenType)> keywords { 
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
        public List<(string, TokenType)> brackets
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
        public List<(string, TokenType)> separators 
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
        public List<(string, TokenType)> qoutations 
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

        public List<Token> TokenizLine(string line, int lineNumber)
        {
            var tokens = new List<Token>();
            var i = 0;
            var symbolStart = 0;
            var symbol = string.Empty;

            while (i < line.Length)
            {
                if (string.IsNullOrWhiteSpace(CurrentChar(line, i)))
                {
                    ++i;
                    continue;
                }

                if (brackets.Count(b => b.Item1 == CurrentChar(line, i)) > 0)
                {
                    tokens.Add(new Token { Type = TokenType.bracket, LineNumber = lineNumber, PositionStart = i - 1, SymbolLength = 1, Symbol = CurrentChar(line, i) });
                    ++i;
                    continue;
                }

                if (separators.Count(b => b.Item1 == CurrentChar(line, i)) > 0)
                {
                    tokens.Add(new Token { Type = TokenType.separator, LineNumber = lineNumber, PositionStart = i - 1, SymbolLength = 1, Symbol = CurrentChar(line, i) });
                    ++i;
                    continue;
                }

                if (qoutations.Count(b => b.Item1 == CurrentChar(line, i)) > 0)
                {
                    tokens.Add(new Token { Type = TokenType.quotation, LineNumber = lineNumber, PositionStart = i - 1, SymbolLength = 1, Symbol = CurrentChar(line, i) });
                    ++i;
                    continue;
                }

                symbol += CurrentChar(line, i);

                if (
                        qoutations.Count(b => b.Item1 == NextChar(line, i)) > 0
                        ||
                        separators.Count(b => b.Item1 == NextChar(line, i)) > 0
                        ||
                        brackets.Count(b => b.Item1 == NextChar(line, i)) > 0
                        ||
                        NextChar(line, i) == " "
                    )

                {
                    if (keywords.Count(k => k.Item1 == symbol) > 0)
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

            tokens
            .ForEach(t =>
            {
                if (t.Type == TokenType.keyword)
                {
                    t.HtmlSymbol = $"<span class=\"{t.Type}\">{t.Symbol}</span>";
                }
                if (t.Type == TokenType.identifier)
                {
                    t.HtmlSymbol = $"<span class=\"{t.Type}\">{t.Symbol}</span>";
                }
                if (t.Type == TokenType.separator || t.Type == TokenType.quotation || t.Type == TokenType.bracket)
                {
                    t.HtmlSymbol = $"{t.Symbol}";
                }
            });

            return tokens;
        }
    }
}
