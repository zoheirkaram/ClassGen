using System.Collections.Generic;
using Common.Enums;
using Common.Classes;

namespace SimpleTokenizer
{
    public  interface ITokenizer
    {
        List<Token> GetTokens(string code);
        List<Token> TokenizLine(string line, int lineNumber);
        string CurrentChar(string line, int location);
        string NextChar(string line, int location);
        void PrepareHtmlTokens(List<Token> tokens);

        List<(string, TokenType)> Keywords { get; set; }
        List<(string, TokenType)> Brackets { get; set; }
        List<(string, TokenType)> Separators { get; set; }
        List<(string, TokenType)> Quotations { get; set; }
        List<(string, TokenType)> Nullable { get; set; }

    }
}
