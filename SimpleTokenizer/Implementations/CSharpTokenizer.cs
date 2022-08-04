using System;
using System.Collections.Generic;
using Common.Enums;

namespace SimpleTokenizer
{
	public class CSharpTokenizer : BaseTokenizer, IDisposable
    {
        public override List<(string, TokenType)> Keywords
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
                    ("null", TokenType.Keyword)
                };
            }
            set { }
        }
        public override List<(char, TokenType)> Brackets
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
        public override List<(char, TokenType)> Separators
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
        public override List<(char, TokenType)> Quotations
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
        public override List<(char, TokenType)> Nullable
        {
            get
            {
                return new List<(char, TokenType)>
                {
                    ('?', TokenType.Nullable)
                };
            }
            set { }
        }

        public override List<(string, TokenType)> Types
		{
			get
			{
                return new List<(string, TokenType)>
                {
                    ("bool", TokenType.Type),
                    ("byte", TokenType.Type),
                    ("char", TokenType.Type),
                    ("decimal", TokenType.Type),
                    ("double", TokenType.Type),
                    ("extern", TokenType.Type),
                    ("float", TokenType.Type),
                    ("int", TokenType.Type),
                    ("long", TokenType.Type),
                    ("object", TokenType.Type),
                    ("readonly", TokenType.Type),
                    ("sbyte", TokenType.Type),
                    ("short", TokenType.Type),
                    ("string", TokenType.Type),
                    ("TimeSpan", TokenType.Type),
                    ("DateTime", TokenType.Type),
                    ("DateTimeOffset", TokenType.Type),
                    ("Guid", TokenType.Type),
                };
			}
            set { }
		}
    }
}
