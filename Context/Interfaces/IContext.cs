using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Context
{
	public interface IContext
	{
		void SetTableName(string tableName);
		Task<SqlConnection> GetConnection();
		Task<SqlCommand> GetCommand(string commandString);
		Task<List<String>> GetTables();
		Task<List<T>> GetTableData<T>();
		string CommandString(string tableName);
	}
}
