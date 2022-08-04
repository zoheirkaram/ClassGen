namespace Common.Classes
{
	public class TableSchemaResult
	{
		public string ColumnName { get; set; }
		public string TypeName { get; set; }
		public int MaxLength { get; set; }
		public bool IsPrimaryKey { get; set; }
		public bool HasReference { get; set; }
		public bool IsNullable { get; set; }
		public string ReferencedTableName { get; set; }
		public int ReferencedTableNumber { get; set; }
		public string ConvertedType { get; set; }
	}
}
