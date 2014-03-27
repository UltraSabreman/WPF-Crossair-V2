﻿using System;
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

		private System.Windows.Forms.NotifyIcon TrayIcon = null;
		private System.Windows.Forms.MenuItem enabledContext = null;

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

			TrayIcon = new System.Windows.Forms.NotifyIcon();
			TrayIcon.Icon = WPF_Crosshair.Properties.Resources.on;
			TrayIcon.Visible = true;
			TrayIcon.ContextMenu = new System.Windows.Forms.ContextMenu();

			enabledContext = new System.Windows.Forms.MenuItem("Enable", (object s, EventArgs a) => { ToggleThing(); });

			System.Windows.Forms.MenuItem optionsContext = new System.Windows.Forms.MenuItem("Options", OptionsContext_Click);

			System.Windows.Forms.MenuItem exitContext = new System.Windows.Forms.MenuItem("Exit", (object s, EventArgs a) => {
				System.Windows.Application.Current.Shutdown();
			});

			TrayIcon.ContextMenu.MenuItems.Add(enabledContext);
			TrayIcon.ContextMenu.MenuItems.Add(optionsContext);
			TrayIcon.ContextMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("-"));
			TrayIcon.ContextMenu.MenuItems.Add(exitContext);

			TrayIcon.DoubleClick += delegate(object sender, EventArgs args) { ToggleThing(); };


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

			try {
				//if (configs.CrosshairPath != null && File.Exists(configs.CrosshairPath))
					testWindow = new GameWindow(configs.TargetWindowTitle, configs.CrosshairPath);
					testWindow.isEnabled = true;
					ChangeIcon();
				//else
				//	testWindow = new GameWindow(null, null);

			} catch (FileNotFoundException) {
				MessageBoxResult res = MessageBox.Show("Can't find a crosshair.\nWould you like to set it's location?", "No Crosshair", System.Windows.MessageBoxButton.OKCancel, MessageBoxImage.Error);
				if (res == MessageBoxResult.Cancel)
					Application.Current.Shutdown();
				else
					OptionsContext_Click(null, null);
			}

		}

		public void OnClose(object o, EventArgs e) {
			TrayIcon.Visible = false;
			TrayIcon.Dispose();
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
				ToggleThing();
			}
		}

		private void ChangeIcon() {
			if (TrayIcon == null) return;
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
			}));
		}

		private void OptionsContext_Click(object sender, EventArgs e) {
			var test = new Options(configs);
			test.OnAccept += OptionsAccept;
			test.Show();
		}

		private void OptionsAccept(Config src, GameWindow newWind) {
			hotKeys.UnregisterHotKey(configs.ShowHideCrosshair);
			configs = src;
			hotKeys.RegisterHotKey(configs.ShowHideCrosshair);

			bool en = testWindow != null ? testWindow.isEnabled : true;
			testWindow = newWind;

			try {
				testWindow.LoadImage(configs.CrosshairPath);			
			} catch (FileLoadException) {
				System.Windows.MessageBox.Show("Crosshair failed to load, is it corrupted?", "Failed to load", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);			
			} catch (FileNotFoundException) {
				System.Windows.MessageBox.Show("Crosshair file not found.", "File not found", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);			
			}

			testWindow.isEnabled = en;

			ChangeIcon();
			DataReader.Serialize(configs);
		}

		private void ToggleThing() {
			enabled = !enabled;
			enabledContext.Checked = enabled;
			testWindow.isEnabled = enabled;
			ChangeIcon();
		}
	}
}
