using Common.Classes;
using Converter;
using Common.Enums;
using Context;
using System.Threading.Tasks;
using System;

namespace ConsoleTest
{
	class Program
	{
		static async Task Main(string[] args)
		{
			var connectinoString = @"Data Source=.;Integrated Security=SSPI;Initial Catalog=AdventureWorks2017";
			var tableName = "WorkOrder";
			var context = new SqlContext(connectinoString);

			context.SetTableName(tableName);

			var classOptions = new ConvertOptions
			{
				TableName = tableName,
				Modifier = Modifier.Public,
				ClassType = ClassType.Entity,
				ShowForeignKey = true,
				ShowForeignProperty = true,
				ShowMaxLength = true,
				ShowPrimaryKey = true,
				ShowTableName = true,
				EnumerateSimilarForeignKeyProperties = false
			};
			var highlightColors = new CSharpHighlightColor() { KeywordColor = "1d44a7" };

			//var convert = new TableConverter(tableName);
			var convert = new TableConverter(classOptions);
			//var convert = new TableConverter(classOptions, highlightColors);

			var tableSchemaResult = await context.GetTableData<TableSchemaResult>();

			convert.TableSchama = tableSchemaResult;

			var @class = convert.GetCSharpClass();
			//var classHtmlDocument = convert.GetHighlightedCSharpClass();

			Console.WriteLine(@class);
			//Console.WriteLine(classHtmlDocument);
			Console.ReadLine();
		}
	}
}
