using Common.Enums;

namespace Common.Classes
{
	public class ConvertOptions
	{
		public string TableName { get; set; }
		public Modifier Modifier { get; set; } = Modifier.Public;
		public ClassType ClassType { get; set; } = ClassType.Contract;
		public bool ShowPrimaryKey { get; set; } = true;
		public bool ShowMaxLength { get; set; } = true;
		public bool ShowTableName { get; set; } = true;
		public bool ShowForeignKey { get; set; } = true;
		public bool ShowForeignProperty { get; set; } = true;
		public bool EnumerateSimilarForeignKeyProperties { get; set; } = false;
	}
}
