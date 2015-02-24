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
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;

namespace WPF_Crosshair {
	/// <summary>
	/// Interaction logic for Options.xaml
	/// </summary>
	public partial class Options : Window {
	
		//private GameWindow tempWindow = null;

		//public delegate void Done(GameWindow wind);
		//public event Done OnAccept;


		public Options() {
			//tempWindow = new GameWindow(null, null);
			//tempWindow.isEnabled = false;

			InitializeComponent();
			
			FilePath.Text = Configs.Properties["ImagePath"] as String;
			HotKey key = Configs.convertTo<HotKey>(Configs.Properties["HotKey"]);
			if (key == null)
				ToggleBind.KeyBind = new List<Keys>();
			else
				ToggleBind.KeyBind = key.KeyList;

			ToggleBind.updateText();
			ExitWith.IsChecked = (bool) Configs.Properties["ExitWithProgram"];
			TargetWindow.Text = Configs.Properties["TargetTitle"] as String;

			FilePath.TextWrapping = TextWrapping.NoWrap;
			this.ResizeMode = System.Windows.ResizeMode.NoResize;
		}

		private void ReloadButton_Click(object sender, RoutedEventArgs e) {
			try {
				//tempWindow.LoadImage(Configs.Properties["ImagePath"] as String);
				System.Windows.MessageBox.Show("Crosshair loaded successfully!", "Loaded", System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);			
			} catch (FileLoadException) {
				System.Windows.MessageBox.Show("Crosshair failed to load, is it corrupted?", "Failed to load", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);			
			} catch (FileNotFoundException) {
				System.Windows.MessageBox.Show("Crosshair file not found.", "File not found", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);			
			}

			FilePath.Text = Configs.Properties["ImagePath"] as String;
			HotKey key = Configs.convertTo<HotKey>(Configs.Properties["HotKey"]);
			if (key == null)
				ToggleBind.KeyBind = new List<Keys>();
			else
				ToggleBind.KeyBind = key.KeyList;

			ToggleBind.updateText();
			ExitWith.IsChecked = (bool)Configs.Properties["ExitWithProgram"];
			TargetWindow.Text = Configs.Properties["TargetTitle"] as String;
		}

		private void OkButton_Click(object sender, RoutedEventArgs e) {
			Configs.Properties["ImagePath"] = FilePath.Text;

			try {
				//tempWindow.LoadImage(Configs.Properties["ImagePath"] as String);			
			} catch (FileLoadException) {
				System.Windows.MessageBox.Show("Crosshair failed to load, is it corrupted?", "Failed to load", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			} catch (FileNotFoundException) {
				System.Windows.MessageBox.Show("Crosshair file not found.", "File not found", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			Configs.Properties["ImagePath"] = FilePath.Text;
			Configs.Properties["HotKey"] = new HotKey(ToggleBind.KeyBind);
			Configs.Properties["ExitWithProgram"] = ExitWith.IsChecked;
			Configs.Properties["TargetTitle"] = TargetWindow.Text;

			//if (OnAccept != null)
			//	OnAccept(tempWindow);

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
			}
		}

		private void TestTarget_Click(object sender, RoutedEventArgs e) {
			//tempWindow.setWindowTitleRegex(TargetWindow.Text);
			//tempWindow.test();
		}
	}
}
