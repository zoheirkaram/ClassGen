using Common.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassConverter
{
    public interface IConverter
    {
        List<TableSchemaResult> TableSchama { get; set; }
        string GetClass();
        string GetHighlightedClass();
    }
}
