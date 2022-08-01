using Common.Enums;

namespace Common.Classes
{
    public class Token
    {
        public TokenType Type { get; set; }
        public int LineNumber { get; set; }
        public int PositionStart { get; set; }
        public int SymbolLength { get; set; }
        public string Symbol { get; set; }
    }
}
