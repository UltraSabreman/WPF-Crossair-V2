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
				} catch (Exception) {
					//well this failed terribly. At this point
					//there's not much we can do, so we throw a io error 
					//and hope for the best.
					throw new Exception("Failed to read AND write to configs. Check premissions");
				}
			} finally {
				Initilize();
			}
		}

		public static Object get(String key) {
			return (Configs.Settings.ContainsKey(key) ? Configs.Settings[key] : null);

		}

		public static void set(String key, Object Value) {
			Configs.Settings[key] = Value;
			if (Configs.AutoSave)
				WriteConfigs();
		}

		public static T getAs<T>(String key) {
			Object o = get(key);
			try {
				Newtonsoft.Json.Linq.JObject jo = (Newtonsoft.Json.Linq.JObject)o;
				return (T)jo.ToObject(typeof(T));
			} catch (Exception) {
				return (T)o;
			}
		}

		public static void setAs<T>(String key, T Value) {
			Configs.Settings[key] = Value;
			if (Configs.AutoSave)
				WriteConfigs();
		}

		public static bool remove(String key) {
			if (Configs.Settings.ContainsKey(key)) {
				Configs.Settings.Remove(key);
				if (Configs.AutoSave)
					WriteConfigs();
				return true;
			}
			return false;
		}


		//Reads the config file
		public static void ReadConfigs() {
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
		public static void WriteConfigs() {
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
		
	}
}
