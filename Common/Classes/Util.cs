using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
	}
}
