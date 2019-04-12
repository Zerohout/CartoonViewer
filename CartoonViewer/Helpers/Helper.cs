namespace CartoonViewer.Helpers
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Threading;
	using System.Windows;
	using Caliburn.Micro;
	using Models.CartoonModels;
	using OpenQA.Selenium;
	using OpenQA.Selenium.Chrome;

	public static class Helper
	{
		public static readonly string AppPath = AppDomain.CurrentDomain.BaseDirectory;
		public const string MMBackgroundUri = "../../Resources/Images/HDMMBackground.png";
		public const string MMBackgroundOnExitUri = "../../Resources/Images/HDMMBackgroundOnExit.png";
		public const string WorkingDataPath = "WorkingData";
		public const string NewElementString = "**Добавить новый**";
		public const string FreehatWebSite = "http://freehat.cc";

		public static int CurrentSkipCount = 6;
		public static int DelayedSkipCount = 5;

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

		public static readonly TimeSpan ApproximateEpisodeDuration = new TimeSpan(0,21,10);
		public static WindowManager WinMan = new WindowManager();
		public static MessageHelper Msg = new MessageHelper();
		public static Stopwatch Timer = new Stopwatch();
		public static Visibility AdvancedSettingsVisibility = Visibility.Hidden;

		public static IWebDriver Browser;
		public static IntPtr HWND;
		public static HotkeysRegistrator HotReg;

		public readonly static BindableCollection<string> CartoonTypes = new BindableCollection<string>
		{
			"/disney",
			"/pixar",
			"/serii",
			"/multfilm",
		};

		
		public enum DialogState
		{
			YES_NO,
			OK,
			YES_NO_CANCEL,
			ERROR
		}

		/// <summary>
		/// Запуск браузера
		/// </summary>
		public static void StartBrowser()
		{
			Browser = new ChromeDriver();

			Thread.Sleep(1000);

			HWND = Msg.getWindowId(null,
			                       Process.GetProcessesByName("chrome")
			                              .Single(p => p.MainWindowTitle.Contains("Google"))
			                              .MainWindowTitle);

			Browser.Manage().Window.Maximize();
		}

		
	}
}
