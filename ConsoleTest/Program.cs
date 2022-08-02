using Common.Classes;
using Common.Enums;
using Context;
using Converter;
using System;
using System.Threading.Tasks;

namespace ConsoleTest
{
    class Program
	{
		static async Task Main(string[] args)
		{
			var connectinoString = @"Data Source=DESKTOP-UA91BKA\SQLEXPRESS;Initial Catalog=PTC;Integrated Security=SSPI";
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
			var highlightColors = new HighlightColor() { Keyword = "1d44a7" };

			//var convert = new TableConverter(tableName);
			var convert = new CSharpConverter(classOptions);
			//var convert = new CSharpConverter(classOptions, highlightColors);

			var tableSchemaResult = await context.GetTableData<TableSchemaResult>();

			convert.TableSchama = tableSchemaResult;

			var @class = convert.GetClass();
			var classHtmlDocument = convert.GetHighlightedClass();

			var st = new SimpleTokenizer.CSharpTokenizer();
			var tokens = st.GetTokens(@class);
			var html = st.Highlight(tokens);
			//tokens.ForEach(token =>
			//{
			//	Console.WriteLine($"{token.Type} \t\t {token.LineNumber} \t [{token.PositionStart}, {token.SymbolLength}] \t\t {token.Symbol}");
			//});

			//Console.WriteLine(@class);
			Console.WriteLine(classHtmlDocument);
			//Console.WriteLine(html);
			Console.ReadLine();
		}
	}
}
