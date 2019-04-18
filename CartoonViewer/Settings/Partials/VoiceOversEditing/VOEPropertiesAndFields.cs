namespace CartoonViewer.Settings.ViewModels
{
	using System.Windows;
	using Caliburn.Micro;
	using Models.CartoonModels;

	public partial class VoiceOversEditingViewModel : Screen
	{
		#region Private fields

		private BindableCollection<Cartoon> _cartoons = new BindableCollection<Cartoon>();
		private BindableCollection<CartoonVoiceOver> _cartoonVoiceOvers = new BindableCollection<CartoonVoiceOver>();
		private BindableCollection<CartoonSeason> _cartoonSeasons = new BindableCollection<CartoonSeason>();
		private BindableCollection<CartoonEpisode> _cartoonEpisodes = new BindableCollection<CartoonEpisode>();
		private BindableCollection<CartoonVoiceOver> _episodeVoiceOvers = new BindableCollection<CartoonVoiceOver>();
		private Cartoon _selectedCartoon;
		private CartoonVoiceOver _selectedCartoonVoiceOver;
		private CartoonSeason _selectedSeason;
		private CartoonEpisode _selectedEpisode;
		private CartoonVoiceOver _selectedEpisodeVoiceOver;
		private (int WebSiteId, int CartoonId, int SeasonId, int EpisodeId) IdList;

		//private readonly (int cartoonId, int seasonId, int episodeId) UsagesIds;

		#endregion

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
		/// Выбранный мультфильм
		/// </summary>
		public Cartoon SelectedCartoon
		{
			get => _selectedCartoon;
			set
			{
				IdList.CartoonId = value?.CartoonId ?? 0;
				_selectedCartoon = value;
				NotifyOfPropertyChange(() => SelectedCartoon);
				ChangeData();
			}
		}

		/// <summary>
		/// Список озвучек мультфильма
		/// </summary>
		public BindableCollection<CartoonVoiceOver> CartoonVoiceOvers
		{
			get => _cartoonVoiceOvers;
			set
			{
				_cartoonVoiceOvers = value;
				NotifyOfPropertyChange(() => CartoonVoiceOvers);
			}
		}
		/// <summary>
		/// Выбранная озвучка мульфтильма
		/// </summary>
		public CartoonVoiceOver SelectedCartoonVoiceOver
		{
			get => _selectedCartoonVoiceOver;
			set
			{
				_selectedCartoonVoiceOver = value;
				NotifyOfPropertyChange(() => SelectedCartoonVoiceOver);
			}
		}
		/// <summary>
		/// Временная выбранная озвучка мультфильма для определения наличия изменений
		/// </summary>
		public CartoonVoiceOver TempSelectedCartoonVoiceOver { get; set; }
		/// <summary>
		/// Список сезонов мультфильма
		/// </summary>
		public BindableCollection<CartoonSeason> CartoonSeasons
		{
			get => _cartoonSeasons;
			set
			{
				_cartoonSeasons = value;
				NotifyOfPropertyChange(() => CartoonSeasons);
			}
		}
		/// <summary>
		/// Выбранный сезон мультфильма
		/// </summary>
		public CartoonSeason SelectedSeason
		{
			get => _selectedSeason;
			set
			{
				IdList.SeasonId = value?.CartoonSeasonId ?? 0;
				_selectedSeason = value;
				NotifyOfPropertyChange(() => SelectedSeason);
				ChangeData();
			}
		}
		/// <summary>
		/// Список эпизодов выбранного сезона
		/// </summary>
		public BindableCollection<CartoonEpisode> CartoonEpisodes
		{
			get => _cartoonEpisodes;
			set
			{
				_cartoonEpisodes = value;
				NotifyOfPropertyChange(() => CartoonEpisodes);
			}
		}
		/// <summary>
		/// Выбранный эпизод
		/// </summary>
		public CartoonEpisode SelectedEpisode
		{
			get => _selectedEpisode;
			set
			{
				IdList.EpisodeId = value?.CartoonEpisodeId ?? 0;
				_selectedEpisode = value;
				NotifyOfPropertyChange(() => SelectedEpisode);
				ChangeData();
			}
		}
		/// <summary>
		/// Список озвучек выбранного эпизода
		/// </summary>
		public BindableCollection<CartoonVoiceOver> EpisodeVoiceOvers
		{
			get => _episodeVoiceOvers;
			set
			{
				_episodeVoiceOvers = value;
				NotifyOfPropertyChange(() => EpisodeVoiceOvers);
			}
		}
		/// <summary>
		/// Выбранная озвучка эпизода
		/// </summary>
		public CartoonVoiceOver SelectedEpisodeVoiceOver
		{
			get => _selectedEpisodeVoiceOver;
			set
			{
				_selectedEpisodeVoiceOver = value;
				NotifyOfPropertyChange(() => SelectedEpisodeVoiceOver);
			}
		}
		/// <summary>
		/// Временная выбранная озвучка эпизода для определения наличия изменений
		/// </summary>
		public CartoonVoiceOver TempSelectedEpisodeVoiceOver { get; set; }

		
		/// <summary>
		/// Свойство Visibility списка сезонов и озвучек выбравнного мультфильма
		/// </summary>
		public Visibility SeasonsAndCartoonVoiceOversVisibility =>
			SelectedCartoon == null
				? Visibility.Hidden
				: Visibility.Visible;

		/// <summary>
		/// Свойство Visibility списка эпизодов
		/// </summary>
		public Visibility EpisodesVisibility =>
			SelectedSeason == null
				? Visibility.Hidden
				: Visibility.Visible;

		/// <summary>
		/// Свойство Visibility списка озвучек выбранного эпизода
		/// </summary>
		public Visibility EpisodeVoiceOversVisibility =>
			SelectedEpisode == null ||
			EpisodesVisibility == Visibility.Hidden
				? Visibility.Hidden
				: Visibility.Visible;

		#endregion
	}
}
