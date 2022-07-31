using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Classes;
using Common.Enums;
using Pluralize.NET.Core;
using Skybrud.SyntaxHighlighter;

namespace Converter
{
	public class TableConverter
    {
		public List<TableSchemaResult> TableSchama { set; get; }

		private ConvertOptions _classOptions;
		private CSharpHighlightColor _highlightColors;
		private string _style = string.Empty;

		public TableConverter(string tableName)
		{
			this._classOptions = new ConvertOptions() { TableName = tableName };
			this.setStyle();
		}

		public TableConverter(ConvertOptions classOptions)
		{
			this._classOptions = classOptions ?? new ConvertOptions();
			this.setStyle();
		}

		public TableConverter(ConvertOptions classOptions, CSharpHighlightColor cSharpHighlightColor)
		{
			this._classOptions = classOptions ?? new ConvertOptions();
			this._highlightColors = cSharpHighlightColor ?? new CSharpHighlightColor();
			this.setStyle();
		}

		public string GetCSharpClass()
		{
			var nullableSqlTypes = new List<string> { "bigint, bit, date, datetime, datetime2, datetimeoffset, decimal, float, int, money, numeric, real, smalldatetime, smallint, smallmoney, time, tinyint, uniqueidentifier" };
			var stringBuilder = new StringBuilder();

			if (this._classOptions.ClassType == ClassType.Entity)	
			{
				stringBuilder.AppendLine("using System.ComponentModel.DataAnnotations;");

				if (this.TableSchama.Any(td => td.HasReference))
				{
					stringBuilder.AppendLine("using System.ComponentModel.DataAnnotations.Schema;");
				}

				stringBuilder.AppendLine("");
				stringBuilder.AppendLine("-----");
				stringBuilder.AppendLine("");
				if (this._classOptions.ShowTableName)
                {
					stringBuilder.AppendLine($"[Table(\"{this._classOptions.TableName}\")]");
                }
			}

			stringBuilder.AppendLine($"{this._classOptions.Modifier.ToString().ToLower()} class {new Pluralizer().Singularize(this._classOptions.TableName)}");
			stringBuilder.AppendLine("{");

			this.TableSchama.ToList()
			.ForEach(td =>
			{
				if (this._classOptions.ClassType == ClassType.Entity)
				{
					if (td.IsPrimaryKey && this._classOptions.ShowPrimaryKey)
					{
						stringBuilder.AppendLine("\t[Key]");
					}

					if (td.cSharpType.Equals("string", StringComparison.OrdinalIgnoreCase) && this._classOptions.ShowMaxLength)
					{
						if (td.MaxLength > 0)
						{
							stringBuilder.AppendLine($"\t[MaxLength(\"{td.MaxLength}\")]");
						}
					}
				}

				stringBuilder.AppendLine($"\tpublic {td.cSharpType}{(td.IsNullable && nullableSqlTypes.Where(st => st == td.TypeName) != null && !"string,byte[]".Contains(td.cSharpType) ? "?" : "")} {td.ColumnName} {{ get; set; }}");

				if (this._classOptions.ShowForeignProperty)
				{
					if (td.HasReference)
					{
						var referenceClass = new Pluralizer().Singularize(td.ReferencedTableName);

						if (this._classOptions.ClassType == ClassType.Entity && this._classOptions.ShowForeignKey)
						{
							stringBuilder.AppendLine($"\t[ForeignKey(\"{td.ColumnName}\")]");
						}
						stringBuilder.AppendLine($"\tpublic virtual {referenceClass} {(this._classOptions.EnumerateSimilarForeignKeyProperties ? $"{referenceClass}_{td.ReferencedTableNumber}" : $"{td.ColumnName}_{referenceClass}")} {{ get; set; }}");
					}
				}
			});

			stringBuilder.AppendLine("}");

			return stringBuilder.ToString();
		}

		public string GetHighlightedCSharpClass()
		{
			string html = Highlighter.HighlightCSharp(this.GetCSharpClass());

			html = html.Insert(0, $"<style>{_style}</style><body>").Replace("-----", "<hr />");
			html = html.Insert(html.Length, "</body>");

			return html;
		}

		private void setStyle()
		{
			if (_highlightColors == null)
			{
				this._highlightColors = new CSharpHighlightColor();
			}

			_style = $@"
						body {{ background-color: {_highlightColors.BackgroundColor};}}
						.highlight.csharp {{ color: #333; font-size: 12px; line-height: 16px; }}
						.highlight.csharp .identifier {{ color: #{_highlightColors.IdentifierColor}; }}
						.highlight.csharp .keyword {{ color: #{_highlightColors.KeywordColor}; }}
						.highlight.csharp .comment {{ color: #{_highlightColors.CommentColor}; }}
						.highlight.csharp .string {{ color: #{_highlightColors.StringColor}; }}
						.highlight.csharp .constant {{ color: #{_highlightColors.ConstantColor}; }}
					";
		}

	}
}
