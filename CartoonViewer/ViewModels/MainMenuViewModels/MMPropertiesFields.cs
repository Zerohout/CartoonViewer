namespace CartoonViewer.ViewModels
{
	using System;
	using System.Collections.Generic;
	using Caliburn.Micro;
	using Models;
	using OpenQA.Selenium;
	using static Helpers.Helper;

	public partial class MainMenuViewModel : Screen
	{
		private readonly Random rnd = new Random();
		private BindableCollection<Cartoon> _cartoons = new BindableCollection<Cartoon>();
		private List<Cartoon> _checkedCartoons = new List<Cartoon>();
		private TimeSpan _totalDuration;
		private TimeSpan _delayedSkipDuration;
		
		private IWebElement WebElement;
		private bool _isPause;
		private bool _shutdownComp;
		private string _episodeCount;
		private double _opacity = 1;
		private Uri _background = new Uri("../Resources/MainBackground.png", UriKind.Relative);

		public bool SwitchEpisode { get; set; } = false;

		/// <summary>
		/// Текущая длительность серии
		/// </summary>
		public TimeSpan CurrentDuration { get; set; }

		/// <summary>
		/// Флаг установки отложенного старта
		/// </summary>
		public bool IsDelayedSkip { get; set; }
		
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
		/// Продолжительность отложенного пропуска
		/// </summary>
		public TimeSpan DelayedSkipDuration
		{
			get => _delayedSkipDuration;
			set
			{
				_delayedSkipDuration = value;
				NotifyOfPropertyChange(() => DelayedSkipDuration);
			}
		}

		/// <summary>
		/// Флаг включения паузы
		/// </summary>
		public bool IsPause
		{
			get => _isPause;
			set
			{
				_isPause = value;
				NotifyOfPropertyChange(() => IsPause);
			}
		}

		/// <summary>
		/// Флаг выключения компьютера
		/// </summary>
		public bool ShutdownComp
		{
			get => _shutdownComp;
			set
			{
				_shutdownComp = value;
				NotifyOfPropertyChange(() => ShutdownComp);
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

		/// <summary>
		/// Прозрачность элементов и фона MainMenu
		/// </summary>
		public double Opacity
		{
			get => _opacity;
			set
			{
				_opacity = value;
				NotifyOfPropertyChange(() => Opacity);
			}
		}

		/// <summary>
		/// Фон MainMenu
		/// </summary>
		public Uri Background
		{
			get => _background;
			set
			{
				_background = value;
				NotifyOfPropertyChange(() => Background);
			}
		}
	}
}
