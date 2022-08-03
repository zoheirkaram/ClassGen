using Common.Classes;
using Common.Enums;
using DBContext;
using ClassConverter;
using System;
using System.Threading.Tasks;

namespace ConsoleTest
{
    class Program
	{
		static async Task Main(string[] args)
		{
			var connectinoString = @"Data Source=.;Initial Catalog=AdventureWorks2017;Integrated Security=SSPI";
			var tableName = "Address";
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

			//var convert = new TableConverter(tableName);
			var convert = new CSharpConverter(classOptions);

			var tableSchemaResult = await context.GetTableDataAsync<TableSchemaResult>();

			convert.TableSchama = tableSchemaResult;

			var @class = convert.GetClass();
			var classHtmlDocument = convert.GetHighlightedClass();

			var st = new SimpleTokenizer.CSharpTokenizer();
			var tokens = st.GetTokens(@class);
			//var html = st.Highlight(tokens);

			tokens.ForEach(token =>
			{
				Console.WriteLine($"{token.Type} \t\t {token.LineNumber} \t [{token.PositionStart}, {token.SymbolLength}] \t\t {token.Symbol}");
			});

			//Console.WriteLine(@class);
			//Console.WriteLine(classHtmlDocument);
			//Console.WriteLine(html);
			Console.ReadLine();
		}
	}
}
