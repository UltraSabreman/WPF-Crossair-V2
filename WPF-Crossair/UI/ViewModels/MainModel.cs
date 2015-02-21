using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPF_Crosshair {
	class MainModel : ViewModelNotifier {
		public BitmapImage Reticule { get { return GetProp<BitmapImage>(); } set { SetProp(value); } }
		public int Width { get { return GetProp<int>(); } set { SetProp(value); } }
		public int Height { get { return GetProp<int>(); } set { SetProp(value); } }
		public double Left { get { return GetProp<double>(); } set { SetProp(value); } }
		public double Top { get { return GetProp<double>(); } set { SetProp(value); } }
		public int Opacity { get { return GetProp<int>(); } set { SetProp(value); } }
		public bool TopMost { get { return GetProp<bool>(); } set { SetProp(value); } }
		public bool IsEnabled { get { return GetProp<bool>(); } set { SetProp(value); } }


		private Regex windowRegex = null;
		private MainWindow mainWindow = null;
		private bool isFocused = false;
		private bool hasWindow = false;

		public MainModel(MainWindow win) {
			String regex = Configs.Properties["TargetTitle"] as String;
			if (regex != null)
				windowRegex = new Regex(regex, RegexOptions.Compiled);

			mainWindow = win;

			mainWindow.DataContext = this;
			mainWindow.RetImage.DataContext = this;
			IsEnabled = true;
			TopMost = true;


			try {
				LoadImage(Configs.Properties["ImagePath"] as String);
			} catch (FileNotFoundException e) {
				//TODO: handle file not found
			}

			mainWindow.Show();
			//This has to happen after the window is visible (aka after show)
			Haax.SetWindowExTransparent(new WindowInteropHelper(mainWindow).Handle);
		}

		public void Update(Object o, EventArgs e) {
			if (!IsEnabled || windowRegex == null || Reticule == null) {
				Opacity = 0;
				return; 
			}
			IntPtr GameWindow = IntPtr.Zero;

			try {
				GameWindow = Process.GetProcesses().FirstOrDefault(x => windowRegex.Match(x.MainWindowTitle).Success).MainWindowHandle;
				if (GameWindow == Haax.GetForegroundWindow()) {
					isFocused = true;
					hasWindow = true;
				} else {
					hasWindow = true;
					isFocused = false;
				}
			} catch (System.NullReferenceException) {
				isFocused = false;
				hasWindow = false;
				Opacity = 0;
				//System.Diagnostics.Trace.WriteLine("Window:0, Focus:0");
				return;
			}

			Haax.RECT tempSize = new Haax.RECT();
			Haax.GetWindowRect(GameWindow, ref tempSize);


			if (isFocused)
				Opacity = 100;
			else
				Opacity = 0;

			double newX = ((double)tempSize.Left + ((double)tempSize.Right - (double)tempSize.Left) / 2 - Width / 2);
			double newY = ((double)tempSize.Top + ((double)tempSize.Bottom - (double)tempSize.Top) / 2 - Height / 2);

			Left = newX;
			Top = newY;
		}


		public bool LoadImage(String Path) {
			Path = System.IO.Path.Combine(Environment.CurrentDirectory, Path);

			if (!File.Exists(Path)) {
				throw new FileNotFoundException("Crosshair not found!");

			} else {
				Reticule = new BitmapImage();
				Reticule.DownloadFailed += (object o, ExceptionEventArgs e) => { throw new FileLoadException("Failed to load crosshair. File might be corrupted?"); };

				try {
					Reticule.BeginInit();
					Reticule.UriSource = new Uri(Path);
					Reticule.EndInit();
				} catch (Exception) { throw new FileLoadException("Failed to load crosshair. File might be corrupted?"); }


				Width = Reticule.PixelWidth;
				Height = Reticule.PixelHeight;

				return true;
			}

		}


		public void focusTargetWindo() {
			IntPtr GameWindow = IntPtr.Zero;
			GameWindow = Process.GetProcesses().FirstOrDefault(x => windowRegex.Match(x.MainWindowTitle).Success).MainWindowHandle;

			if (GameWindow != IntPtr.Zero) {
				Haax.SetForegroundWindow(GameWindow);
				//TODO: add some more functionality, like highliting?
			} else
				MessageBox.Show("Can't find window!\nIs your app running?", "No Match", System.Windows.MessageBoxButton.OK, MessageBoxImage.Warning);
		}


		public void setWindowTitleRegex(String title) {
			windowRegex = new Regex(title, RegexOptions.Compiled);
		}
	}
}
