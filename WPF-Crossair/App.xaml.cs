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
using System.Globalization;


namespace WPF_Crosshair {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application {
		FileStream filestream = new FileStream("logfile.txt", FileMode.Create);
		StreamWriter wr;

		public App() {

			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(resolveJson);
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(ErrorHandler);

		}


		public void ErrorHandler(Object o, UnhandledExceptionEventArgs args) {
			wr = new StreamWriter(filestream);
			DumpMachineInfo();

			PrintLine("==== Unhandled Exception Occured!");
			Exception curEx = (Exception)args.ExceptionObject;

			PrintLine("= Type: " + args.ExceptionObject.GetType().ToString());
			PrintLine("= Throwie: " + curEx.TargetSite);
			PrintLine("= Message: " + curEx.Message);

			StringBuilder Tabs = new StringBuilder();
			while (curEx.InnerException != null) {
				Tabs.Append("\t");
				curEx = (Exception)curEx.InnerException;
				PrintLine("=--- Inner");
				PrintLine(String.Format("= {0}Type: " + curEx.GetType().ToString(), Tabs.ToString()));
				PrintLine(String.Format("= {0}Throwie: " + curEx.TargetSite, Tabs.ToString()));
				PrintLine(String.Format("= {0}Message: " + curEx.Message, Tabs.ToString()));
			}
			PrintLine("=== Stack Trace");
			PrintLine("\n" + ((Exception)args.ExceptionObject).StackTrace);
			wr.Close();
			Environment.Exit(1);
		}

		void PrintLine(params object[] args) {
			StringBuilder line = new StringBuilder();

			foreach (Object o in args)
				line.Append(o.ToString());

			line.Append('\n');
			wr.WriteLine(line.ToString());
		}

		void DumpMachineInfo() {
			PrintLine("==== Date: " + DateTime.Now.ToString("MM-dd-yyyy hh:m:ss-fff tt"));
			//PrintLine("==== Name: " + Environment.MachineName);
			PrintLine("==== Os: " + Environment.OSVersion);
			PrintLine("==== Ver: " + Environment.Version);
			PrintLine("==== Dir: " + Environment.CurrentDirectory);
		}

		//Embedding DLLs into WPF project via:
		//http://www.digitallycreated.net/Blog/61/combining-multiple-assemblies-into-a-single-exe-for-a-wpf-application
		static Assembly resolveJson(object sender, ResolveEventArgs args) {
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			AssemblyName assemblyName = new AssemblyName(args.Name);
 
			string path = assemblyName.Name + ".dll";
			if (assemblyName.CultureInfo.Equals(CultureInfo.InvariantCulture) == false)
			{
				path = String.Format(@"{0}\{1}", assemblyName.CultureInfo, path);
			}
 
			using (Stream stream = executingAssembly.GetManifestResourceStream(path))
			{
				if (stream == null)
					return null;

				byte[] assemblyRawBytes = new byte[stream.Length];
				stream.Read(assemblyRawBytes, 0, assemblyRawBytes.Length);
				return Assembly.Load(assemblyRawBytes);
			}

		}
		 

	}
}
