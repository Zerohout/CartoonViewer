// ReSharper disable once CheckNamespace
namespace CartoonViewer.MainMenu.ViewModels
{
	using System;
	using System.Collections.Generic;
	using Caliburn.Micro;
	using Database;
	using Helpers;
	using Models.CartoonModels;
	using OpenQA.Selenium;
	using static Helpers.Helper;

	public partial class MainMenuViewModel : Screen
	{
		private CVDbContext CvDbContext = new CVDbContext(SettingsHelper.AppDataPath);
		private readonly Random rnd = new Random();
		private BindableCollection<Cartoon> _cartoons = new BindableCollection<Cartoon>();

		private IWebElement WebElement;
		private bool _isPaused;
		private bool _isShutdownComp;
		private double _opacity = 1;
		private int _currentEpisodeIndex;
		private Uri _background = new Uri(MMBackgroundUri, UriKind.Relative);
		private string _episodesCountRemainingString = "Серий к просмотру";

		public TimeSpan TotalEpisodeTime { get; set; }


		/// <summary>
		/// Список мультсериалов
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

		public List<CartoonEpisode> CheckedEpisodes { get; set; } = new List<CartoonEpisode>();

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
		public bool IsPaused
		{
			get => _isPaused;
			set
			{
				_isPaused = value;
				NotifyOfPropertyChange(() => IsPaused);
			}
		}

		public bool IsNowPause { get; set; }

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
		/// Флаг переключения серии
		/// </summary>
		public bool IsSwitchEpisode { get; set; }

		#endregion

		#region Свойства связанные со временем

		/// <summary>
		/// Конечное время просмотра указанного числа серий
		/// </summary>
		public TimeSpan EndTime => DateTime.Now.TimeOfDay +
								   new TimeSpan(
									   0,
									   0,
									   (int)GeneralSettings.ApproximateDuration.TotalSeconds)
								   + (CurrentDuration - ElapsedTime);

		/// <summary>
		/// Конечная дата просмотра указанного числа серий
		/// </summary>
		public DateTime EndDate => DateTime.Now.AddDays(EndTime.Days);

		/// <summary>
		/// Текущая длительность серии
		/// </summary>
		public TimeSpan CurrentDuration { get; set; }

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
