using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace WPF_Crosshair {
	using ConfigMap = Dictionary<String, Object>;
	using System.Text.RegularExpressions;
	
	public static class Configs {
		//This is a super convaluted way to make sure we always put the config file in the same dir as this assembly.
		private static String path = new Uri(new Regex(@"(/[^/]*$)").Replace(System.Reflection.Assembly.GetExecutingAssembly().CodeBase, "/configs.json")).LocalPath;
		
		//Here you can define defaults for all values. This is a string-object dictionary so anything can be used for a value.
		private static ConfigMap Settings = new ConfigMap() {
			{ "ExitWithProgram", false },
			{ "ImagePath", "ret.png" },
			{ "TargetTitle", "Calc.*" },
			{ "HotKey",  new HotKey(Keys.F4)}
		};

		/// <summary>
		/// whether the configs are saved automaticly each time they are changed.
		/// </summary>
		private static bool AutoSave = false;
		
		/// <summary>
		/// Initilizes the configs by trying to read from the settings file.
		/// If something goes wrong, defaults are loaded and the file is overwritten.
		/// </summary>
		static Configs() {
			try {
				ReadConfigs();
			} catch (Exception) {
				//Trys to write the default settings.
				try { 
					WriteConfigs(); 
				} catch (Exception) { }
			} finally {
				Initilize();
			}
		}


		public static T convertTo<T>(Object o) {
			try {
				Newtonsoft.Json.Linq.JObject jo = (Newtonsoft.Json.Linq.JObject)o;
				return (T)jo.ToObject(typeof(T));
			} catch (Exception) {
				return (T)o;
			}
		}

		//Reads the config file
		private static void ReadConfigs() {
			using (StreamReader rd = new StreamReader(path)) {
				//Replace with your preffered method of de-serialization
				ConfigMap temp = new JsonSerializer().Deserialize(rd, typeof(ConfigMap)) as ConfigMap;
				if (temp != null)
					Settings = temp;
				else
					throw new IOException();
			}
		}
		
		//writes the config file.
		private static void WriteConfigs() {
			using (StreamWriter wr = new StreamWriter(path))
				//Replace with your preffered method of serialization
				JsonSerializer.Create(new JsonSerializerSettings() { Formatting = Newtonsoft.Json.Formatting.Indented }).Serialize(wr, Settings);
		}
		
		/// <summary>
		/// This is used to initilize any settings after thing have loaded.
		/// This is where things like databse quereys and other file reads should happen.
		/// </summary>
		private static void Initilize() {
		
		}
		
		/// <summary>
		/// This class acts as a gateway to our settings. This should be the only thing (And it's instance)
		/// that's exposed in the config class. All interaction with configs should happen only through it.
		/// </summary>
		public sealed class ConfigurationPropertyIndexer {
			/// <summary>
			/// Access a spesifed config to assing to it or read it.
			/// On assingment, will create the config if it doesn't exhist and if
			/// autodave is set to true, it will write to the file.
			/// </summary>
			/// <param name="name">The Key</param>
			/// <returns>The config object</returns>
			public object this[string name] {
				get {
					return (Configs.Settings.ContainsKey(name) ? Configs.Settings[name] : null);
				}
				set {
					Configs.Settings[name] = value;
					if (Configs.AutoSave)
						Save();
				}
			}
			
			/// <summary>
			/// Remove the spesifed config.
			/// </summary>
			/// <param name="name">The Key</param>
			/// <returns>False: the key did not exhist.
			/// True: The key was removed</returns>
			public bool RemoveConfig(String name) {
				if (Configs.Settings.ContainsKey(name)) {
					Configs.Settings.Remove(name);
					if (Configs.AutoSave)
						Save();
					return true;
				}
				return false;
			}
			
			/// <summary>
			/// Saves the configs to file.
			/// </summary>
			public void Save() {
				Configs.WriteConfigs();
			}
		}
		
		/// <summary>
		/// Gives read/write access to the configurations.
		/// This should be the only public member in the config class.
		/// </summary>
		public static ConfigurationPropertyIndexer Properties = new ConfigurationPropertyIndexer();
	}
}
