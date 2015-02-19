using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;

//using System.Diagnostics;

/// <summary>
/// Hotkey, holds the actual key, and all the modifiers.
/// </summary>
public class HotKey {
    /// <summary>
    /// Contians the hotkey and all modifiers (the first entery is the hotkey)
    /// </summary>
	public List<Keys> KeyList = null;
    public bool isDown = false;

	public HotKey() {
		KeyList = new List<Keys>();
	}

	public HotKey(List<Keys> mod) {
        KeyList = mod;
    }

    public HotKey(params object [] hotkeys) {
		KeyList = new List<Keys>();
        isDown = false;
		foreach (Keys k in hotkeys) 
            KeyList.Add(k);
        
    }

	public override bool Equals(Object o) {
		if (System.Object.ReferenceEquals(this, o))
			return true;

		HotKey k2 = o as HotKey;
		if (((object)this == null) || ((object)k2 == null) || this.KeyList == null || k2.KeyList == null || this.KeyList.Count != k2.KeyList.Count) {
			return false;
		}


		for (int i = 0; i < this.KeyList.Count; i++)
			if (this.KeyList [i] != k2.KeyList [i])
				return false;

		return true;
	}

    public static bool operator ==(HotKey k1, HotKey k2) {
		if (((object)k1 == null)) return false;
		return k1.Equals(k2);
    }

	public static bool operator !=(HotKey k1, HotKey k2) {
        return !(k1 == k2);
    }

	public Keys First() {
        return KeyList.First();
    }
}

public sealed class AsyncGlobalShortcuts : IDisposable {
    [DllImport("user32.dll")]
	private static extern short GetAsyncKeyState(Keys vKey);

	private List<HotKey> keys = new List<HotKey>();
	private Object locker = new Object();
	private AutoResetEvent autoEvent;
	private System.Threading.Timer keyTimer;

    public event EventHandler<KeyPressedEventArgs> KeyDown;
    public event EventHandler<KeyPressedEventArgs> KeyUp;
    public event EventHandler<KeyPressedEventArgs> KeyPressed;


	public AsyncGlobalShortcuts() {
		autoEvent = new AutoResetEvent(false);
		keyTimer = new System.Threading.Timer(CheckForKeys, null, 0, 10);
    }

	private void CheckForKeys(object call) {
		lock (locker) {
			try {
				foreach (HotKey k in keys) {
					if (k == null) continue;
					if (k.KeyList == null) continue;
					if (k.KeyList.Count == 0) continue;

					bool allPressed = true;
					foreach (Keys key in k.KeyList) {
						if (!isKeyPressed(key)) {
							allPressed = false;
							break;
						}
					}

					if (allPressed && !k.isDown) {
						k.isDown = true;

						if (KeyDown != null)
							KeyDown(this, new KeyPressedEventArgs(k));
					}
					if (!allPressed && k.isDown) {

						k.isDown = false;

						if (KeyUp != null)
							KeyUp(this, new KeyPressedEventArgs(k));
						if (KeyPressed != null)
							KeyPressed(this, new KeyPressedEventArgs(k));
						//for some odd reason, the above statement triggers the InvalidOperationException sometimes.
						//Not sure what the cause is, but keeping it as the last thing seems like a good idea for now
						//(aside from adding try{} blocks for each call)
					}
				}
			} catch (System.InvalidOperationException) { }
		}
	}

	static private bool isKeyPressed(Keys code) {
        short result = GetAsyncKeyState(code);

        //result returns two things. The first bin digit says if it's currently pressed.
        //the last bin digit indicates if it was pressed before
        //Because this is running constantly on a thread, we check both for the fastest and
        //responce time (and no potentual misses).
        return (((result >> 0xF) & 1) == 1 || (((result << 1) >> 1) & 1) == 1);
    }

    /// <summary>
    /// Registers a hot key in the system with a variable amount of modifiers.
    /// </summary>
    /// <param name="hotkeys">Any and all modifiers you wish to add</param>
    public HotKey RegisterHotKey(params object [] hotkeys) {
		lock (locker) {
			List<Keys> temp = new List<Keys>();
			foreach (Keys k in hotkeys)
				temp.Add(k);

			HotKey t = new HotKey(temp);
			keys.Add(t);
			return t;
		}
    }

    /// <summary>
    /// Registers a hot key in the system.
    /// </summary>
    /// <param name="hotkey">A HotKey Struct</param>
    public void RegisterHotKey(HotKey hotkey) {
		lock (locker) {
			keys.Add(hotkey);
		}
    }

    /// <summary>
    /// Unregisters a hot key in the system.
    /// </summary>
    /// <param name="hotkey">A HotKey Struct</param>
    public void UnregisterHotKey(HotKey hotkey) {
		lock (locker) {
			if (keys.Contains(hotkey)) {
				keys.Remove(hotkey);
			} else
				throw new InvalidOperationException("Couldn’t unregister the hot key.");
		}
    }

	public void UnregisterAll() {
		lock (locker) {
			keys.Clear();
		}
	}

    #region IDisposable Members

    public void Dispose() {
		keyTimer.Dispose();
        keys.Clear();
    }

    #endregion
}

/// <summary>
/// Event Args for the event that is fired after the hot key has been pressed.
/// </summary>
public class KeyPressedEventArgs : EventArgs {
    private HotKey _key;

    internal KeyPressedEventArgs(HotKey key) {
        _key = key;
    }

    public HotKey Key {
        get { return _key; }
    }
}
