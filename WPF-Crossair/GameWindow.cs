using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Controls;
using System.IO;
using System.Diagnostics;

namespace WPF_Crosshair {
	public class GameWindow {
		private Regex windowRegex = null;
		private Window overlay = null;
		private BitmapImage crosshair = null;
		public bool isFocused { get; private set; }
		public bool isEnabled { get; set; }
		public bool hasWindow { get; private set; }

		public GameWindow(String regex, String imgPath) {
			if (regex != null)
				windowRegex = new Regex(regex, RegexOptions.Compiled);

			overlay = new Window();
				overlay.WindowStyle = System.Windows.WindowStyle.None;
				overlay.AllowsTransparency = true;
				overlay.Background = Brushes.Transparent;
				overlay.Focusable = false;
				overlay.ShowInTaskbar = false;
				overlay.Topmost = true;
				overlay.ShowActivated = false;
				overlay.Opacity = 0;
				overlay.Content = new Image();
			overlay.Show();

			if (imgPath != null)
				LoadImage(imgPath);

			hasWindow = true; //set to true at first so it has a chance to load
		}

		/// <summary>
		/// Repositiones the pverlay and toggles its visibility
		/// </summary>
		public void OnTick() {
			if (!isEnabled || windowRegex == null || crosshair == null) { overlay.Dispatcher.Invoke(new Action(() => { overlay.Opacity = 0; })); return; }
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
				overlay.Dispatcher.Invoke(new Action(() => { overlay.Opacity = 0; }));
				return;
			}

			Haax.RECT tempSize = new Haax.RECT();
			Haax.GetWindowRect(GameWindow, ref tempSize);

			overlay.Dispatcher.Invoke(new Action(() => {
				if (isFocused)
					overlay.Opacity = 100;
				else
					overlay.Opacity = 0;					

				double newX = ((double)tempSize.Left + ((double)tempSize.Right - (double)tempSize.Left) / 2 - overlay.Width / 2);
				double newY = ((double)tempSize.Top + ((double)tempSize.Bottom - (double)tempSize.Top) / 2 - overlay.Height / 2);

				overlay.Left = newX;
				overlay.Top = newY;
			}));
		}

		/// <summary>
		/// Sets the window title regex
		/// </summary>
		/// <param name="title">The title</param>
		public void setWindowTitleRegex(String title) {
			windowRegex = new Regex(title, RegexOptions.Compiled);
		}

		/// <summary>
		/// Tests the regex and tries to find our window.
		/// </summary>
		public void test() {
			IntPtr GameWindow = IntPtr.Zero;
				GameWindow = Process.GetProcesses().FirstOrDefault(x => windowRegex.Match(x.MainWindowTitle).Success).MainWindowHandle;

			if (GameWindow != IntPtr.Zero) {
				Haax.SetForegroundWindow(GameWindow);
				//TODO: add some more functionality, like highliting?
			} else
				MessageBox.Show("Can't find window!\nIs your app running?", "No Match", System.Windows.MessageBoxButton.OK, MessageBoxImage.Warning);			
		}

		/// <summary>
		/// Tries to load the indicated image.
		/// </summary>
		/// <param name="Path">The image filepath</param>
		/// <returns>True if image is loaded succesfully</returns>
		/// <exception cref="FileLoadException"/>
		/// <exception cref="FileNotFoundException"/>
		public bool LoadImage(String Path) {
			Path = System.IO.Path.Combine(Environment.CurrentDirectory, Path);

			if (!File.Exists(Path)) {
				throw new FileNotFoundException("Crosshair not found!");

			} else {
				crosshair = new BitmapImage();
				crosshair.DownloadFailed += (object o, ExceptionEventArgs e) => { throw new FileLoadException("Failed to load crosshair. File might be corrupted?"); };

				try {
					crosshair.BeginInit();
					crosshair.UriSource = new Uri(Path);
					crosshair.EndInit();
				} catch (Exception) { throw new FileLoadException("Failed to load crosshair. File might be corrupted?"); }


				overlay.Width = crosshair.PixelWidth;
				overlay.Height = crosshair.PixelHeight;

				getOverlay().Source = crosshair;
				getOverlay().Width = crosshair.PixelWidth;
				getOverlay().Height = crosshair.PixelHeight;

				return true;
			}
		}

		/// <summary>
		/// Gets the image control
		/// </summary>
		/// <returns>The image control</returns>
		public Image getOverlay() {
			return (Image)overlay.Content;
		}

	}
}
