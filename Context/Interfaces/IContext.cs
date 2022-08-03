using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DBContext
{
	public interface IContext
	{
		string CommandString { get; set; }
		void SetConnectionString(string connectionString);
		Task<SqlConnection> GetConnectionAsync();
		Task<SqlCommand> GetCommandAsync(string commandString);
		Task<List<String>> GetTablesAsync();
		Task<List<T>> GetTableDataAsync<T>();
	}
}
