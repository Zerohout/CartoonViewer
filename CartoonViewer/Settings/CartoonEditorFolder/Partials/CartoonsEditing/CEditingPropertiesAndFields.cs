// ReSharper disable CheckNamespace
namespace CartoonViewer.Settings.CartoonEditorFolder.ViewModels
{
	using System.Windows;
	using Caliburn.Micro;
	using Helpers;
	using Models.CartoonModels;
	using Newtonsoft.Json;
	using static Helpers.Helper;

	public partial class CartoonsEditingViewModel : Screen, ISettingsViewModel
	{
		private BindableCollection<CartoonVoiceOver> _voiceOvers = new BindableCollection<CartoonVoiceOver>();
		private BindableCollection<CartoonSeason> _seasons = new BindableCollection<CartoonSeason>();
		private Cartoon _selectedCartoon;
		private CartoonSeason _selectedSeason;
		private CartoonUrl _selectedCartoonUrl;

		public string TempCartoonSnapshot { get; set; }



		#region Flags

		/// <summary>
		/// Флаг наличия изменений
		/// </summary>
		public bool HasChanges
		{
			get
			{
				if(SelectedCartoon == null && TempCartoonSnapshot == null) return false;

				var temp = JsonConvert.DeserializeObject<Cartoon>(TempCartoonSnapshot);

				return IsEquals(SelectedCartoon, temp) is false;
			}
		}

		#endregion


		/// <summary>
		/// Выбранный м/с
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
		/// Список озвучек выбранного м/с
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
		/// Список сезонов выбранного м/с
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
		/// Выьранный сезон м/с
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

		#region Visibility

		/// <summary>
		/// Видимость элементов связанным с изменением м/с
		/// </summary>
		public Visibility SaveChangesVisibility =>
			SelectedCartoon?.Name == SettingsHelper.NewElementString
				? Visibility.Hidden
				: Visibility.Visible;

		/// <summary>
		/// Видимость элементов связанных с созданием нового м/с
		/// </summary>
		public Visibility CreateNewCartoonVisibility =>
			SelectedCartoon?.Name == SettingsHelper.NewElementString
				? Visibility.Visible
				: Visibility.Hidden;

		#endregion
	}
}
