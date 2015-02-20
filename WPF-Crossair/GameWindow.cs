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
using System.Windows.Interop;

namespace WPF_Crosshair {
	public class GameWindow {
		private Window overlay = null;
		private BitmapImage crosshair = null;
		public bool isFocused { get; private set; }
		public bool isEnabled { get; set; }
		public bool hasWindow { get; private set; }
		private Regex windowRegex = null;

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
			Haax.SetWindowExTransparent(new WindowInteropHelper(overlay).Handle);

			//if (imgPath != null)
			//	LoadImage(imgPath);

			hasWindow = true; //set to true at first so it has a chance to load
		}

		/// <summary>
		/// Repositiones the pverlay and toggles its visibility
		/// </summary>
		public void OnTick() {

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
		/// Gets the image control
		/// </summary>
		/// <returns>The image control</returns>
		public Image getOverlay() {
			return (Image)overlay.Content;
		}

	}
}
