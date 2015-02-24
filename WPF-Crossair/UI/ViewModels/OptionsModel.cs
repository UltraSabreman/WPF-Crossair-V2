using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Crosshair {
	using System.Diagnostics;
	using System.IO;
	using System.Text.RegularExpressions;
	using System.Windows;
	using Keys = System.Windows.Forms.Keys;
	class OptionsModel : ViewModelNotifier {
		public String FilePath { get { return GetProp<String>(); } set { SetProp(value); } }
		public String TargetWindow { get { return GetProp<String>(); } set { SetProp(value); } }
		public bool ExitWithProgram { get { return GetProp<bool>(); } set { SetProp(value); } }
			
		//Hack, find a way to bind this
		private Options options;

		public delegate void Accepted();
		public event Accepted OnAccept;

		//TODO: Handle file exceptions
		//TODO: Handle binds + special cases better
		//TODO: Handle test button

		public OptionsModel(Options b) {
			options = b;

			init();

			options.CancelButton.Click += CancelButton_Click;
			options.OkButton.Click += OkButton_Click;
			options.ReloadButton.Click += ReloadButton_Click;
			options.BrowseButton.Click += BrowseButton_Click;
			options.TestTarget.Click += TestTarget_Click;
		}
	

		private void ReloadButton_Click(object sender, RoutedEventArgs e) {
			init();
		}

		private void init() {
			FilePath = Configs.Properties["ImagePath"] as String;
			HotKey key = Configs.convertTo<HotKey>(Configs.Properties["HotKey"]);
			if (key == null)
				options.ToggleBind.KeyBind = new List<Keys>();
			else
				options.ToggleBind.KeyBind = key.KeyList;

			options.ToggleBind.updateText();
			ExitWithProgram = (bool)Configs.Properties["ExitWithProgram"];
			TargetWindow = Configs.Properties["TargetTitle"] as String;
		}

		private void OkButton_Click(object sender, RoutedEventArgs e) {

			Configs.Properties["ImagePath"] = FilePath;
			Configs.Properties["HotKey"] = new HotKey(options.ToggleBind.KeyBind);
			Configs.Properties["ExitWithProgram"] = ExitWithProgram;
			Configs.Properties["TargetTitle"] = TargetWindow;
			Configs.Properties.Save();

			if (OnAccept != null)
				OnAccept();

			options.Close();
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e) {
			options.Close();
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
				FilePath = filename;
			}
		}

		private void TestTarget_Click(object sender, RoutedEventArgs e) {
			IntPtr GameWindow = IntPtr.Zero;
			GameWindow = Process.GetProcesses().FirstOrDefault(x => new Regex(TargetWindow, RegexOptions.Compiled).Match(x.MainWindowTitle).Success).MainWindowHandle;

			if (GameWindow != IntPtr.Zero) {
				Haax.SetForegroundWindow(GameWindow);
				//TODO: add some more functionality, like highliting?
			} else
				MessageBox.Show("Can't find window!\nIs your app running?", "No Match", System.Windows.MessageBoxButton.OK, MessageBoxImage.Warning);
		}
	}
}
