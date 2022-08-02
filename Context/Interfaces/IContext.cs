using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DBContext
{
	public interface IContext
	{
		void SetTableName(string tableName);
		Task<SqlConnection> GetConnectionAsync();
		Task<SqlCommand> GetCommandAsync(string commandString);
		Task<List<String>> GetTablesAsync();
		Task<List<T>> GetTableDataAsync<T>();
		string CommandString(string tableName);
	}
}
