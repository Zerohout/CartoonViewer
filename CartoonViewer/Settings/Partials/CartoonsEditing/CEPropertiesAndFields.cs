namespace CartoonViewer.Settings.ViewModels
{
	using System.Windows;
	using Caliburn.Micro;
	using Helpers;
	using Models.CartoonModels;

	public partial class CartoonsEditingViewModel : Screen, ISettingsViewModel
	{
		#region Private fields

		private readonly int WebSiteId;

		private readonly int CartoonId;
		private string _name;
		private string _url;
		private string _description;

		private Visibility _cartoonTypesVisibility = Visibility.Hidden;
		private CartoonSeason _selectedSeason;
		private BindableCollection<CartoonVoiceOver> _voiceOvers = new BindableCollection<CartoonVoiceOver>();
		private BindableCollection<CartoonSeason> _seasons = new BindableCollection<CartoonSeason>();
		
		private CartoonUrl _selectedCartoonUrl;

		public int SeasonIndex { get; set; }

		public CartoonUrl SelectedCartoonUrl
		{
			get => _selectedCartoonUrl;
			set
			{
				_selectedCartoonUrl = value;
				NotifyOfPropertyChange(() => SelectedCartoonUrl);
			}
		}

		public CartoonUrl TempCartoonUrl { get; set; }

		private Cartoon _selctedCartoon;

		public Cartoon SelectedCartoon
		{
			get => _selctedCartoon;
			set
			{
				_selctedCartoon = value;
				NotifyOfPropertyChange(() => SelectedCartoon);
			}
		}

		public Cartoon TempCartoon { get; set; }


		#endregion

		#region Properties

		public BindableCollection<CartoonVoiceOver> VoiceOvers
		{
			get => _voiceOvers;
			set
			{
				_voiceOvers = value;
				NotifyOfPropertyChange(() => VoiceOvers);
			}
		}

		public Visibility SaveChangesVisibility { get; set; } = Visibility.Hidden;
		public Visibility AddCartoonVisibility { get; set; } = Visibility.Hidden;
		public BindableCollection<string> CartoonTypes { get; set; } = Helper.CartoonTypes;


		public bool HasChanges => CanSaveChanges;

		public BindableCollection<CartoonSeason> Seasons
		{
			get => _seasons;

			set
			{
				_seasons = value;
				NotifyOfPropertyChange(() => Seasons);
			}
		}

		public CartoonSeason SelectedSeason
		{
			get => _selectedSeason;
			set
			{
				_selectedSeason = value;
				NotifyOfPropertyChange(() => SelectedSeason);
			}
		}

		public Visibility CartoonTypesVisibility
		{
			get => _cartoonTypesVisibility;
			set
			{
				_cartoonTypesVisibility = value;
				NotifyOfPropertyChange(() => CartoonTypesVisibility);
			}
		}

		public string Name
		{
			get => _name;
			set
			{
				_name = value;
				NotifyOfPropertyChange(() => Name);
			}
		}

		public string TempName { get; set; }

		public string Url
		{
			get => _url;
			set
			{
				_url = value;
				NotifyOfPropertyChange(() => Url);
			}
		}

		public string TempUrl { get; set; }



		public string Description
		{
			get => _description;
			set
			{
				_description = value;
				NotifyOfPropertyChange(() => Description);
			}
		}

		public string TempDescription { get; set; }

		#endregion
	}
}
