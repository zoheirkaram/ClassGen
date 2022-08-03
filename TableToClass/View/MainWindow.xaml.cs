using DBContext;
using Common.Enums;
using Common.Classes;
using Converter;
using System;
using System.Windows;
using System.Linq;

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

			this.cboObjectTypes.SelectedIndex = 0;
			this.cboModifiers.SelectedIndex = 0;
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

					var converter = new TypeScriptConverter(classOptions);

					var code = await converter.GetClass(context);
					var @class = converter.GetHighlightedHtmlCode(code);

					this.htmlDisplay.NavigateToString(@class);
					this.btnGenerateClass.IsEnabled = true;
				}

			}
			catch (Exception ex)
            {
				MessageBox.Show(ex.InnerException?.Message ?? ex.Message, "Exception");
				this.btnGenerateClass.IsEnabled = true;
			}
		}
	}
}
