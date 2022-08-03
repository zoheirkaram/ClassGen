using System;
using System.Collections.Generic;
using Common.Enums;

namespace SimpleTokenizer
{
	public class CSharpTokenizer : TokenizerBase, IDisposable
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
                    ('?', TokenType.Quotation)
                };
            }
            set { }
        }

		public void Dispose()
		{
			this.Dispose(true);
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
