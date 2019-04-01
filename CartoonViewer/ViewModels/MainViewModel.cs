namespace CartoonViewer.ViewModels
{
	using System;
	using System.Data.Entity;
	using System.Diagnostics;
	using System.Linq;
	using System.Runtime.InteropServices;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Windows;
	using Caliburn.Micro;
	using Database;
	using Helpers;
	using Models;
	using OpenQA.Selenium;
	using OpenQA.Selenium.Chrome;
	using static Helpers.Helper;


	public class MainViewModel : Screen
	{
		//MessageHelper msg = new MessageHelper();



		private BindableCollection<Cartoon> _cartoons = new BindableCollection<Cartoon>();

		public BindableCollection<Cartoon> Cartoons
		{
			get => _cartoons;
			set
			{
				_cartoons = value;
				NotifyOfPropertyChange(() => Cartoons);
			}
		}




		private WindowState _windowState;

		private string _episodeCount;

		public string EpisodeCount
		{
			get => _episodeCount;
			set
			{
				_episodeCount = value;
				NotifyOfPropertyChange(() => EpisodeCount);
			}
		}



		public int SeriesCount { get; set; }

		public MainViewModel()
		{
			LoadCartoons();
		}

		[DllImport("user32.dll")]
		private static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, int lParam);
		[DllImport("User32.dll")]
		private static extern int SetForegroundWindow(IntPtr hWnd);
		[DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		internal static extern IntPtr SetFocus(IntPtr hwnd);
		[DllImport("user32.dll")]
		public static extern int SendMessage(int hWnd, int Msg, int wParam, ulong lParam);
		[DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
		public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);


		private void LoadCartoons()
		{
			using (var ctx = new CVDbContext())
			{
				ctx.Cartoons.Load();
				Cartoons.AddRange(ctx.Cartoons.Local.OrderBy(c => c.CartoonId));
			}
		}

		public async void Start()
		{
			//KillChromedriver();

			WindowState = WindowState.Minimized;

			await Task.Run(LaunchBrowser);
		}

		private void KillChromedriver()
		{
			var proc = Process.GetProcessesByName("chromedriver");
			var proc1 = Process.GetProcesses();

			if (proc1.Any(p => p.MainWindowTitle.Contains("chrome")))
			{
				foreach (var pr in proc1.Where(p1 => p1.MainWindowTitle.Contains("chrome")))
				{
					pr.Kill();
				}
			}
			
			if (proc.Length > 0)
			{
				foreach (var p in proc)
				{
					p.Kill();

					while (true)
					{
						if (p.HasExited)
						{
							break;
						}

						Thread.Sleep(500);
					}
				}
			}
		}

		private Task LaunchBrowser()
		{
			StartBrowser();

			var autoEvent = new AutoResetEvent(false);

			var statusChecker = new StatusChecker(int.TryParse(EpisodeCount, out var epCount) ? epCount : 10);

			//var duration = new TimeSpan(0,21,30);

			//duration = new TimeSpan(0 , 0 , 10);

			//var stateTimer = new Timer(statusChecker.CheckStatus,
			//						   autoEvent, new TimeSpan(0,0,0), duration);

			//autoEvent.WaitOne();
			//autoEvent.Dispose();

			//var test = Process.GetProcessById(TabId).MainWindowHandle;

			Browser.Navigate().GoToUrl("https://sp.freehat.cc/episode/rand.php");

			//var t = FindWindow(null, Process.GetProcessById(TabId).MainWindowTitle);
			
			//var hWnd = msg.getWindowId(null, Process.GetProcessById(TabId).MainWindowTitle);

			var el = Browser.FindElement(By.CssSelector("pjsdiv:nth-child(8) > pjsdiv > pjsdiv"));
			el.Click();
			Thread.Sleep(100);
			el.Click();

			Msg.PressKey(VK_LEFT);
			Msg.PressKey(VK_F);
			Msg.PressKey(VK_RIGHT,4);
			

			return Task.CompletedTask;
		}

		private void StartBrowser()
		{
			Browser = new ChromeDriver();

			Thread.Sleep(1000);

			//TabId = Process.GetProcessesByName("chrome").Single(p => p.MainWindowTitle.Contains("Google")).Id;
			HWND = Msg.getWindowId(null, 
				Process.GetProcessesByName("chrome")
				       .Single(p => p.MainWindowTitle.Contains("Google"))
				       .MainWindowTitle);

			Browser.Manage().Window.Maximize();
		}

		public void Closing()
		{
			Browser?.Quit();

		}

		/// <summary>
		/// Состояние окна
		/// </summary>
		public WindowState WindowState
		{
			get => _windowState;
			set
			{
				_windowState = value;
				NotifyOfPropertyChange(() => WindowState);
			}
		}
	}
}
