using System.Runtime.InteropServices;

namespace Common.Classes
{
    public static class Util
    {
		public static bool IsNumeric(string number)
		{
			var result = int.TryParse(number, out _);
			result = result || float.TryParse(number, out _);
			result = result || decimal.TryParse(number, out _);

			return result;
		}

		public static string DefaultScreen()
{
			return $"<style>body{{background-color: {HighlightColor.Background};}}</style>" +
				   $"<body><div class=\"highlight\"><pre>" +
				   $"</pre></div></body>";
		}
	}
}
