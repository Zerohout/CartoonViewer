namespace CartoonViewer.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.Data.Entity;
	using System.Diagnostics;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Windows;
	using System.Windows.Input;
	using Caliburn.Micro;
	using Database;
	using Helpers;
	using Models;
	using OpenQA.Selenium;
	using static Helpers.Helper;
	using Keys = System.Windows.Forms.Keys;
	using Timer = System.Threading.Timer;


	public class MenuViewModel : Screen
	{
		private Random rnd = new Random();
		private readonly WindowManager wm = new WindowManager();
		private BindableCollection<Cartoon> _cartoons = new BindableCollection<Cartoon>();
		private List<Cartoon> _checkedCartoons = new List<Cartoon>();
		private TimeSpan _totalDuration;
		private string _episodeCount;
		private IWebElement WebElement;

		public bool IsDelayedStart { get; set; }

		private double _opacity = 1;

		public double Opacity
		{
			get => _opacity;
			set
			{
				_opacity = value;
				NotifyOfPropertyChange(() => Opacity);
			}
		}

		public TimeSpan CurrentDuration { get; set; }

		public MenuViewModel(HotkeysRegistrator hotReg)
		{
			hotReg.RegisterGlobalHotkey(Pause, Keys.Pause, ModifierKeys.None);
			hotReg.RegisterGlobalHotkey(Exit, Keys.Pause, ModifierKeys.Shift);
			hotReg.RegisterGlobalHotkey(Start, Keys.P, ModifierKeys.Alt);
		}

		public MenuViewModel()
		{

		}

		public void Pause()
		{
			IsPause = !IsPause;
		}

		private bool _isPause = false;

		public bool IsPause
		{
			get => _isPause;
			set
			{
				_isPause = value;
				NotifyOfPropertyChange(() => IsPause);
			}
		}

		private bool _shutdownComp;

		public bool ShutdownComp
		{
			get => _shutdownComp;
			set
			{
				_shutdownComp = value;
				NotifyOfPropertyChange(() => ShutdownComp);
			}
		}

		protected override void OnInitialize()
		{
			LoadCartoons();
			SetDefaultValues();

			base.OnInitialize();
		}

		private void SetDefaultValues()
		{
			EpisodeCount = "10";

			foreach (var c in Cartoons)
			{
				if (c.Name == "Южный парк" || c.Name == "Гриффины")
				{
					c.Checked = true;
				}
			}

			CheckedValidation();
			NotifyOfPropertyChange(() => Cartoons);
		}

		/// <summary>
		/// Загрузка списка мультфильмов из базы данных
		/// </summary>
		private void LoadCartoons()
		{
			using (var ctx = new CVDbContext())
			{
				ctx.Cartoons.Load();
				Cartoons.AddRange(ctx.Cartoons.Local.OrderBy(c => c.CartoonId));
			}
		}

		#region Actions

		/// <summary>
		/// Действие при выборе/снятия выбора с мультфильма
		/// </summary>
		public void CheckedValidation()
		{
			CheckedCartoons.Clear();
			CheckedCartoons.AddRange(Cartoons.Where(c => c.Checked));
			NotifyOfPropertyChange(() => CanStart);
		}

		/// <summary>
		/// Оповещение свойств
		/// </summary>
		private void NotifyEpisodesTime()
		{
			NotifyOfPropertyChange(() => EpisodeCount);
			NotifyOfPropertyChange(() => DayDuration);
			NotifyOfPropertyChange(() => HourDuration);
			NotifyOfPropertyChange(() => MinuteDuration);
		}

		#endregion



		public void Exit()
		{
			((MainViewModel)Parent).TryClose();
		}

		public void CursorOnExit()
		{
			Background = new Uri("../Resources/MainBackgroundOnExit.png", UriKind.Relative);
		}

		public void CursorOutsideExit()
		{
			Background = new Uri("../Resources/MainBackground.png", UriKind.Relative);
		}

		public List<Cartoon> LoadedCartoons { get; set; }


		/// <summary>
		/// Запуск
		/// </summary>
		public async void Start()
		{
			((MainViewModel)Parent).WindowState = WindowState.Minimized;

			StartBrowser();

			await Task.Run(() => ChooseEpisode());
		}
		
		public bool CanStart => CheckedCartoons.Count > 0;
		
		private void ChooseEpisode()
		{
			var rndCartList = new List<int>();

			for (var i = 0; i < int.Parse(EpisodeCount); i++)
			{
				rndCartList.Add(rnd.Next(1, 101) % CheckedCartoons.Count);
			}


			foreach (var cartoonNum in rndCartList)
			{
				PlayEpisode(CheckedCartoons[cartoonNum]);
			}

			if (ShutdownComp)
			{
				var psi = new ProcessStartInfo("shutdown", "/s /t 0")
				{
					CreateNoWindow = true,
					UseShellExecute = false
				};
				Process.Start(psi);
			}



			Exit();
		}

		public void PlayEpisode(Cartoon cartoon)
		{
			string address;

			while (true)
			{
				Browser.Navigate().GoToUrl($"https://{cartoon.Url}.freehat.cc/episode/rand.php");
				address = Browser.Url;

				if (ExtractNumber(address) > 0)
				{
					break;
				}
			}

			var episodeNum = ExtractNumber(address);

			if (cartoon.Name == "Южный парк")
			{
				CurrentSkipCount = 7;

				#region Начальные пропуски

				if (episodeNum == 514)
				{
					CurrentSkipCount = 0;
				}

				if (episodeNum >= 801 &&
					episodeNum <= 814)
				{
					CurrentSkipCount = 8;
				}

				if (episodeNum == 1901 ||
					episodeNum == 1902 ||
					episodeNum == 1905 ||
					episodeNum == 1906 ||
					(episodeNum >= 2001 && episodeNum <= 2010))
				{
					CurrentSkipCount = 10;
				}

				if (episodeNum == 1903 ||
					episodeNum == 1904 ||
					episodeNum == 1907 ||
					episodeNum == 1908 ||
					episodeNum == 1910 ||
					episodeNum == 2104 ||
					episodeNum == 2105)
				{
					CurrentSkipCount = 11;
				}

				if (episodeNum == 1406 || episodeNum == 1909 ||
					(episodeNum >= 2101 && episodeNum <= 2103))
				{
					CurrentSkipCount = 12;
				}

				if ((episodeNum >= 2106 && episodeNum <= 2110) ||
					(episodeNum >= 2202 && episodeNum <= 2209))
				{
					CurrentSkipCount = 13;
				}

				if (episodeNum == 2201)
				{
					CurrentSkipCount = 14;
				}

				if (episodeNum == 2210)
				{
					CurrentSkipCount = 15;
				}

				#endregion

				if (episodeNum >= 101 && episodeNum <= 113)
				{
					Browser.Navigate().GoToUrl($"{address}?v=par");
				}
			}

			PlayCartoon();

			CurrentDuration = cartoon.Name == "Южный парк"
				? new TimeSpan(0, 21, 10)
				: new TimeSpan(0, 21, 30);

			Helper.Timer.Restart();
			LaunchTimer();

			Msg.PressKey(VK_ESCAPE);
		}

		private Uri _background = new Uri("../Resources/MainBackground.png", UriKind.Relative);

		public Uri Background
		{
			get => _background;
			set
			{
				_background = value;
				NotifyOfPropertyChange(() => Background);
			}
		}

		public void PlayCartoon()
		{
			WebElement = Browser.FindElement(By.CssSelector("pjsdiv:nth-child(8) > pjsdiv > pjsdiv"));

			WebElement.Click();
			Thread.Sleep(500);
			WebElement.Click();
			Thread.Sleep(500);


			Msg.PressKey(VK_F);
			Msg.PressKey(VK_LEFT);
			Msg.PressKey(VK_RIGHT, CurrentSkipCount);

			Thread.Sleep(500);
			Msg.PressKey(VK_SPACE);
		}

		private void LaunchTimer()
		{
			var autoEvent = new AutoResetEvent(false);

			var stateTimer = new Timer(TimerBreaker, autoEvent,
									   new TimeSpan(0, 0, 0), new TimeSpan(0, 0, 1));
			autoEvent.WaitOne();
			autoEvent.Dispose();
			stateTimer.Dispose();
		}

		/// <summary>
		/// Прерыватель таймера
		/// </summary>
		/// <param name="stateInfo"></param>
		public void TimerBreaker(object stateInfo)
		{
			var autoEvent = (AutoResetEvent)stateInfo;

			if (Helper.Timer.Elapsed > CurrentDuration)
			{
				Helper.Timer.Reset();
				autoEvent.Set();
			}

			if (IsDelayedStart)
			{

			}

			//Set pause
			if (IsPause && Helper.Timer.IsRunning)
			{
				Helper.Timer.Stop();
				Msg.PressKey(VK_SPACE);
				((MainViewModel)Parent).WindowState = WindowState.Normal;
			}

			//Unpause
			if (!IsPause && !Helper.Timer.IsRunning)
			{
				Helper.Timer.Start();
				((MainViewModel)Parent).WindowState = WindowState.Minimized;
				Msg.PressKey(VK_SPACE);
			}
		}

		private int ExtractNumber(string address)
		{
			var temp = "";

			foreach (var s in address)
			{
				if (char.IsDigit(s))
				{
					temp += s;
				}
			}

			if (int.TryParse(temp, out var res))
			{
				if (res < 100)
				{
					return -1;
				}

				return res;
			}

			return -1;
		}

		#region Properties

		/// <summary>
		/// Список мультфильмов
		/// </summary>
		public BindableCollection<Cartoon> Cartoons
		{
			get => _cartoons;
			set
			{
				_cartoons = value;
				NotifyOfPropertyChange(() => Cartoons);
			}
		}

		/// <summary>
		/// Список выбранных мультфильмов
		/// </summary>
		public List<Cartoon> CheckedCartoons
		{
			get => _checkedCartoons;
			set
			{
				_checkedCartoons = value;
				NotifyOfPropertyChange(() => CheckedCartoons);
			}
		}

		/// <summary>
		/// Общая длительность выбранного количество серий
		/// </summary>
		public TimeSpan TotalDuration
		{
			get => _totalDuration;
			set
			{
				_totalDuration = value;
				NotifyOfPropertyChange(() => TotalDuration);
			}
		}

		/// <summary>
		/// Минуты общей длительности
		/// </summary>
		public int MinuteDuration => TotalDuration.Minutes;
		/// <summary>
		/// Часы общей длительности
		/// </summary>
		public int HourDuration => TotalDuration.Hours;
		/// <summary>
		/// Дни общей длительности
		/// </summary>
		public int DayDuration => TotalDuration.Days;

		/// <summary>
		/// Количество серий для просмотра
		/// </summary>
		public string EpisodeCount
		{
			get => int.TryParse(_episodeCount, out var val) ? val.ToString() : "0";
			set
			{
				_episodeCount = int.TryParse(value, out var temp)
					? temp.ToString()
					: "0";
				TotalDuration =
					new TimeSpan(
						0,
						(int)Math.Ceiling(ApproximateEpisodeDuration.TotalMinutes
										  * (double.Parse(_episodeCount))),
						0);
				NotifyEpisodesTime();
			}
		}

		#endregion

	}
}
