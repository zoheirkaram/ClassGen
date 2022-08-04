using DBContext;
using Common.Classes;
using Common.Enums;
using Pluralize.NET.Core;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Converter
{
	public class TypeScriptConverter : BaseConverter, IConverter, IDisposable
	{
		private ConvertOptions _classOptions;

		public TypeScriptConverter(string tableName) : base(Language.TypeScript)
		{
			this._classOptions = new ConvertOptions() { TableName = tableName };
		}

		public TypeScriptConverter(ConvertOptions classOptions) : base(Language.TypeScript)
		{
			this._classOptions = classOptions ?? new ConvertOptions();
		}

		public override async Task<string> GetClass(IContext context)
		{
			var stringBuilder = new StringBuilder();

			context.CommandString = this.GetTableDefinitoinCommandString();
			var tableSchama = await context.GetTableDataAsync<TableSchemaResult>();

			stringBuilder.AppendLine($"export class {new Pluralizer().Singularize(this._classOptions.TableName)}");
			stringBuilder.AppendLine("{");

			var constructor = new StringBuilder().Append("constructor (");
			var constructorList = new List<string>();

			tableSchama.ToList()
			.ForEach(td =>
			{
				stringBuilder.AppendLine($"\t{this._classOptions.Modifier.ToString().ToLower()} {td.ColumnName}: {td.ConvertedType};");
				constructorList.Add($"_{td.ColumnName}{(td.IsNullable ? "?" : "")}: {td.ConvertedType}");
			});



			stringBuilder.AppendLine("");
			stringBuilder.AppendLine($"\tconstructor ({string.Join(", ", constructorList.ToArray())})");

			tableSchama.ToList()
			.ForEach(td =>
			{
				stringBuilder.AppendLine($"\t\tthis.{td.ColumnName} = _{td.ColumnName};");
			});

			stringBuilder.AppendLine("\t}");
			stringBuilder.AppendLine("}");

			return stringBuilder.ToString();
		}

		public override string GetTableDefinitoinCommandString()
		{
			var command = $@"DECLARE @tableName varchar(50) = '{this._classOptions.TableName}';

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
									   WHEN 'bigint' THEN 'number'
									   WHEN 'binary' THEN 'any'
									   WHEN 'bit' THEN 'boolean'
									   WHEN 'char' THEN 'string'
									   WHEN 'date' THEN 'Date'
									   WHEN 'datetime' THEN 'Date'
									   WHEN 'datetime2' THEN 'Date'
									   WHEN 'datetimeoffset' THEN 'Date'
									   WHEN 'decimal' THEN 'number'
									   WHEN 'float' THEN 'number'
									   WHEN 'image' THEN 'Array<number>'
									   WHEN 'int' THEN 'number'
									   WHEN 'money' THEN 'number'
									   WHEN 'nchar' THEN 'string'
									   WHEN 'ntext' THEN 'string'
									   WHEN 'numeric' THEN 'number'
									   WHEN 'nvarchar' THEN 'string'
									   WHEN 'real' THEN 'number'
									   WHEN 'smalldatetime' THEN 'Date'
									   WHEN 'smallint' THEN 'number'
									   WHEN 'smallmoney' THEN 'number'
									   WHEN 'text' THEN 'string'
									   WHEN 'time' THEN 'Date'
									   WHEN 'timestamp' THEN 'number'
									   WHEN 'tinyint' THEN 'number'
									   WHEN 'uniqueidentifier' THEN 'string'
									   WHEN 'varbinary' THEN 'Array<number>'
									   WHEN 'varchar' THEN 'string'
									   ELSE 'UNKNOWN_' + stp.name
								   END AS ConvertedType
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

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
