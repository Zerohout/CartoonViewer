namespace CartoonViewer.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.Data.Entity;
	using System.Diagnostics;
	using System.Linq;
	using System.Runtime.InteropServices;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Windows;
	using System.Windows.Input;
	using System.Windows.Interop;
	using Caliburn.Micro;
	using Database;
	using Helpers;
	using Models;
	using OpenQA.Selenium;
	using OpenQA.Selenium.Chrome;
	using static Helpers.Helper;
	using Action = System.Action;
	using Keys = System.Windows.Forms.Keys;


	public class MenuViewModel : Screen
	{
		private Random rnd = new Random();
		private readonly WindowManager wm = new WindowManager();
		private BindableCollection<Cartoon> _cartoons = new BindableCollection<Cartoon>();
		private List<Cartoon> _checkedCartoons = new List<Cartoon>();
		private TimeSpan _totalDuration = new TimeSpan();
		private string _episodeCount;
		private IWebElement WebElement;
		HotkeysRegistrator _hotReg;

		public TimeSpan CurrentDuration { get; set; }

		public MenuViewModel(HotkeysRegistrator hotReg)
		{
			EpisodeCount = "68";
			FirstStart = true;
			_hotReg = hotReg;

			_hotReg.RegisterGlobalHotkey(Pause, Keys.Pause,ModifierKeys.Shift);

		}

		public void Pause()
		{

		}


		private bool _isPause;

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
			base.OnInitialize();
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
			_hotReg.UnregisterHotkeys();
			Browser?.Close();
			((MainViewModel)Parent).TryClose();
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
				Process.Start("shutdown", "/s /t 00");
			}

			

			Exit();
		}

		public void PlayEpisode(Cartoon cartoon)
		{
			Browser.Navigate().GoToUrl($"https://{cartoon.Url}.freehat.cc/episode/rand.php");
			var address = Browser.Url;

			if (cartoon.Name == "Южный парк" && ExtractNumber(address) < 200)
			{
				Browser.Navigate().GoToUrl($"{address}?v=par");
			}

			PlayCartoon();

			CurrentDuration = cartoon.Name == "Южный парк"
				? new TimeSpan(0,21,10)
				: new TimeSpan(0,21,30);


			LaunchTimer();

			Msg.PressKey(VK_ESCAPE);

		}

		public void PlayCartoon()
		{
			WebElement = Browser.FindElement(By.CssSelector("pjsdiv:nth-child(8) > pjsdiv > pjsdiv"));

			WebElement.Click();
			Thread.Sleep(500);
			WebElement.Click();


			Msg.PressKey(VK_F);
			Msg.PressKey(VK_LEFT);
			Msg.PressKey(VK_RIGHT, CurrentSkipCount);

			Thread.Sleep(500);
			WebElement.Click();
		}

		private void LaunchTimer()
		{
			var autoEvent = new AutoResetEvent(false);

			var stateTimer = new Timer(TimerBreaker, autoEvent,
									   new TimeSpan(0, 0, 0), new TimeSpan(0,0,5));
			autoEvent.WaitOne();
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

			//Set pause
			if (!IsPause && Helper.Timer.IsRunning)
			{
				Helper.Timer.Stop();
				WebElement.Click();
			}

			//Unpause
			if (IsPause && !Helper.Timer.IsRunning)
			{
				Helper.Timer.Start();
				WebElement.Click();
				((MainViewModel) Parent).WindowState = WindowState.Minimized;
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

			return int.Parse(temp);
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
