// ReSharper disable UnusedMember.Local
namespace CartoonViewer.Helpers
{
	using System;
	using System.Runtime.InteropServices;
	using System.Text;
	using System.Threading;
	using static Helper;

	public class MessageHelper
	{
		public const int WM_KEYDOWN = 0x100;
		public const int WM_KEYUP = 0x101;
		public const int WM_CHAR = 0x102;
		public const int WM_SYSKEYDOWN = 0x104;
		public const int WM_SYSKEYUP = 0x105;
		public const int VK_LEFT = 0x25;
		public const int VK_RIGHT = 0x27;
		public const int VK_F = 0x46;
		public const int VK_ESCAPE = 0x1B;
		public const int VK_SPACE = 0x20;

		[DllImport("User32.dll")]
		private static extern int RegisterWindowMessage(string lpString);

		[DllImport("User32.dll", EntryPoint = "FindWindow")]
		public static extern IntPtr FindWindow(String lpClassName, String lpWindowName);

		//For use with WM_COPYDATA and COPYDATASTRUCT
		[DllImport("User32.dll", EntryPoint = "SendMessage")]
		public static extern int SendMessage(int hWnd, int Msg, int wParam, ref COPYDATASTRUCT lParam);

		//For use with WM_COPYDATA and COPYDATASTRUCT
		[DllImport("User32.dll", EntryPoint = "PostMessage")]
		public static extern int PostMessage(int hWnd, int Msg, int wParam, ref COPYDATASTRUCT lParam);

		[DllImport("User32.dll", EntryPoint = "SendMessage")]
		public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

		[DllImport("User32.dll", EntryPoint = "PostMessage")]
		public static extern int PostMessage(int hWnd, int Msg, int wParam, int lParam);

		[DllImport("User32.dll", EntryPoint = "SetForegroundWindow")]
		public static extern bool SetForegroundWindow(int hWnd);

		public const int WM_USER = 0x400;
		public const int WM_COPYDATA = 0x4A;

		//Used for WM_COPYDATA for string messages
		public struct COPYDATASTRUCT
		{
			public IntPtr dwData;
			public int cbData;
			[MarshalAs(UnmanagedType.LPStr)]
			public string lpData;
		}

		public bool bringAppToFront(int hWnd)
		{
			return SetForegroundWindow(hWnd);
		}

		public int sendWindowsStringMessage(int hWnd, int wParam, string msg)
		{
			int result = 0;

			if (hWnd > 0)
			{
				byte[] sarr = Encoding.Default.GetBytes(msg);
				int len = sarr.Length;
				COPYDATASTRUCT cds;
				cds.dwData = (IntPtr)100;
				cds.lpData = msg;
				cds.cbData = len + 1;
				result = SendMessage(hWnd, WM_COPYDATA, wParam, ref cds);
			}

			return result;
		}

		public int sendWindowsMessage(IntPtr hWnd, int Msg, int wParam, int lParam)
		{
			int result = 0;

			if (hWnd != IntPtr.Zero)
			{
				result = SendMessage(hWnd, Msg, wParam, lParam);
			}

			return result;
		}

		public IntPtr getWindowId(string className, string windowName)
		{
			return FindWindow(className, windowName);
		}

		public void PressKey(int key, int count = 1)
		{
			var result = 0;

			for (var i = 0; i < count; i++)
			{
				Thread.Sleep(450);
				result = sendWindowsMessage(HWND, WM_KEYDOWN, key, 0);
				Thread.Sleep(50);
				result = sendWindowsMessage(HWND, WM_KEYUP, key, 0);
			}
		}
	}
}
