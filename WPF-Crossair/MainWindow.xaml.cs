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
		private AsyncGlobalShortcuts hotKeys = new AsyncGlobalShortcuts();
		private GameWindow testWindow = null;

		public MainWindow() {
			InitializeComponent();

			//Sets the windows properties
			this.WindowStyle = System.Windows.WindowStyle.None;
			this.Focusable = false;
			this.ShowInTaskbar = false;
			this.Topmost = true;
			this.ShowActivated = false;
			this.Hide();
			this.Opacity = 0;

			//try to read in the options file
			try {
				DataReader.Deserialize(configs);
			} catch (FileNotFoundException) {}

			if (!configs.Initilized) {
				configs.Initilized = true;
				configs.ResetHotKeys();
			}

			//register events and start the main update clock.
			this.Closed += OnClose;

			updateTimer = new Timer(OnTick, null, 0, 1000);

			hotKeys.RegisterHotKey(configs.ShowHideCrosshair);
			hotKeys.KeyPressed += hotkeyHandler;

			testWindow = new GameWindow(configs.TargetWindowTitle, configs.CrosshairPath);
			testWindow.isEnabled = true;

			ChangeIcon();
		}

		public void OnClose(object o, EventArgs e) {
			DataReader.Serialize(configs);
		}

		private void OnTick(object call) {
			if (testWindow == null) return;
			testWindow.OnTick();
			
			this.Dispatcher.Invoke(new Action(() => { 
				ChangeIcon();

				if (!testWindow.hasWindow && configs.ExitWithProgram)
					Application.Current.Shutdown();
			}));

		}

		private void hotkeyHandler(object source, KeyPressedEventArgs e) {
			if (e.Key == configs.ShowHideCrosshair) {
				enabled = !enabled;
				testWindow.isEnabled = !testWindow.isEnabled;
				ChangeIcon();
			}
		}

		private void ChangeIcon() {
			this.Dispatcher.Invoke(new Action(() => {
				if (enabled) {
					if (testWindow.isFocused) {
						TrayIcon.Icon = Properties.Resources.on;
					} else {
						TrayIcon.Icon = Properties.Resources.paused;
					}
				} else {
					TrayIcon.Icon = Properties.Resources.off;
				}
				TrayIcon.UpdateLayout();
			}));
		}

		private void ExitContext_Click(object sender, RoutedEventArgs e) {
			Application.Current.Shutdown();
		}

		private void OptionsContext_Click(object sender, RoutedEventArgs e) {
			var test = new Options(configs);
			test.OnAccept += OptionsAccept;
			test.Show();
		}

		private void OptionsAccept(Config src, GameWindow newWind) {
			hotKeys.UnregisterHotKey(configs.ShowHideCrosshair);
			configs = src;
			hotKeys.RegisterHotKey(configs.ShowHideCrosshair);

			bool en = testWindow.isEnabled;
			testWindow = newWind;
			testWindow.LoadImage(configs.CrosshairPath);
			testWindow.isEnabled = en;

			DataReader.Serialize(configs);
		}

		private void EnabledContext_Click(object sender, RoutedEventArgs e) {
			enabled = !enabled;
			EnabledContext.IsChecked = enabled;
			ChangeIcon();
		}

	}
}
