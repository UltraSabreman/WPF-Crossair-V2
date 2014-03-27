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
	}
}
