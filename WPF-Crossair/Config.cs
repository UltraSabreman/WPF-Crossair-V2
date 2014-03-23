using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WPF_Crosshair {
	public class Config {
		public bool ExitWithProgram = false;
		public String CrosshairPath = "ret.png";
		public String TargetWindowTitle = "Calculator";
		public HotKey ShowHideCrosshair = null;
		public bool Initilized = false;

		public void ResetHotKeys() {
			ShowHideCrosshair = new HotKey(Keys.F4);
		}

		public void copy(Config src) {
			ExitWithProgram = src.ExitWithProgram;
			CrosshairPath = src.CrosshairPath;
			TargetWindowTitle = src.TargetWindowTitle;
			ShowHideCrosshair = src.ShowHideCrosshair;
			Initilized = src.Initilized;

		}
	}
}
