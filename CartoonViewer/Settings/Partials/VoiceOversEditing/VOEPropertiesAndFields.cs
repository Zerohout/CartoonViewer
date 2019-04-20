namespace CartoonViewer.Settings.ViewModels
{
	using System.Linq;
	using System.Windows;
	using Caliburn.Micro;
	using Models.CartoonModels;

	public partial class VoiceOversEditingViewModel : Screen
	{
		#region Private fields

		private BindableCollection<Cartoon> _cartoons = new BindableCollection<Cartoon>();
		private BindableCollection<CartoonVoiceOver> _cartoonVoiceOvers = new BindableCollection<CartoonVoiceOver>();
		private BindableCollection<CartoonSeason> _seasons = new BindableCollection<CartoonSeason>();
		private BindableCollection<CartoonEpisode> _episodes = new BindableCollection<CartoonEpisode>();
		private BindableCollection<CartoonVoiceOver> _episodeVoiceOvers = new BindableCollection<CartoonVoiceOver>();
		private Cartoon _selectedCartoon;
		private CartoonVoiceOver _selectedCartoonVoiceOver;
		private CartoonSeason _selectedSeason;
		private CartoonEpisode _selectedEpisode;
		private CartoonVoiceOver _selectedEpisodeVoiceOver;
		private (int WebSiteId, int CartoonId, int SeasonId, int EpisodeId) IdList;
		private int SelectedVoiceOverId;
		private bool _isNotEditing = true;
		//private Visibility _editingVisibility;

		private CartoonVoiceOver _editedCartoonVoiceOver;

		





		//private readonly (int cartoonId, int seasonId, int episodeId) UsagesIds;

		#endregion

		#region Properties

		/// <summary>
		/// Флаг состояния редактирования
		/// </summary>
		public bool IsNotEditing
		{
			get => _isNotEditing;
			set
			{
				_isNotEditing = value;
				NotifyOfPropertyChange(() => IsNotEditing);
				NotifyOfPropertyChange(() => EditingVisibility);
				NotifyCartoonVoiceOversButtons();
				NotifyEpisodeVoiceOversButtons();
				NotifyCancelButtons();
			}
		}
		/// <summary>
		/// Флаг необходимый для корректной работы конструктора XAML
		/// </summary>
		public bool IsDesignTime { get; set; }

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
			set => ChangeSelectedCartoon(value);
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
				SelectedVoiceOverId = value?.CartoonVoiceOverId ?? 0;
				NotifyOfPropertyChange(() => SelectedCartoonVoiceOver);
				NotifyCartoonVoiceOversButtons();

				if(SelectedEpisode == null)
					return;


				_selectedEpisodeVoiceOver = EpisodeVoiceOvers
					.FirstOrDefault(evo => evo.CartoonVoiceOverId == SelectedVoiceOverId);


				NotifyOfPropertyChange(() => SelectedEpisodeVoiceOver);
				NotifyEpisodeVoiceOversButtons();

			}
		}
		/// <summary>
		/// Редактируемый экземпляр выбранной озвучки м/ф
		/// </summary>
		public CartoonVoiceOver EditedCartoonVoiceOver
		{
			get => _editedCartoonVoiceOver;
			set
			{
				_editedCartoonVoiceOver = value;
				NotifyOfPropertyChange(() => EditedCartoonVoiceOver);
			}
		}
		/// <summary>
		/// Временная выбранная озвучка мультфильма для определения наличия изменений
		/// </summary>
		public CartoonVoiceOver TempEditedCartoonVoiceOver { get; set; }
		/// <summary>
		/// Список сезонов мультфильма
		/// </summary>
		public BindableCollection<CartoonSeason> Seasons
		{
			get => _seasons;
			set
			{
				_seasons = value;
				NotifyOfPropertyChange(() => Seasons);
			}
		}
		/// <summary>
		/// Выбранный сезон мультфильма
		/// </summary>
		public CartoonSeason SelectedSeason
		{
			get => _selectedSeason;
			set => ChangeSelectedSeason(value);
		}
		/// <summary>
		/// Список эпизодов выбранного сезона
		/// </summary>
		public BindableCollection<CartoonEpisode> Episodes
		{
			get => _episodes;
			set
			{
				_episodes = value;
				NotifyOfPropertyChange(() => Episodes);
			}
		}
		/// <summary>
		/// Выбранный эпизод
		/// </summary>
		public CartoonEpisode SelectedEpisode
		{
			get => _selectedEpisode;
			set => ChangeSelectedEpisode(value);
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
				SelectedVoiceOverId = value?.CartoonVoiceOverId ?? 0;
				_selectedEpisodeVoiceOver = value;

				SelectedCartoonVoiceOver = CartoonVoiceOvers
					.FirstOrDefault(cvo => cvo.CartoonVoiceOverId == SelectedVoiceOverId);

				//NotifyEpisodeVoiceOversButtons();

			}
		}
		
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
			SelectedEpisode == null
				? Visibility.Hidden
				: Visibility.Visible;
		/// <summary>
		/// Свойство Visibility полей для редактирования выбранной озвучки м/ф
		/// </summary>
		public Visibility EditingVisibility =>
			IsNotEditing
				? Visibility.Hidden
				: Visibility.Visible;
		/// <summary>
		/// Флаг наличия изменения
		/// </summary>
		public bool HasChanges
		{
			get
			{
				if(EditedCartoonVoiceOver == null || TempEditedCartoonVoiceOver == null)
				{
					return false;
				}

				if(string.IsNullOrEmpty(EditedCartoonVoiceOver.Name) ||
				   string.IsNullOrEmpty(EditedCartoonVoiceOver.UrlParameter) ||
				   string.IsNullOrEmpty(EditedCartoonVoiceOver.Description))
				{
					return false;
				}

				if(EditedCartoonVoiceOver.Name == TempEditedCartoonVoiceOver.Name &&
				   EditedCartoonVoiceOver.UrlParameter == TempEditedCartoonVoiceOver.UrlParameter &&
				   EditedCartoonVoiceOver.Description == TempEditedCartoonVoiceOver.Description)
				{
					return false;
				}

				return true;
			}
		}

		#endregion
	}
}
