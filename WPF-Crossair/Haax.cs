using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;

namespace WPF_Crosshair {
	static class Haax {
		[StructLayout(LayoutKind.Sequential)]
		public struct RECT {
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll", SetLastError = true)]

		public static extern bool SetForegroundWindow(IntPtr hWnd);

		const int WS_EX_TRANSPARENT = 0x00000020;
		const int GWL_EXSTYLE = (-20);

		[DllImport("user32.dll")]
		static extern int GetWindowLong(IntPtr hwnd, int index);

		[DllImport("user32.dll")]
		static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

		public static void SetWindowExTransparent(IntPtr hwnd) {
			var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
			SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
		}
	}
}
