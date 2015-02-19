using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;
using System.IO;


namespace WPF_Crosshair {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application {
		FileStream filestream = new FileStream("logfile.txt", FileMode.Create);
		StreamWriter streamwriter;

		public App() {
#if DEBUG
			RedirectOut();
#endif

			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(resolveDll);

		}
		void RedirectOut() {
			streamwriter = new StreamWriter(filestream);
			streamwriter.AutoFlush = true;

			Console.SetError(streamwriter);
			Console.SetOut(streamwriter);

			IntPtr handle = filestream.SafeFileHandle.DangerousGetHandle();
			Haax.SetStdHandle(-11, handle); // set stdout
			// Check status as needed
			Haax.SetStdHandle(-12, handle); // set stderr

			Console.Error.WriteLine("-----" + DateTime.Now + "-----");
		}
		 static Assembly resolveDll(object sender, ResolveEventArgs args) {
			 using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WPF_Crosshair.Resources.Newtonsoft.Json.dll")) {
				byte[] assemblyData = new byte[stream.Length];
				stream.Read(assemblyData, 0, assemblyData.Length);
				return Assembly.Load(assemblyData);
			}

			 /*using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WPF_Crosshair.Hardcodet.Wpf.TaskbarNotification.dll")) {
				 byte [] assemblyData = new byte [stream.Length];
				 stream.Read(assemblyData, 0, assemblyData.Length);
				 return Assembly.Load(assemblyData);
			 }*/
		}
		 

	}
}
