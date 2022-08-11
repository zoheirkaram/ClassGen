using Common.Enums;
using System;
using System.Collections.Generic;

namespace SimpleTokenizer
{
	public class TypeScriptTokenizer : BaseTokenizer, IDisposable
    {
        public override List<(string, TokenType)> Keywords
        {
            get
            {
                return new List<(string, TokenType)>
                {
                    ("public", TokenType.Keyword),
                    ("private", TokenType.Keyword),
                    ("protected", TokenType.Keyword),

                    ("export", TokenType.Keyword),
                    ("class", TokenType.Keyword),
                    ("this", TokenType.Keyword),
                    ("constructor", TokenType.Keyword),
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
                    ('{', TokenType.Bracket),
                    ('}', TokenType.Bracket),
                    ('<', TokenType.Bracket),
                    ('>', TokenType.Bracket)
                };
            }
            set { }
        }
        public override List<(char, TokenType)> Punctuator
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
                    ("boolean", TokenType.Type),
                    ("string", TokenType.Type),
                    ("number", TokenType.Type),
                    ("Date", TokenType.Type),
                };
            }
            set { }
        }
    }
}
