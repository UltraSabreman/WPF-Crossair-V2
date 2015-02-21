using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WPF_Crosshair {
	class bindButton : System.Windows.Forms.Button {
		//Hold the keybinding
		public List<Keys> keyBind = new List<Keys>();
		//Hold the temp key binding as the user enters it.
		private List<Keys> tempKeys = new List<Keys>();
		//the text that's being displayed.
		public string controlString = "Bind";

		//triggeres when a new bind is made (including clearing it)
		public delegate void KeysDone(List<Keys> keys);
		public event KeysDone OnNewBind;

		private bool enterKeys = false;
		private bool holdEscape = false;

		private Timer escapeDelay = new Timer();

		public bindButton() {
			init();			
		}

		public bindButton(List<Keys> inkeys) {
			if (inkeys != null)
				keyBind = inkeys;

			init();
		}

		/// <summary>
		/// Initilizes the control by creating the reset delay, and setting the text.
		/// </summary>
		private void init() {
			updateText();

			escapeDelay.Interval = 1000;
			escapeDelay.Tick += new EventHandler(escape);
		}

		/// <summary>
		/// Triggered when escape is held (clears the bind).
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void escape(object source, EventArgs e) {
			keyBind.Clear();
			tempKeys.Clear();

			updateText();

			if (OnNewBind != null)
				OnNewBind(null);
		}

		/// <summary>
		/// Resets the control to allow new input
		/// </summary>
		/// <param name="e"></param>
		protected override void OnClick(EventArgs e) {
			tempKeys.Clear();

			controlString = "ESC to Cancel, Hold to Clear";
			this.Refresh();
			base.OnClick(e);
		}

		/// <summary>
		/// Updates the redered text with the bind, or approrpiate hint strings.
		/// </summary>
		/// <param name="tempOrNot">Whether to display the temp bind list or the actual bind</param>
		public void updateText(bool tempOrNot = false) {
			List<Keys> temp = (tempOrNot ? tempKeys : keyBind);

			controlString = "";
			if (temp == null || temp.Count == 0)
				controlString = "No Bind";
			else {
				int ind = 0;
				foreach (Keys k in temp) {
					controlString += k.ToString();
					if (ind < temp.Count - 1)
						controlString += " + ";
					ind++;
				}
			}

			this.Refresh();
		}

		/// <summary>
		/// Triggered on key down.
		/// </summary>
		/// <param name="kevent"></param>
		protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs kevent) {
			if (!this.Focused) return;

			if (!enterKeys) {
				//starts the clear timer.
				if (kevent.KeyCode == Keys.Escape && !holdEscape) {
					holdEscape = true;
					escapeDelay.Start();
				//Otherwise starts the procces to add keys
				} else if (kevent.KeyCode != Keys.Escape) {
					enterKeys = true;

					tempKeys.Add(kevent.KeyCode);
					updateText(true);
				}
			//adds unique keys to the bind chain.
			} else if (enterKeys && !tempKeys.Contains(kevent.KeyCode)) {
				tempKeys.Add(kevent.KeyCode);
				updateText(true);
			}

			base.OnKeyDown(kevent);
		}


		/// <summary>
		/// Triggered on key release
		/// </summary>
		/// <param name="kevent"></param>
		protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs kevent) {
			if (!this.Focused) return;
			if (enterKeys) {
				//clears the old bind and adds the new keys.
				keyBind.Clear();
				foreach (Keys k in tempKeys)
					keyBind.Add(k);

				//sends out the bind event
				if (OnNewBind != null)
					OnNewBind(keyBind);

				holdEscape = false;
				enterKeys = false;
				//resets the controll so we dont keep input focus.
				this.Enabled = false;
				this.Enabled = true;
				updateText();

			} else {
				if (kevent.KeyCode == Keys.Escape) {
					escapeDelay.Stop();
					enterKeys = false;
					holdEscape = false;
					this.Enabled = false;
					this.Enabled = true;
				}
				updateText();
			}

			base.OnKeyUp(kevent);
		}

		/// <summary>
		/// Draws the actual "button".
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			Pen p = new Pen(Color.Black, 1f);
			Brush b = new SolidBrush(Color.White);
			Brush fontBrush = new SolidBrush(Color.Black);
			Font f = new Font("Arial", 8, FontStyle.Regular);
			SizeF txtSize = TextRenderer.MeasureText(controlString, f);

			e.Graphics.FillRectangle(b, new Rectangle(new Point(0, 0), this.Size));
			e.Graphics.DrawRectangle(p, new Rectangle(new Point(0, 0), this.Size - new Size(1, 1)));
			e.Graphics.DrawString(controlString, f, fontBrush, this.Size.Width / 2 - txtSize.Width / 2, this.Size.Height / 2 - txtSize.Height / 2);

			f.Dispose();
			b.Dispose();
			p.Dispose();
		}
	}
}
