using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;


namespace WPF_Crosshair {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application {
		public App() {
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(resolveDll);
		}
		 static Assembly resolveDll(object sender, ResolveEventArgs args) {
			 using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WPF_Crosshair.Newtonsoft.Json.dll")) {
				byte[] assemblyData = new byte[stream.Length];
				stream.Read(assemblyData, 0, assemblyData.Length);
				return Assembly.Load(assemblyData);
			}

			 using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WPF_Crosshair.Hardcodet.Wpf.TaskbarNotification.dll")) {
				 byte [] assemblyData = new byte [stream.Length];
				 stream.Read(assemblyData, 0, assemblyData.Length);
				 return Assembly.Load(assemblyData);
			 }
		}
		 

	}
}
