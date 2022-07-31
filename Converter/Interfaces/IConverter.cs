using Common.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter
{
    public interface IConverter
    {
        List<TableSchemaResult> TableSchama { get; set; }
        string GetCSharpClass();
        string GetHighlightedCSharpClass();
    }
}
