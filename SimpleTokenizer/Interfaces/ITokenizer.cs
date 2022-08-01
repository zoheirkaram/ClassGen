using System.Collections.Generic;
using Common.Enums;
using Common.Classes;

namespace SimpleTokenizer
{
    public  interface ITokenizer
    {
        List<Token> TokenizLine(string line, int lineNumber);
        string CurrentChar(string line, int location);
        string NextChar(string line, int location);

        List<(string, TokenType)> keywords { get; set; }
        List<(string, TokenType)> brackets { get; set; }
        List<(string, TokenType)> separators { get; set; }
        List<(string, TokenType)> qoutations { get; set; }
    }
}
