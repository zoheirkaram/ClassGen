using Common.Enums;
using Common.Classes;
using System.Collections.Generic;

namespace SimpleTokenizer
{
	public interface ITokenizer
	{
		List<Token> GetTokens(string code);
		List<Token> TokenizLine(string line, int lineNumber);
		char CurrentChar(string line, int location);
		char NextChar(string line, int location);
		void PrepareHtmlTokens(List<Token> tokens);

		List<(string, TokenType)> Keywords { get; set; }
		List<(string, TokenType)> Types { get; set; }
		List<(char, TokenType)> Brackets { get; set; }
		List<(char, TokenType)> Punctuator { get; set; }
		List<(char, TokenType)> Quotations { get; set; }
		List<(char, TokenType)> Nullable { get; set; }

	}
}
