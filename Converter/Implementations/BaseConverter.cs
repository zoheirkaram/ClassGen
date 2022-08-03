using Common.Classes;
using Common.Enums;
using SimpleTokenizer;

namespace Converter
{
	public class BaseConverter
	{
		private string _style = string.Empty;
		private Language _language;

		public BaseConverter(Language language)
		{
			this._language = language;
		}

		public string GetHighlightedHtmlCode(string code)
		{
			BaseTokenizer selectedTokenizer = new BaseTokenizer();

			switch(_language)
			{
				case Language.CSharp:
					selectedTokenizer = new CSharpTokenizer();
					break;

				case Language.TypeScript:
					selectedTokenizer = new TypeScriptTokenizer();
					break;
			}

			using (var tokenizer = selectedTokenizer)
			{
				this.SetStyle();

				var tokens = tokenizer.GetTokens(code);
				var html = tokenizer.Highlight(tokens);

				html = html.Insert(0, $"<style>{_style}</style><body><div class=\"highlight\"><pre>").Replace("-----", "<hr />");
				html = html.Insert(html.Length, "</pre></div></body>");

				return html;
			};
		}

		private void SetStyle()
		{
			_style = $@"
						body {{ background-color: #{HighlightColor.Background};}}
						.highlight {{ color: #333; font-size: 12px; line-height: 16px; }}
						.highlight .Identifier {{ color: #{HighlightColor.Identifier}; }}
						.highlight .Keyword {{ color: #{HighlightColor.Keyword}; }}
						.highlight .Comment {{ color: #{HighlightColor.Comment}; }}
						.highlight .QuotedString {{ color: #{HighlightColor.QuotedString}; }}
						.highlight .Constant {{ color: #{HighlightColor.Constant}; }}
						.highlight .Number {{ color: #{HighlightColor.Number}; }}
						.highlight .Bracket {{ color: #{HighlightColor.Identifier}; }}
					";
		}
	}
}
