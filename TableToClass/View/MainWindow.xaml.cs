using DBContext;
using Common.Enums;
using Common.Classes;
using Converter;
using System;
using System.Windows;
using System.Linq;
using System.Threading.Tasks;

namespace TableToClass
{
	public partial class MainWindow : Window
	{
		public SqlContext context;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void OnLoad(object sender, RoutedEventArgs e)
		{
			this.htmlDisplay.NavigateToString(Util.DefaultScreen());

			this.cboObjectTypes.ItemsSource = Enum.GetValues(typeof(ClassType)).Cast<ClassType>();
			this.cboModifiers.ItemsSource = Enum.GetValues(typeof(Modifier)).Cast<Modifier>();
			this.cboLanguages.ItemsSource = Enum.GetValues(typeof(Language)).Cast<Language>();

			this.cboObjectTypes.SelectedIndex = 0;
			this.cboModifiers.SelectedIndex = 0;
			this.cboLanguages.SelectedIndex = 0;
		}

		private async void Button_Connect_Click(object sender, RoutedEventArgs e)
		{
            try
            {
				this.btnConnect.IsEnabled = false;
				this.context = new SqlContext();

				context.SetConnectionString(this.txtConnection.Text);

				this.cboTables.ItemsSource = await this.context.GetTablesAsync();
				this.btnConnect.IsEnabled = true;

			}
			catch (Exception ex)
            {
				MessageBox.Show(ex.InnerException?.Message ?? ex.Message, "Exception");
				this.btnConnect.IsEnabled = true;
            }
		}

		private async void Button_GenerateClass_Click(object sender, RoutedEventArgs e)
		{
            try
            {
				if (this.cboTables.SelectedIndex > -1)
				{

					this.btnGenerateClass.IsEnabled = false;

					await this.GenerateCode();

					this.btnGenerateClass.IsEnabled = true;
				}

			}
			catch (Exception ex)
            {
				MessageBox.Show(ex.InnerException?.Message ?? ex.Message, "Exception");
				this.btnGenerateClass.IsEnabled = true;
			}
		}

		private async Task GenerateCode()
		{

			var classOptions = new ConvertOptions
			{
				TableName = this.cboTables.Text,
				Modifier = (Modifier)Enum.Parse(typeof(Modifier), this.cboModifiers.SelectedValue.ToString()),
				ClassType = (ClassType)Enum.Parse(typeof(ClassType), this.cboObjectTypes.SelectedValue.ToString()),
				ShowForeignKey = this.chkShowForeignKey.IsChecked ?? false,
				ShowForeignProperty = this.chkShowForeignProperty.IsChecked ?? false,
				ShowMaxLength = this.chkShowMaxLength.IsChecked ?? false,
				ShowPrimaryKey = this.chkShowPrimaryKey.IsChecked ?? false,
				ShowTableName = this.chkShowTableName.IsChecked ?? false,
				EnumerateSimilarForeignKeyProperties = this.chkEnumerateSimilarFKProperties.IsChecked ?? false
			};

			var lang = (Language)Enum.Parse(typeof(Language), this.cboLanguages.SelectedValue.ToString());
			var cSharpConverter = new CSharpConverter(classOptions);
			var typeScriptConverter = new TypeScriptConverter(classOptions);

			string code;
			var @class = string.Empty;


			switch (lang)
			{
				case Common.Enums.Language.CSharp:
					code = await cSharpConverter.GetClass(this.context);
					@class = cSharpConverter.GetHighlightedHtmlCode(code);
					break;

				case Common.Enums.Language.TypeScript:
					code = await typeScriptConverter.GetClass(this.context);
					@class = typeScriptConverter.GetHighlightedHtmlCode(code);
					break;
			}

			this.htmlDisplay.NavigateToString(@class);

		}
	}
}
