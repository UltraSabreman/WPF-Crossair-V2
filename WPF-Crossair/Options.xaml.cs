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
		private Config configs = null;
		private Config tempConfig = new Config();
		private GameWindow tempWindow = null;

		public delegate void Done(Config c, GameWindow wind);
		public event Done OnAccept;

		public Options(Config configin) {
			tempWindow = new GameWindow(configin.TargetWindowTitle, configin.CrosshairPath);
			tempWindow.isEnabled = false;

			InitializeComponent();

			ToggleBind.OnNewBind += BindChanged;
			configs = configin;
			tempConfig = configs;

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
			try {
				tempWindow.LoadImage(tempConfig.CrosshairPath);
			} catch (FileLoadException) {
				System.Windows.MessageBox.Show("Crosshair failed to load, is it corrupted?", "Failed to load", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);			
			} catch (FileNotFoundException) {
				System.Windows.MessageBox.Show("Crosshair file not found.", "File not found", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);			
			}
		}

		private void OkButton_Click(object sender, RoutedEventArgs e) {
			tempConfig.CrosshairPath = FilePath.Text;
			
			if (OnAccept != null)
				OnAccept(tempConfig, tempWindow);

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
			diag.Filter = "PNG Image (*.png)|*.png";
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
			tempConfig.ExitWithProgram = (bool)ExitWith.IsChecked;
		}

		private void FilePath_TextChanged(object sender, TextChangedEventArgs e) {
			tempConfig.CrosshairPath = FilePath.Text;
		}

		private void TargetWindow_TextChanged(object sender, TextChangedEventArgs e) {
			tempConfig.TargetWindowTitle = TargetWindow.Text;
			tempWindow.setWindowTitleRegex(TargetWindow.Text);
		}

		private void TestTarget_Click(object sender, RoutedEventArgs e) {
			tempWindow.test();
		}

	}
}
