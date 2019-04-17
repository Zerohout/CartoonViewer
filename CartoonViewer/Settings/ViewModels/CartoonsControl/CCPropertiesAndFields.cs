namespace CartoonViewer.Settings.ViewModels
{
	using System.Windows;
	using Caliburn.Micro;
	using Models.CartoonModels;

	public partial class CartoonsControlViewModel : Conductor<Screen>.Collection.OneActive
	{
		#region Private fields

		//private Visibility _cartoonsVisibility = Visibility.Hidden;
		//private Visibility _seasonsVisibility = Visibility.Hidden;
		private CartoonWebSite _selectedWebSite;
		private Cartoon _selectedCartoon;
		private CartoonSeason _selectedSeason;
		private BindableCollection<CartoonWebSite> _webSites = new BindableCollection<CartoonWebSite>();
		private BindableCollection<Cartoon> _cartoons = new BindableCollection<Cartoon>();
		private BindableCollection<CartoonSeason> _seasons = new BindableCollection<CartoonSeason>();

		#endregion

		#region Properties

		public CartoonWebSite SelectedWebSite
		{
			get => _selectedWebSite;
			set
			{
				_selectedWebSite = value;
				NotifyOfPropertyChange(() => SelectedWebSite);
				NotifyOfPropertyChange(() => CartoonsVisibility);
			}
		}

		public Cartoon SelectedCartoon
		{
			get => _selectedCartoon;
			set
			{
				_selectedCartoon = value;
				NotifyOfPropertyChange(() => SelectedCartoon);
				NotifyOfPropertyChange(() => CartoonEditingAndSeasonsVisibility);
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

		public BindableCollection<CartoonWebSite> WebSites
		{
			get => _webSites;
			set
			{
				_webSites = value;
				NotifyOfPropertyChange(() => WebSites);
			}
		}

		public BindableCollection<Cartoon> Cartoons
		{
			get => _cartoons;
			set
			{
				_cartoons = value;
				NotifyOfPropertyChange(() => Cartoons);
			}
		}

		public BindableCollection<CartoonSeason> Seasons
		{
			get => _seasons;
			set
			{
				_seasons = value;
				NotifyOfPropertyChange(() => Seasons);
			}
		}

		public Visibility CartoonsVisibility =>
			SelectedWebSite == null
				? Visibility.Hidden
				: Visibility.Visible;
		public Visibility CartoonEditingAndSeasonsVisibility =>
			SelectedCartoon == null
				? Visibility.Hidden
				: Visibility.Visible;
		

		#endregion
	}
}
