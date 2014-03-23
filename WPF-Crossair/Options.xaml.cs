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
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;

namespace WPF_Crosshair {
	/// <summary>
	/// Interaction logic for Options.xaml
	/// </summary>
	public partial class Options : Window {
		public Config configs = null;
		public Config tempConfig = new Config();
		public delegate void Reload(String s);
		private Reload rel;

		public delegate void Done(Config c);
		public event Done OnAccept;

		public Options(Config configin, Reload rel) {
			InitializeComponent();

			ToggleBind.OnNewBind += BindChanged;
			configs = configin;
			tempConfig = configs;

			this.rel = rel;

			FilePath.Text = configs.CrosshairPath;
			FilePath.TextWrapping = TextWrapping.NoWrap;
			ToggleBind.keyBind = configs.ShowHideCrosshair != null ? configs.ShowHideCrosshair.KeyList : new List<Keys>();
			ToggleBind.updateText();
			ExitWith.IsChecked = configs.ExitWithProgram;
			TargetWindow.Text = configs.TargetWindowTitle;

			this.ResizeMode = System.Windows.ResizeMode.NoResize;

		}

		private void BindChanged(List<Keys> bind) {
			tempConfig.ShowHideCrosshair = new HotKey(bind);
		}

		private void ReloadButton_Click(object sender, RoutedEventArgs e) {
			rel(tempConfig.CrosshairPath);
		}

		private void OkButton_Click(object sender, RoutedEventArgs e) {
			tempConfig.CrosshairPath = FilePath.Text;
			
			if (OnAccept != null)
				OnAccept(tempConfig);

			this.Close();
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}

		private void BrowseButton_Click(object sender, RoutedEventArgs e) {
			Microsoft.Win32.OpenFileDialog diag = new Microsoft.Win32.OpenFileDialog();
			diag.InitialDirectory = Directory.GetCurrentDirectory();
			diag.CheckFileExists = true;
			diag.DefaultExt = "png";
			diag.Multiselect = false;
			

			// Process open file dialog box results 
			if (diag.ShowDialog() == true)
			{ 
				string filename = diag.FileName;
				FilePath.Text = filename;
				tempConfig.CrosshairPath = filename;
			}
		}

		private void ExitWith_Checked(object sender, RoutedEventArgs e) {
			tempConfig.ExitWithProgram = ExitWith.HasContent ? (bool)ExitWith.IsChecked : false;
		}

		private void FilePath_TextChanged(object sender, TextChangedEventArgs e) {
			tempConfig.CrosshairPath = FilePath.Text;
		}

		private void TargetWindow_TextChanged(object sender, TextChangedEventArgs e) {
			tempConfig.TargetWindowTitle = TargetWindow.Text;
		}

		private void TestTarget_Click(object sender, RoutedEventArgs e) {
			Regex t = new Regex(tempConfig.TargetWindowTitle);
			Process win = Process.GetProcesses().FirstOrDefault(x => t.Match(x.MainWindowTitle).Success);

			if (win != null)
				System.Windows.MessageBox.Show("Found a window. \n Window title: " + win.MainWindowTitle, "Found Matching Window", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
			else
				System.Windows.MessageBox.Show("Can't match a window, are you sure you spelled it right?", "Error: No Matching Window", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
		}

	}
}
