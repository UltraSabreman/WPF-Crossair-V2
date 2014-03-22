using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Reflection;

namespace WPF_Crosshair {
	/// <summary>
	/// Interaction logic for Options.xaml
	/// </summary>
	public partial class Options : Window {
		public Config configs = null;
		public Options(Config configin) {
			InitializeComponent();

			ToggleBind.OnNewBind += BindChanged;
			configs = configin;

			FilePath.Text = configs.CrosshairPath;
			ToggleBind.keyBind = configs.ShowHideCrosshair.KeyList;
			ToggleBind.updateText();
			ExitWith.IsChecked = configs.ExitWithProgram;
			TargetWindow.Text = configs.TargetWindowTitle;

		}

		private void BindChanged(List<Keys> bind) {

		}

		private void ReloadButton_Click(object sender, RoutedEventArgs e) {

		}

		private void OkButton_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}

		private void BrowseButton_Click(object sender, RoutedEventArgs e) {
			OpenFileDialog diag = new OpenFileDialog();
			diag.InitialDirectory = Assembly.GetExecutingAssembly().Location;
	

			// Process open file dialog box results 
			if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				// Open document 
				string filename = diag.FileName;
				Console.WriteLine(filename);

			}



		}

		private void ExitWith_Checked(object sender, RoutedEventArgs e) {

		}

		private void FilePath_TextChanged(object sender, TextChangedEventArgs e) {

		}

		private void TargetWindow_TextChanged(object sender, TextChangedEventArgs e) {

		}
	}
}
