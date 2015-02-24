using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BindButton
{
	using Keys = System.Windows.Forms.Keys;
    public class BindButton : Button
    {
		private Object thelock = new Object();
		//Hold the keybinding
		private List<Keys> keyBind = new List<Keys>();
		public List<Keys> KeyBind {
			get { return keyBind; }
			set {
				lock (thelock) {
					keyBind = value;
				}
			}
		}
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

        static BindButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BindButton), new FrameworkPropertyMetadata(typeof(BindButton)));
			
        }

		public BindButton() {
			init();
		}

		private void init() {
			updateText();

			escapeDelay.Interval = 1000;
			escapeDelay.AutoReset = false;
			escapeDelay.Enabled = true;
			escapeDelay.Elapsed += escape;
			escapeDelay.Stop();

			Click += OnClick;
			KeyDown += OnKeyDown;
			KeyUp += OnKeyUp;
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
		protected void OnClick(Object o, EventArgs e) {
			tempKeys.Clear();

			controlString = "ESC to Cancel, Hold to Clear";
			Dispatcher.Invoke(new Action(() => {
				Focus();
				Content = controlString;
				this.UpdateLayout();
			}));
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

			Dispatcher.Invoke(new Action(() => {
				Content = controlString;
				this.UpdateLayout();
			}));
		}

		/// <summary>
		/// Triggered on key down.
		/// </summary>
		/// <param name="kevent"></param>
		protected void OnKeyDown(Object o, KeyEventArgs e) {
			if (!this.IsFocused) return;
			Keys fk = toFromKey(e.Key);

			if (!enterKeys) {
				//starts the clear timer.
				if (fk == Keys.Escape && !holdEscape) {
					holdEscape = true;
					escapeDelay.Start();
					//Otherwise starts the procces to add keys
				} else if (fk != Keys.Escape) {
					enterKeys = true;

					tempKeys.Add(fk);
					updateText(true);
				}
				//adds unique keys to the bind chain.
			} else if (enterKeys && !tempKeys.Contains(fk)) {
				tempKeys.Add(fk);
				updateText(true);
			}
		}

		private Keys toFromKey(Key k) {
			return (Keys)KeyInterop.VirtualKeyFromKey(k);
		}

		/// <summary>
		/// Triggered on key release
		/// </summary>
		/// <param name="kevent"></param>
		protected void OnKeyUp(Object o, KeyEventArgs e) {
			if (!this.IsFocused) return;
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
				this.IsEnabled = false;
				this.IsEnabled = true;
				updateText();
				Keyboard.ClearFocus();
				//escape();
			} else {
				if (toFromKey(e.Key) == Keys.Escape) {
					escapeDelay.Stop();
					enterKeys = false;
					holdEscape = false;
					this.IsEnabled = false;
					this.IsEnabled = true;
				}
				updateText();
			}
		}
    }
}
