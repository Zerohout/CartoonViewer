using System;
using System.Collections.Generic;
using System.Linq;

namespace CartoonViewer.Helpers
{
	using System.Runtime.InteropServices;
	using System.Windows;
	using System.Windows.Input;
	using System.Windows.Interop;

	public class HotkeysRegistrator
	{
		[DllImport("User32.dll")]
		private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
		[DllImport("User32.dll")]
		private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
		[DllImport("kernel32.dll")]
		private static extern short GlobalAddAtom(string name);
		[DllImport("kernel32.dll")]
		private static extern short GlobalDeleteAtom(short nAtom);

		private Dictionary<short, Action> _globalActions = new Dictionary<short, Action>();
		private readonly IntPtr _windowHandle;


		public HotkeysRegistrator(Window window)
		{
			_windowHandle = new WindowInteropHelper(window).Handle;

			var source = HwndSource.FromHwnd(_windowHandle);
			source?.AddHook(WndProc);
		}

		public HotkeysRegistrator(IntPtr hWnd)
		{
			var source = HwndSource.FromHwnd(hWnd);
			source?.AddHook(WndProc);
		}

		private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
		{
			if(msg == 0x0312)
			{
				var atom = short.Parse(wparam.ToString());
				if(_globalActions.ContainsKey(atom))
				{
					_globalActions[atom]();
				}
			}
			return IntPtr.Zero;
		}

		public bool RegisterGlobalHotkey(Action action, System.Windows.Forms.Keys commonKey, params ModifierKeys[] keys)
		{
			// ReSharper disable once SuspiciousTypeConversion.Global
			var mod = keys.Cast<uint>().Aggregate((current, modKey) => current | modKey);
			var atom = GlobalAddAtom("CartoonViewer" + (_globalActions.Count + 1));
			var status = RegisterHotKey(_windowHandle, atom, mod, (uint)commonKey);

			if(status)
			{
				_globalActions.Add(atom, action);
			}
			return status;
		}

		public void UnregisterHotkeys()
		{
			foreach(var atom in _globalActions.Keys)
			{
				UnregisterHotKey(_windowHandle, atom);
				GlobalDeleteAtom(atom);
			}
		}
	}
}
