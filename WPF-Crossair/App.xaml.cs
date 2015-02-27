using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;
using System.IO;
using System.Text;


namespace WPF_Crosshair {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application {
		FileStream filestream = new FileStream("logfile.txt", FileMode.Create);
		StreamWriter wr;

		public App() {

			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(resolveJson);
			//AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(resolveButton);
#if !DEBUG
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(ErrorHandler);
#endif

		}


		public void ErrorHandler(Object o, UnhandledExceptionEventArgs args) {
			wr = new StreamWriter(filestream);
			DumpMachineInfo();

			PrintLine("++++Unhandled Exception Occured!++++");
			Exception curEx = (Exception)args.ExceptionObject;

			PrintLine("+ Type: " + args.ExceptionObject.GetType().ToString());
			PrintLine("+ Throwie: " + curEx.TargetSite);
			PrintLine("+ Message: " + curEx.Message);

			StringBuilder Tabs = new StringBuilder();
			while (curEx.InnerException != null) {
				Tabs.Append("\t");
				curEx = (Exception)curEx.InnerException;
				PrintLine("+---Inner---+");
				PrintLine(String.Format("+ {0}Type: " + curEx.GetType().ToString(), Tabs.ToString()));
				PrintLine(String.Format("+ {0}Throwie: " + curEx.TargetSite, Tabs.ToString()));
				PrintLine(String.Format("+ {0}Message: " + curEx.Message, Tabs.ToString()));
			}
			PrintLine("+++++++++++++Stack Trace++++++++++++");
			PrintLine("\n" + ((Exception)args.ExceptionObject).StackTrace);
			PrintLine("++++++++++++++++++++++++++++++++++++");
			wr.Close();
			Environment.Exit(1);
		}

		void PrintLine(params object[] args) {
			StringBuilder line = new StringBuilder();

			foreach (Object o in args)
				line.Append(o.ToString());

			wr.WriteLine(line.ToString());
		}

		void DumpMachineInfo() {
			StringBuilder line = new StringBuilder();
			line.Append(DateTime.Now.ToString("==== Date: MM-dd-yyyy hh:m:ss-fff tt===="));
			line.Append("==== Name: " + Environment.MachineName + "====");
			line.Append("==== Os: " + Environment.OSVersion + "====");
			line.Append("==== Ver: " + Environment.Version + "====");
			line.Append("==== Dir: " + Environment.CurrentDirectory + "====");

			PrintLine(line.ToString());
		}


		static Assembly resolveJson(object sender, ResolveEventArgs args) {
			 using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WPF_Crosshair.Resources.Newtonsoft.Json.dll")) {
				byte[] assemblyData = new byte[stream.Length];
				stream.Read(assemblyData, 0, assemblyData.Length);
				return Assembly.Load(assemblyData);
			}
		}

		static Assembly resolveButton(object sender, ResolveEventArgs args) {
			using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BindButton.Bin.Debug.BindButton.dll")) {
				byte[] assemblyData = new byte[stream.Length];
				stream.Read(assemblyData, 0, assemblyData.Length);
				return Assembly.Load(assemblyData);
			}
		}
		 

	}
}
