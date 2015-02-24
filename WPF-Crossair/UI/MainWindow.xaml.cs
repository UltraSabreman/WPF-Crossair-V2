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

		//private System.Windows.Forms.NotifyIcon TrayIcon = null;

		private MainModel test;

		public MainWindow() {
			InitializeComponent();


			test = new MainModel(this);

			updateTimer = new Timer(1000);
			updateTimer.AutoReset = true;
			updateTimer.Enabled = true;

			updateTimer.Elapsed += test.Update;

			Options op = new Options();
			op.Show();

			updateTimer.Start();

		}

		private void buildTray() {
			/*TrayIcon = new System.Windows.Forms.NotifyIcon();
			TrayIcon.Icon = WPF_Crosshair.Properties.Resources.on;
			TrayIcon.Visible = true;
			TrayIcon.ContextMenu = new System.Windows.Forms.ContextMenu();

			var enabledContext = new System.Windows.Forms.MenuItem("Enable", (o, e) => { 
				null; 
			});

			var exitContext = new System.Windows.Forms.MenuItem("Exit", (o, e) => {	
				Application.Current.Shutdown();	
			});

			var optionsContext = new System.Windows.Forms.MenuItem("Options", OptionsContext_Click);


			TrayIcon.ContextMenu.MenuItems.Add(enabledContext);
			TrayIcon.ContextMenu.MenuItems.Add(optionsContext);
			TrayIcon.ContextMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("-"));
			TrayIcon.ContextMenu.MenuItems.Add(exitContext);

			TrayIcon.DoubleClick += (o, e) => { ToggleThing(); };
			 */
		}
	}

}
