using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WPF_Crosshair {
	class Config {
		public bool ExitWithProgram = false;
		public String CrosshairPath = "ret.png";
		public String TargetWindowTitle = "Calculator";
		public HotKey ShowHideReticule = null;
		public bool Initilized = false;

		public void ResetHotKeys() {
			ShowHideReticule = new HotKey(Keys.F4);
		}
	}
}
