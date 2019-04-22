namespace CartoonViewer.Settings.CartoonEditorSetting.ViewModels
{
	using System.Windows;
	using Caliburn.Micro;
	using CartoonEditorSetting.ViewModels;
	using Helpers;
	using Models.CartoonModels;
	using static Helpers.Helper;

	public partial class CartoonsEditingViewModel : Screen, ISettingsViewModel
	{
		#region Private fields

		private BindableCollection<CartoonVoiceOver> _voiceOvers = new BindableCollection<CartoonVoiceOver>();
		private BindableCollection<CartoonSeason> _seasons = new BindableCollection<CartoonSeason>();
		private Cartoon _selectedCartoon;
		private CartoonSeason _selectedSeason;
		private CartoonUrl _selectedCartoonUrl;
		
		#endregion

		#region Properties

		public Visibility SaveChangesVisibility =>
			TempCartoon?.Name == NewElementString
				? Visibility.Hidden
				: Visibility.Visible;

		public Visibility CreateNewCartoonVisibility =>
			TempCartoon?.Name == NewElementString
				? Visibility.Visible
				: Visibility.Hidden;

		/// <summary>
		/// Флаг наличия изменений
		/// </summary>
		public bool HasChanges => CanSaveChanges;
		/// <summary>
		/// Выбранный м/ф
		/// </summary>
		public Cartoon SelectedCartoon
		{
			get => _selectedCartoon;
			set
			{
				_selectedCartoon = value;
				NotifyOfPropertyChange(() => SelectedCartoon);
				NotifySeasonList();
			}
		}
		/// <summary>
		/// Временный м\ф для отслеживания изменений
		/// </summary>
		public Cartoon TempCartoon { get; set; }
		/// <summary>
		/// Выбранный CartoonUrl
		/// </summary>
		public CartoonUrl SelectedCartoonUrl
		{
			get => _selectedCartoonUrl;
			set
			{
				_selectedCartoonUrl = value;
				NotifyOfPropertyChange(() => SelectedCartoonUrl);
			}
		}
		/// <summary>
		/// Временный CartoonUrl для отслеживания изменений
		/// </summary>
		public CartoonUrl TempCartoonUrl { get; set; }
		/// <summary>
		/// Список озвучек выбранного м/ф
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
		/// Список сезонов выбранного м/ф
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
		/// Выьранный сезон м/ф
		/// </summary>
		public CartoonSeason SelectedSeason
		{
			get => _selectedSeason;
			set
			{
				_selectedSeason = value;
				NotifyOfPropertyChange(() => SelectedSeason);
				NotifySeasonList();
			}
		}

		#endregion
	}
}
