using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Classes;
using Common.Enums;
using Pluralize.NET.Core;
using Converter;

namespace ClassConverter
{
	public class CSharpConverter : BaseConverter, IConverter, IDisposable
    {
		private ConvertOptions _classOptions;

		public CSharpConverter(string tableName)
		{
			this._classOptions = new ConvertOptions() { TableName = tableName };
		}

		public CSharpConverter(ConvertOptions classOptions)
		{
			this._classOptions = classOptions ?? new ConvertOptions();
		}

		public string GetClass()
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
							_ = stringBuilder.AppendLine($"\t[MaxLength(\"{td.MaxLength}\")]");
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

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.TableSchama = null;
				this._classOptions = null;
			}
		}
	}
}
