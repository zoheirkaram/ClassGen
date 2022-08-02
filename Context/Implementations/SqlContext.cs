using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

namespace DBContext
{
    public class SqlContext : IContext
    {
		private string _connectionString = string.Empty;
		private string _tableName = string.Empty;

		public SqlContext(string connectionString)
		{
			this._connectionString = connectionString;
		}

		public void SetTableName(string tableName)
		{
			this._tableName = tableName;
		}

		public async Task<SqlConnection> GetConnection()
		{
			var connection = new SqlConnection(this._connectionString);

			if (connection.State == ConnectionState.Closed)
			{
				await connection.OpenAsync();
			}

			return connection;
		}

		public async Task<SqlCommand> GetCommand(string commandString)
		{
			var connection = await GetConnection();
			var command = new SqlCommand(commandString, connection);

			command.CommandType = CommandType.Text;

			return command;
		}

		public async Task<List<String>> GetTables()
		{
			var sqlCommandString = "SELECT t.name FROM sys.tables t ORDER BY t.name";
			var command = await this.GetCommand(sqlCommandString);
			var reader = command.ExecuteReader();
			var tables = new List<string>();

			while (await reader.ReadAsync())
			{
				tables.Add(reader["name"].ToString());
			}

			return tables;
		}

		public async Task<List<T>> GetTableData<T>()
		{
			var commandString = this.CommandString(_tableName);
			var command = await this.GetCommand(commandString);
			var reader = await command.ExecuteReaderAsync();
			var tableResult = new List<T>();

			while (await reader.ReadAsync())
			{
				var obj = Activator.CreateInstance<T>();

				foreach (var prop in obj.GetType().GetProperties())
				{
					prop.SetValue(obj, reader[prop.Name] == DBNull.Value ? null : reader[prop.Name]);
				}

				tableResult.Add(obj);
			}

			return tableResult;
		}

		public string CommandString(string tableName)
		{
			var command = $@"DECLARE @tableName varchar(50) = '{tableName}';

							SELECT c.name AS ColumnName
								 , stp.name AS TypeName
								 , CASE
									   WHEN stp.name IN
									   (   'nvarchar'
										 , 'nchar'
									   ) THEN c.max_length / 2
									   ELSE c.max_length
								   END AS MaxLength
								 , i.is_primary_key AS IsPrimaryKey
								 , Cast(CASE
											WHEN fkc.parent_column_id IS NULL THEN 0
											ELSE 1
										END AS bit) AS HasReference
								 , c.is_nullable AS IsNullable
								 , t2.name AS ReferencedTableName
								 , Cast(CASE
											WHEN t2.name IS NOT NULL THEN Row_Number() OVER (PARTITION BY t2.name ORDER BY c.column_id)
											ELSE NULL
										END AS int) AS ReferencedTableNumber
								 , CASE stp.name
									   WHEN 'bigint' THEN 'long'
									   WHEN 'binary' THEN 'byte[]'
									   WHEN 'bit' THEN 'bool'
									   WHEN 'char' THEN 'string'
									   WHEN 'date' THEN 'DateTime'
									   WHEN 'datetime' THEN 'DateTime'
									   WHEN 'datetime2' THEN 'DateTime'
									   WHEN 'datetimeoffset' THEN 'DateTimeOffset'
									   WHEN 'decimal' THEN 'decimal'
									   WHEN 'float' THEN 'double'
									   WHEN 'image' THEN 'byte[]'
									   WHEN 'int' THEN 'int'
									   WHEN 'money' THEN 'decimal'
									   WHEN 'nchar' THEN 'string'
									   WHEN 'ntext' THEN 'string'
									   WHEN 'numeric' THEN 'decimal'
									   WHEN 'nvarchar' THEN 'string'
									   WHEN 'real' THEN 'float'
									   WHEN 'smalldatetime' THEN 'DateTime'
									   WHEN 'smallint' THEN 'short'
									   WHEN 'smallmoney' THEN 'decimal'
									   WHEN 'text' THEN 'string'
									   WHEN 'time' THEN 'TimeSpan'
									   WHEN 'timestamp' THEN 'long'
									   WHEN 'tinyint' THEN 'byte'
									   WHEN 'uniqueidentifier' THEN 'Guid'
									   WHEN 'varbinary' THEN 'byte[]'
									   WHEN 'varchar' THEN 'string'
									   ELSE 'UNKNOWN_' + stp.name
								   END AS cSharpType
							FROM sys.columns c
								 INNER JOIN sys.tables t ON t.object_id = c.object_id
								 INNER JOIN sys.types stp ON stp.system_type_id = c.system_type_id
								 LEFT JOIN sys.foreign_key_columns fkc ON fkc.parent_column_id = c.column_id
																		   AND fkc.parent_object_id = t.object_id
								 LEFT JOIN sys.tables t2 ON t2.object_id = fkc.referenced_object_id
								 LEFT JOIN sys.indexes i ON i.object_id = c.object_id
															 AND i.index_id = c.column_id
															 AND i.is_primary_key = 1
							WHERE t.name = @tableName
								AND stp.system_type_id = stp.user_type_id
							ORDER BY c.column_id;";

			return command;
		}
	}
}
