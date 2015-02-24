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
using Newtonsoft.Json.Linq;
//using System.Drawing;



namespace WPF_Crosshair {
	using Timer = System.Timers.Timer;

	public partial class MainWindow : Window {
		private Timer updateTimer = null;
		private AsyncGlobalShortcuts hotKeys = new AsyncGlobalShortcuts();

		private System.Windows.Forms.NotifyIcon TrayIcon = null;
		

		private MainModel mainModel;

		public MainWindow() {
			InitializeComponent();

			mainModel = new MainModel(this);

			buildTray();

			updateTimer = new Timer(1000);
			updateTimer.AutoReset = true;
			updateTimer.Enabled = true;

			updateTimer.Elapsed += (o, e) => {
				if (!mainModel.IsFocused)
					TrayIcon.Icon = WPF_Crosshair.Properties.Resources.paused;
				else {
					if (mainModel.IsEnabled)
						TrayIcon.Icon = WPF_Crosshair.Properties.Resources.on;
					else
						TrayIcon.Icon = WPF_Crosshair.Properties.Resources.off;
				}

			};
			updateTimer.Elapsed += mainModel.Update;

			hotKeys.KeyPressed += hotKeys_KeyPressed;

			updateTimer.Start();

		}

		void hotKeys_KeyPressed(object sender, KeyPressedEventArgs e) {
			if (e.Key == Configs.convertTo<HotKey>(Configs.Properties["HotKey"])) {
				toggleApp();
			}
		}

		private void buildTray() {
			TrayIcon = new System.Windows.Forms.NotifyIcon();
			TrayIcon.Icon = WPF_Crosshair.Properties.Resources.on;
			TrayIcon.Visible = true;
			TrayIcon.ContextMenu = new System.Windows.Forms.ContextMenu();

			var enabledContext = new System.Windows.Forms.MenuItem("Enable", (o, e) => {
				toggleApp();				
			});

			var exitContext = new System.Windows.Forms.MenuItem("Exit", (o, e) => {	
				Application.Current.Shutdown();	
			});

			var optionsContext = new System.Windows.Forms.MenuItem("Options", (o, e) => {
				Options op = new Options();
				op.Closed += (obj, ev) => {
					tryLoadImage();
				};
				op.Show();
			});


			TrayIcon.ContextMenu.MenuItems.Add(enabledContext);
			TrayIcon.ContextMenu.MenuItems.Add(optionsContext);
			TrayIcon.ContextMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("-"));
			TrayIcon.ContextMenu.MenuItems.Add(exitContext);

			TrayIcon.DoubleClick += (o, e) => {
				toggleApp();
			};

		}

		private void toggleApp() {
			mainModel.IsEnabled = !mainModel.IsEnabled;
			if (mainModel.IsEnabled)
				TrayIcon.Icon = WPF_Crosshair.Properties.Resources.on;
			else
				TrayIcon.Icon = WPF_Crosshair.Properties.Resources.off;				
		}

		private void tryLoadImage() {
			try {
				mainModel.LoadImage(Configs.Properties["ImagePath"] as String);			
			} catch (FileLoadException) {
				System.Windows.MessageBox.Show("Crosshair failed to load, is it corrupted?", "Failed to load", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			} catch (FileNotFoundException) {
				System.Windows.MessageBox.Show("Crosshair file not found.", "File not found", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
		}
	}

}
