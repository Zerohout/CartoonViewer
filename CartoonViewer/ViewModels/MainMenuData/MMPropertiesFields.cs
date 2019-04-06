// ReSharper disable once CheckNamespace
namespace CartoonViewer.ViewModels
{
	using System;
	using System.Collections.Generic;
	using Caliburn.Micro;
	using Models.CartoonModels;
	using OpenQA.Selenium;
	using static Helpers.Helper;

	public partial class MainMenuViewModel : Screen
	{
		private readonly Random rnd = new Random();
		private BindableCollection<Cartoon> _cartoons = new BindableCollection<Cartoon>();
		private List<Cartoon> _checkedCartoons = new List<Cartoon>();
		private TimeSpan _delayedSkipDuration;
		private List<int> _randomCartoonNumberList = new List<int>();

		private IWebElement WebElement;
		private bool _isPause;
		private bool _isShutdownComp;
		private string _episodeCount = "10";
		private double _opacity = 1;
		private int _currentEpisodeIndex;
		private Uri _background = new Uri("../Resources/MainBackground.png", UriKind.Relative);
		private string _episodesCountRemainingString = "Серий к просмотру";

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
		/// Список случайных номеров мультфильмов для просмотра
		/// </summary>
		public List<int> RandomCartoonNumberList
		{
			get => _randomCartoonNumberList;
			set
			{
				_randomCartoonNumberList = value;
				NotifyOfPropertyChange(() => RandomCartoonNumberList);
			}
		}

		/// <summary>
		/// Индекс текущей серии
		/// </summary>
		public int CurrentEpisodeIndex
		{
			get => _currentEpisodeIndex;
			set
			{
				_currentEpisodeIndex = value;
				NotifyOfPropertyChange(() => CurrentEpisodeIndex);
			}
		}

		/// <summary>
		/// Количество серий для просмотра
		/// </summary>
		public string EpisodeCountString
		{
			get => _episodeCount;
			set
			{
				if (int.TryParse(value, out var tempValue))
				{
					if (EpisodeCount < tempValue && ElapsedTime > TimeSpan.Zero)
					{
						AddCartoonToQueue(tempValue - EpisodeCount);
					}

					_episodeCount = value;
				}
				else
				{
					_episodeCount = "0";
				}

				NotifyEpisodesTime();
			}
		}

		/// <summary>
		/// Числовое представление количества оставшихся серий для просмотра
		/// </summary>
		public int EpisodeCount => int.Parse(EpisodeCountString);

		/// <summary>
		/// Текст TextBlock'а Осталось серий/Серий к просмотру
		/// </summary>
		public string EpisodesCountRemainingString
		{
			get => _episodesCountRemainingString;
			set
			{
				_episodesCountRemainingString = value;
				NotifyOfPropertyChange(() => EpisodesCountRemainingString);
			}
		}


		#region Флаги

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
		public bool IsShutdownComp
		{
			get => _isShutdownComp;
			set
			{
				_isShutdownComp = value;
				NotifyOfPropertyChange(() => IsShutdownComp);
			}
		}

		/// <summary>
		/// Флаг установки отложенного старта
		/// </summary>
		public bool IsDelayedSkip { get; set; }
		/// <summary>
		/// Флаг переключения серии
		/// </summary>
		public bool IsSwitchEpisode { get; set; }
		
		#endregion
		
		#region Свойства связанные со временем

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
		/// Кончное время просмотра указанного числа серий
		/// </summary>
		public TimeSpan EndTime => DateTime.Now.TimeOfDay +
		                           new TimeSpan(
			                           0,
			                           (int)Math.Ceiling(ApproximateEpisodeDuration.TotalMinutes
			                                             * (double.Parse(_episodeCount))),
			                           0) + (CurrentDuration - ElapsedTime);

		/// <summary>
		/// Конечная дата просмотра указанного числа серий
		/// </summary>
		public DateTime EndDate => new DateTime(
			DateTime.Now.Year,
			DateTime.Now.Month,
			DateTime.Now.Day + EndTime.Days);

		/// <summary>
		/// Текущая длительность серии
		/// </summary>
		public TimeSpan CurrentDuration { get; set; }
		/// <summary>
		/// Год конечной даты
		/// </summary>
		public int FinalYear => EndDate.Year;
		/// <summary>
		/// Месяц конечной даты
		/// </summary>
		public int FinalMonth => EndDate.Month;
		/// <summary>
		/// День конечной даты
		/// </summary>
		public int FinalDay => EndDate.Day;
		/// <summary>
		/// Час конечного времени
		/// </summary>
		public int FinalHour => EndTime.Hours;
		/// <summary>
		/// Минута конечного времени
		/// </summary>
		public int FinalMinute => EndTime.Minutes;
		/// <summary>
		/// Прошедшее время серии
		/// </summary>
		public TimeSpan ElapsedTime => Timer.Elapsed;


		#endregion

		

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
