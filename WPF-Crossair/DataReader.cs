using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;


namespace WPF_Crosshair {
	static class DataReader {
		private static String filePath = "data.json";

		/// <summary>
		/// De-serializes the configs
		/// </summary>
		/// <param name="path">Filepath to the config location</param>
		/// <param name="toFill">The config object that will be populated</param>
		/// <exception cref="FileNotFoundException"/>
		/// <exception cref="UnauthorizedAccessException"/>
		/// 
		public static void Deserialize(String path, Config toFill) {
			if (File.Exists(path)) {
				try {
					using (TextReader reader = File.OpenText(path)) {
						String contents = reader.ReadToEnd();
						if (contents == "{}") return;

						JsonConvert.PopulateObject(contents, toFill); //#yolo
					}
					toFill.Initilized = true;
				} catch (Newtonsoft.Json.JsonSerializationException e) {
					throw new FileNotFoundException();
				}
			} else
				throw new FileNotFoundException();
		}
		/// <summary>
		/// De-serializes the configs, ussing the default path "data.json"
		/// </summary>
		/// <param name="toFill">The config object that will be populated</param>
		/// <exception cref="FileNotFoundException"/>
		/// <exception cref="UnauthorizedAccessException"/>
		public static void Deserialize(Config toFill) {
			Deserialize(filePath, toFill);
		}

		/// <summary>
		/// Serializes the configs
		/// </summary>
		/// <param name="path">Filepath to the config location</param>
		/// <param name="toFill">The config object that will be written</param>
		/// <exception cref="UnauthorizedAccessException"/>
		public static void Serialize(String path, Config toFill) {
			using (StreamWriter writer = new StreamWriter(path)) {
				writer.Write(JsonConvert.SerializeObject(toFill, Formatting.Indented));
			}
		}


		/// <summary>
		/// Serializes the configs, using the default location "data.json"
		/// </summary>
		/// <param name="toFill">The config object that will be written</param>
		/// <exception cref="UnauthorizedAccessException"/>
		public static void Serialize(Config toFill) {
			Serialize(filePath, toFill);
		}
	}
}
