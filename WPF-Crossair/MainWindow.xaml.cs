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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using System.Text.RegularExpressions;
using System.Diagnostics;
//using System.Drawing;



namespace WPF_Crosshair {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		private Config configs = new Config();
		private bool enabled = true;
		private bool focused = false;
		private Timer updateTimer = null;
		private Regex windowTitle = null;
		private AsyncGlobalShortcuts hotKeys = new AsyncGlobalShortcuts();

		public MainWindow() {
			InitializeComponent();

			//Sets the windows properties
			this.WindowStyle = System.Windows.WindowStyle.None;
			this.AllowsTransparency = true;
			this.Background = Brushes.Transparent;
			this.Focusable = false;
			this.ShowInTaskbar = false;
			this.Topmost = true;
			this.ShowActivated = false;
			this.Opacity = 0;

			//try to read in the options file
			try {
				DataReader.Deserialize(configs);
			} catch (FileNotFoundException) {}

			if (!configs.Initilized) {
				configs.Initilized = true;
				configs.ResetHotKeys();
			}

			LoadImage();

			//register events and start the main update clock.
			this.Closed += OnClose;

			updateTimer = new Timer(OnTick, null, 0, 1000);
			windowTitle = new Regex(configs.TargetWindowTitle, RegexOptions.Compiled);

			hotKeys.RegisterHotKey(configs.ShowHideCrosshair);
			hotKeys.KeyPressed += hotkeyHandler;			

			ChangeIcon();

			var test = new Options(configs);
			test.Show();
		}

		public void OnClose(object o, EventArgs e) {
			DataReader.Serialize(configs);
		}

		private void OnTick(object call) {
			IntPtr GameWindow = new IntPtr();

			try {
				GameWindow = Process.GetProcesses().FirstOrDefault(x => windowTitle.Match(x.MainWindowTitle).Success).MainWindowHandle;
				if (GameWindow == Haax.GetForegroundWindow())
					focused = true;
				else 
					focused = false;
			} catch (System.NullReferenceException) {
				focused = false;
				if (configs.ExitWithProgram)
					Application.Current.Shutdown();

				this.Dispatcher.Invoke(new Action(() => { ChangeIcon(); }));

				return;
			}

			Haax.RECT tempSize = new Haax.RECT();
			Haax.GetWindowRect(GameWindow, ref tempSize);

			this.Dispatcher.Invoke(new Action(() => {
				double newX = ((double)tempSize.Left + ((double)tempSize.Right - (double)tempSize.Left) / 2 - this.Width / 2);
				double newY = ((double)tempSize.Top + ((double)tempSize.Bottom - (double)tempSize.Top) / 2 - this.Height / 2);

				this.Left = newX;
				this.Top = newY;

				ChangeIcon();
			}));
		}

		private void hotkeyHandler(object source, KeyPressedEventArgs e) {
			if (e.Key.First() == configs.ShowHideCrosshair.First()) {
				enabled = !enabled;
				ChangeIcon();
			}
		}

		private void ChangeIcon() {
			if (enabled) {
				if (focused) {
					this.Opacity = 100;
					TrayIcon.Icon = Properties.Resources.on;
				} else {
					this.Opacity = 0;
					TrayIcon.Icon = Properties.Resources.paused;
				}
			} else {
				this.Opacity = 0;
				TrayIcon.Icon = Properties.Resources.off;
			}
			TrayIcon.UpdateLayout();
		}

		private void LoadImage() {
			String path = System.IO.Path.Combine(Environment.CurrentDirectory, configs.CrosshairPath);
			bool tryAgain = true;

			while (tryAgain) {
				if (!File.Exists(path)) {
					if (MessageBox.Show("Crosshair file not found! \nClick OK to retry loading or click Cancel to quit", "Error Loading File", System.Windows.MessageBoxButton.OKCancel, MessageBoxImage.Error) == System.Windows.MessageBoxResult.Cancel)
						Application.Current.Shutdown();
					else
						continue;
				} else
					tryAgain = false;

				BitmapImage image = new BitmapImage();
				image.DownloadFailed += (object o, ExceptionEventArgs e) => {
					if (MessageBox.Show("Crosshair failed to load (it may be corrupted)! \nClick OK to retry loading or click Cancel to quit", "Error Loading File", System.Windows.MessageBoxButton.OKCancel, MessageBoxImage.Error) == System.Windows.MessageBoxResult.Cancel)
						Application.Current.Shutdown();
					else
						LoadImage();
				};

				image.BeginInit();
				image.UriSource = new Uri(path);
				image.EndInit();


				this.Width = image.PixelWidth;
				this.Height = image.PixelHeight;

				Test.Source = image;
				Test.Width = image.PixelWidth;
				Test.Height = image.PixelHeight;
			}
		}

		private void ExitContext_Click(object sender, RoutedEventArgs e) {
			Application.Current.Shutdown();
		}

		private void OptionsContext_Click(object sender, RoutedEventArgs e) {
			var test = new Options(configs);
			test.Show();
		}

		private void EnabledContext_Click(object sender, RoutedEventArgs e) {
			enabled = !enabled;
			EnabledContext.IsChecked = enabled;
			ChangeIcon();
		}

	}
}
