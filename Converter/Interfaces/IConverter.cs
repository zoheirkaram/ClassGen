using Common.Classes;
using DBContext;
using System.Threading.Tasks;

namespace ClassConverter
{
    public interface IConverter
    {
        Task<string> GetClass(IContext context);
        string GetTableDefinitoinCommandString();
    }
}
