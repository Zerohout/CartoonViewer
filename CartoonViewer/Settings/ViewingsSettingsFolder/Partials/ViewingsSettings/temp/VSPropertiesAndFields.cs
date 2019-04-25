// ReSharper disable CheckNamespace
namespace CartoonViewer.Settings.ViewingsSettingsFolder.ViewModels
{
	using System.Windows;
	using Caliburn.Micro;
	using CartoonEditorFolder.ViewModels;
	using Models.CartoonModels;

	public partial class ViewingsSettingsViewModel : Screen, ISettingsViewModel
	{
		private BindableCollection<Cartoon> _cartoons = new BindableCollection<Cartoon>();
		private BindableCollection<CartoonSeason> _seasons = new BindableCollection<CartoonSeason>();
		private BindableCollection<CartoonEpisode> _episodes = new BindableCollection<CartoonEpisode>();
		private BindableCollection<CartoonVoiceOver> _voiceOvers = new BindableCollection<CartoonVoiceOver>();

		private Visibility _btnVisibility;

		public Visibility BtnVisibility
		{
			get => _btnVisibility;
			set
			{
				_btnVisibility = value;
				NotifyOfPropertyChange(() => BtnVisibility);
			}
		}



		private bool _isCartoonListEnable;
		private bool _isSeasonListEnable;
		private bool _isEpisodeListEnable;

		private Cartoon _selectedCartoon;
		private CartoonSeason _selectedSeason;
		private CartoonEpisode _selectedEpisode;
		private CartoonVoiceOver _selectedVoiceOver;

		private (int CartoonId, int SeasonId, int EpisodeId, int VoiceOverId) IdList = (0, 0, 0, 0);


		#region Flags

		public bool IsDesignTime { get; set; }

		#endregion


		#region Lists content

		/// <summary>
		/// Список м/с
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
		/// список сезонов выбранного м/с
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
		public BindableCollection<CartoonVoiceOver> VoiceOvers
		{
			get => _voiceOvers;
			set
			{
				_voiceOvers = value;
				NotifyOfPropertyChange(() => VoiceOvers);
			}
		}

		/// <summary>
		/// Выбранный м/с
		/// </summary>
		public Cartoon SelectedCartoon
		{
			get => _selectedCartoon;
			set => ChangeSelectedCartoon(value);
		}

		/// <summary>
		/// Выбранный сезон
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

		public CartoonVoiceOver SelectedVoiceOver
		{
			get => _selectedVoiceOver;
			set => ChangeSelectedVoiceOver(value);

		}

		#endregion

		#region Lists data

		public bool IsCartoonListEnable
		{
			get => _isCartoonListEnable;
			set
			{
				_isCartoonListEnable = value;
				NotifyOfPropertyChange(() => IsCartoonListEnable);
			}
		}

		public bool IsSeasonListEnable
		{
			get => _isSeasonListEnable;
			set
			{
				_isSeasonListEnable = value;
				NotifyOfPropertyChange(() => IsSeasonListEnable);
			}
		}

		public bool IsEpisodeListEnable
		{
			get => _isEpisodeListEnable;
			set
			{
				_isEpisodeListEnable = value;
				NotifyOfPropertyChange(() => IsEpisodeListEnable);
			}
		}

		#endregion

		#region Visibility

		public Visibility SelectedCartoonVisibility =>
			SelectedCartoon == null
				? Visibility.Hidden
				: Visibility.Visible;

		public Visibility SelectedSeasonVisibility =>
			SelectedSeason == null
				? Visibility.Hidden
				: Visibility.Visible;

		public Visibility SelectedEpisodeVisibility =>
			SelectedEpisode == null
				? Visibility.Hidden
				: Visibility.Visible;



		#endregion


		public bool HasChanges { get; }

		public void SaveChanges()
		{

		}

		public bool CanSaveChanges => true;

	}
}
