namespace CartoonViewer.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.Data.Entity;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using Caliburn.Micro;
	using Database;
	using Helpers;
	using Models;
	using OpenQA.Selenium;
	using static Helpers.Helper;


	public class MenuViewModel : Screen
	{
		private Random rnd = new Random();
		private readonly WindowManager wm = new WindowManager();
		private BindableCollection<Cartoon> _cartoons = new BindableCollection<Cartoon>();
		private List<Cartoon> _checkedCartoons = new List<Cartoon>();
		private TimeSpan _totalDuration = new TimeSpan();
		private string _episodeCount;
		private bool exit;

		public MenuViewModel()
		{
			EpisodeCount = "68";
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




		public List<Cartoon> LoadedCartoons { get; set; }


		/// <summary>
		/// Запуск
		/// </summary>
		public async void Start()
		{
			using (var ctx = new CVDbContext())
			{
				foreach (var cc in CheckedCartoons)
				{
					ctx.Cartoons.Where(c => c.CartoonId == cc.CartoonId).Include("Seasons").Include("Episodes").Load();
					LoadedCartoons.AddRange(ctx.Cartoons.Local);
				}
			}

			var t = LoadedCartoons;
			

			StartBrowser();



			//((MainViewModel)Parent).WindowState = WindowState.Minimized;

			//await Task.Run(LaunchTimer);

			ChooseEpisode();
		}

		public bool CanStart => CheckedCartoons.Count > 0;

		private Task LaunchTimer(TimeSpan duration)
		{
			var autoEvent = new AutoResetEvent(false);
			
			var stateTimer = new Timer(TimerBreaker,autoEvent, 
			                           new TimeSpan(0, 0, 0), duration);
			autoEvent.WaitOne();
			stateTimer.Dispose();

			return Task.CompletedTask;
		}

		private void CreateTimer()
		{
			ChooseEpisode();




			var tm = new TimerCallback(PlayEpisode);


		}

		public void PlayEpisode(object obj)
		{

		}

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
		}

		/// <summary>
		/// Прерыватель таймера
		/// </summary>
		/// <param name="stateInfo"></param>
		public void TimerBreaker(object stateInfo)
		{
			var autoEvent = (AutoResetEvent)stateInfo;

			if (exit) autoEvent.Set();

			exit = true;
		}

		public void PlayCartoon(Cartoon cartoon)
		{
			Browser.Navigate().GoToUrl("https://sp.freehat.cc/episode/rand.php");
			var address = Browser.Url;

			//if (ExtractNumber(address) < 200)
			//{
			//	Browser.Navigate().GoToUrl($"{address}?v=par");
			//}

			var el = Browser.FindElement(By.CssSelector("pjsdiv:nth-child(8) > pjsdiv > pjsdiv"));

			el.Click();
			Thread.Sleep(100);
			el.Click();

			if (FirstStart)
			{
				Msg.PressKey(VK_F);
				FirstStart = false;
			}

			Msg.PressKey(VK_LEFT);
			Msg.PressKey(VK_RIGHT, CurrentSkipCount);

			Thread.Sleep(500);
			el.Click();
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
