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

namespace WPF_Crossair {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		public MainWindow() {
			InitializeComponent();

			String path = System.IO.Path.Combine(Environment.CurrentDirectory, "ret.png");
			BitmapImage image = new BitmapImage();
			image.BeginInit();
			image.UriSource = new Uri(path);
			image.EndInit();

			this.WindowStyle = System.Windows.WindowStyle.None;
			this.AllowsTransparency = true;
			this.Background = Brushes.Transparent;
			this.Focusable = false;
			//this.Foreground = true;
			this.ShowInTaskbar = false;
			this.Topmost = true;
			//this.WindowStyle = System.Windows.WindowStyle.None;
			//this.Style


			//this.SizeToContent = true;
			this.Width = image.Width;
			this.Height = image.Height;

			Test.Source = image;
			Test.Width = image.Width;
			Test.Height = image.Height;

		}
	}
}
