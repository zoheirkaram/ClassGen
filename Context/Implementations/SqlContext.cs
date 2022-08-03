using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

namespace DBContext
{
    public class SqlContext : IContext
    {
		public string CommandString { get; set; }

		private string _connectionString = string.Empty;

		public SqlContext() { }

		public void SetConnectionString(string connectionString)
		{
			this._connectionString = connectionString;
		}

		public async Task<SqlConnection> GetConnectionAsync()
		{
			var connection = new SqlConnection(this._connectionString);

			if (connection.State == ConnectionState.Closed)
			{
				await connection.OpenAsync();
			}

			return connection;
		}

		public async Task<SqlCommand> GetCommandAsync(string commandString)
		{
			var connection = await GetConnectionAsync();
			var command = new SqlCommand(commandString, connection);

			command.CommandType = CommandType.Text;

			return command;
		}

		public async Task<List<String>> GetTablesAsync()
		{
			var sqlCommandString = "SELECT t.name FROM sys.tables t ORDER BY t.name";
			var command = await this.GetCommandAsync(sqlCommandString);
			var reader = command.ExecuteReader();
			var tables = new List<string>();

			while (await reader.ReadAsync())
			{
				tables.Add(reader["name"].ToString());
			}

			return tables;
		}

		public async Task<List<T>> GetTableDataAsync<T>()
		{
			string commandString = this.CommandString;
			var command = await this.GetCommandAsync(commandString);
			var reader = await command.ExecuteReaderAsync();
			var tableResult = new List<T>();

			while (await reader.ReadAsync())
			{
				var obj = Activator.CreateInstance<T>();

				foreach (var prop in obj.GetType().GetProperties())
				{
					try
					{
						prop.SetValue(obj, reader[prop.Name] == DBNull.Value ? null : reader[prop.Name]);
					}
					catch(Exception)
					{
						prop.SetValue(obj, null);
					}
				}

				tableResult.Add(obj);
			}

			return tableResult;
		}
	}
}
