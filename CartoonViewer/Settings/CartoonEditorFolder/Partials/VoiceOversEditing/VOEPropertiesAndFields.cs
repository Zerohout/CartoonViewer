namespace CartoonViewer.Settings.CartoonEditorSetting.ViewModels
{
	using System.Windows;
	using Caliburn.Micro;
	using Models.CartoonModels;

	public partial class VoiceOversEditingViewModel : Screen
	{
		#region Private fields

		private BindableCollection<CartoonVoiceOver> _globalVoiceOvers = new BindableCollection<CartoonVoiceOver>();
		private BindableCollection<CartoonVoiceOver> _cartoonVoiceOvers = new BindableCollection<CartoonVoiceOver>();
		private BindableCollection<CartoonVoiceOver> _episodeVoiceOvers = new BindableCollection<CartoonVoiceOver>();
		private BindableCollection<Cartoon> _cartoons = new BindableCollection<Cartoon>();
		private BindableCollection<CartoonSeason> _seasons = new BindableCollection<CartoonSeason>();
		private BindableCollection<CartoonEpisode> _episodes = new BindableCollection<CartoonEpisode>();

		private CartoonVoiceOver _selectedGlobalVoiceOver;
		private CartoonVoiceOver _selectedCartoonVoiceOver;
		private CartoonVoiceOver _selectedEpisodeVoiceOver;
		private CartoonVoiceOver _editedVoiceOver;
		private Cartoon _selectedCartoon;
		private CartoonSeason _selectedSeason;
		private CartoonEpisode _selectedEpisode;

		private (int WebSiteId, int CartoonId, int SeasonId, int EpisodeId) IdList;
		private int SelectedVoiceOverId;
		private bool _isNotEditing = true;

		#endregion

		#region Flags

		/// <summary>
		/// Флаг необходимый для корректной работы конструктора XAML
		/// </summary>
		public bool IsDesignTime { get; set; }

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
				NotifySharedVoiceOversButtons();
				NotifyGlobalVoiceOversButtons();
				NotifyCartoonVoiceOversButtons();
				NotifyEpisodeVoiceOversButtons();
				NotifyCancelButtons();
			}
		}

		/// <summary>
		/// Флаг наличия изменения
		/// </summary>
		public bool HasChanges
		{
			get
			{
				if(EditedVoiceOver == null || TempEditedVoiceOver == null)
				{
					return false;
				}

				if (string.IsNullOrEmpty(EditedVoiceOver.Name))
				{
					return false;
				}

				if(EditedVoiceOver.Name == TempEditedVoiceOver.Name &&
				   EditedVoiceOver.UrlParameter == TempEditedVoiceOver.UrlParameter &&
				   EditedVoiceOver.Description == TempEditedVoiceOver.Description)
				{
					return false;
				}

				return true;
			}
		}
		
		#endregion

		#region Collections

		/// <summary>
		/// Глобальный список (всех) озвучек
		/// </summary>
		public BindableCollection<CartoonVoiceOver> GlobalVoiceOvers
		{
			get => _globalVoiceOvers;
			set
			{
				_globalVoiceOvers = value;
				NotifyOfPropertyChange(() => GlobalVoiceOvers);
			}
		}

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

		/// <summary>
		/// Список озвучек мультсериала
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
		/// Список сезонов мультсериала
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

		#endregion

		#region Top selections

		/// <summary>
		/// Выбранный мультсериал
		/// </summary>
		public Cartoon SelectedCartoon
		{
			get => _selectedCartoon;
			set => ChangeSelectedCartoon(value);
		}

		/// <summary>
		/// Выбранный сезон мультсериала
		/// </summary>
		public CartoonSeason SelectedSeason
		{
			get => _selectedSeason;
			set => ChangeSelectedSeason(value);
		}

		/// <summary>
		/// Выбранный эпизод
		/// </summary>
		public CartoonEpisode SelectedEpisode
		{
			get => _selectedEpisode;
			set => ChangeSelectedEpisode(value);
		}

		#endregion

		#region Selected voice overs

		/// <summary>
		/// Выбранная глобальная озвучка
		/// </summary>
		public CartoonVoiceOver SelectedGlobalVoiceOver
		{
			get => _selectedGlobalVoiceOver;
			set
			{
				SelectedVoiceOverId = value?.CartoonVoiceOverId ?? 0;
				SetVoiceOverValue();
			}
		}

		/// <summary>
		/// Выбранная озвучка м/ф
		/// </summary>
		public CartoonVoiceOver SelectedCartoonVoiceOver
		{
			get => _selectedCartoonVoiceOver;
			set
			{
				SelectedVoiceOverId = value?.CartoonVoiceOverId ?? 0;
				SetVoiceOverValue();
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
				SetVoiceOverValue();
			}
		}

		#endregion

		#region Editable properties

		/// <summary>
		/// Редактируемый экземпляр выбранной озвучки
		/// </summary>
		public CartoonVoiceOver EditedVoiceOver
		{
			get => _editedVoiceOver;
			set
			{
				_editedVoiceOver = value;
				NotifyOfPropertyChange(() => EditedVoiceOver);
			}
		}

		/// <summary>
		/// Временный экземпляр редактируемой озвучки
		/// </summary>
		public CartoonVoiceOver TempEditedVoiceOver { get; set; }

		#endregion

		#region Visibility

		/// <summary>
		/// Свойство Visibility элементов зависимых от выбранного м/ф
		/// </summary>
		public Visibility SelectedCartoonVisibility =>
			SelectedCartoon == null
				? Visibility.Hidden
				: Visibility.Visible;

		/// <summary>
		/// Свойство Visibility элементов зависимых от выбранного сезона
		/// </summary>
		public Visibility SelectedSeasonVisibility =>
			SelectedSeason == null
				? Visibility.Hidden
				: Visibility.Visible;

		/// <summary>
		/// Свойство Visibility элементов зависимых от выбранного эпизода
		/// </summary>
		public Visibility SelectedEpisodeVisibility =>
			SelectedEpisode == null
				? Visibility.Hidden
				: Visibility.Visible;
		/// <summary>
		/// Свойство Visibility полей для редактирования выбранной озвучки
		/// </summary>
		public Visibility EditingVisibility =>
			IsNotEditing
				? Visibility.Hidden
				: Visibility.Visible;

		#endregion

	}
}
