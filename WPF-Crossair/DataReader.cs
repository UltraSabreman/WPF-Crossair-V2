using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;


namespace WPF_Crosshair {
	static class DataReader {
		private static String filePath = "data.json";

		public static void Deserialize(String path, Config toFill) {
			if (File.Exists(path)) {
				try {
					using (StreamReader fs = File.OpenText(path)) {
						JsonConvert.PopulateObject(fs.ReadToEnd(), toFill); //#yolo
					}
				} catch (Newtonsoft.Json.JsonSerializationException) {
					using (StreamWriter fs = new StreamWriter(path)) {
						fs.Write("{}");
					}
				}
			} else
				throw new FileNotFoundException();
		}
		public static void Deserialize(Config toFill) {
			Deserialize(filePath, toFill);
		}

		public static void Serialize(String path, Config toFill) {
			if (!File.Exists(path)) File.CreateText(path);

			using (StreamWriter fs = new StreamWriter(path)) {
				fs.Write(JsonConvert.SerializeObject(toFill, Formatting.Indented));
			}
		}

		public static void Serialize(Config toFill) {
			Serialize(filePath, toFill);
		}
	}
}
