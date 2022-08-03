using DBContext;
using System.Threading.Tasks;

namespace Converter
{
    public interface IConverter
    {
        Task<string> GetClass(IContext context);
        string GetTableDefinitoinCommandString();
    }
}
