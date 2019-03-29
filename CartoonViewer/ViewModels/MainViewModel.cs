namespace CartoonViewer.ViewModels
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Timers;
	using System.Windows;
	using Caliburn.Micro;
	using OpenQA.Selenium;
	using OpenQA.Selenium.Chrome;
	using WindowsInput;
	using WindowsInput.Native;
	using Timer = System.Timers.Timer;

	public class MainViewModel : Screen
	{
		private IWebDriver Browser;

		private WindowState _windowState;

		public bool FirstStart = true;

		public MainViewModel()
		{
			
		}

		public async void Start()
		{
			WindowState = WindowState.Minimized;

			await LaunchBrowser();
		}

		private Task LaunchBrowser()
		{
			var options = new ChromeOptions();

			options.AddExtension($"{AppDomain.CurrentDomain.BaseDirectory}\\3.42.0_0.crx");

			Browser = new ChromeDriver(options);

			Thread.Sleep(7000);

			Browser.Navigate().GoToUrl("http://www.yandex.ru");

			Browser.Manage().Window.Maximize();

			var timer = new Timer
			{
				Interval = 1290000,
				AutoReset = true,
				Enabled = true
			};

			timer.Elapsed += TimerAction;

			timer.Start();
			TimerAction(timer,null);
			
			return Task.CompletedTask;
		}

		private async void TimerAction(object sender, ElapsedEventArgs e)
		{
			await PlayCartoon();
		}
		
		private Task PlayCartoon()
		{
			Browser.Navigate().GoToUrl("https://sp.freehat.cc/episode/rand.php");

			var sim = new InputSimulator();

			var el = Browser.FindElement(By.Id("videoplayer"));
			el.Click();

			Thread.Sleep(2000);
			if (FirstStart)
			{
				sim.Keyboard.KeyPress(VirtualKeyCode.VK_F);
				FirstStart = false;
			}

			Thread.Sleep(2000);

			for (var i = 0; i < 5; i++)
			{
				sim.Keyboard.KeyPress(VirtualKeyCode.RIGHT);
				Thread.Sleep(500);
			}
			
			return Task.CompletedTask;
		}

		public void Closing()
		{
			Browser.Quit();
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
