namespace CartoonViewer.Settings.ViewModels
{
	using System.Data.Entity;
	using System.Linq;
	using System.Windows;
	using System.Windows.Data;
	using Caliburn.Micro;
	using CartoonViewer.ViewModels;
	using Database;
	using Helpers.Converters;
	using Models.CartoonModels;
	using static Helpers.Helper;

	public class CartoonsControlViewModel : Conductor<Screen>.Collection.OneActive
	{
		private Visibility _cartoonsVisibility = Visibility.Hidden;
		private Visibility _seasonsVisibility = Visibility.Hidden;
		private WebSite _webSite;
		private Cartoon _cartoon;
		private Season _season;
		private BindableCollection<WebSite> _webSites = new BindableCollection<WebSite>();
		private BindableCollection<Cartoon> _cartoons = new BindableCollection<Cartoon>();
		private BindableCollection<Season> _seasons = new BindableCollection<Season>();

		public IValueConverter NullReplacer => new NullReplaceConverter();
		public IValueConverter EnumerableNulReplacer => new EnumerableNullReplaceConverter();


		public CartoonsControlViewModel()
		{

		}

		protected override void OnInitialize()
		{
			LoadDataAsync();
			NotifyOfPropertyChange(() => CanCancelSeasonSelection);
			NotifyOfPropertyChange(() => CanCancelCartoonSelection);
			base.OnInitialize();
		}

		public async void LoadDataAsync()
		{
			WebSites.Clear();

			using (var ctx = new CVDbContext())
			{
				await ctx.WebSites.LoadAsync();
				WebSites.AddRange(ctx.WebSites.Local);
				NotifyOfPropertyChange(() => WebSites);
			}

		}

		#region EventsActions

		public async void WebSitesSelectionChanged()
		{
			Cartoons.Clear();
			Seasons.Clear();

			if (WebSite == null)
			{
				if (CartoonsVisibility == Visibility.Visible)
				{
					CartoonsVisibility = Visibility.Hidden;
					ChangeActiveItem(null);
					return;
				}
			}

			using (var ctx = new CVDbContext())
			{
				await ctx.Cartoons
				         .Include(c => c.WebSites)
				         .LoadAsync();
				
				Cartoons.AddRange(ctx.Cartoons.Local
				                           .Where(c => c.WebSites.Any(w => w.Url == WebSite.Url))
				                           .ToList());
			}

			Cartoons.Add(new Cartoon{Name = NewElementString});
			

			CartoonsVisibility = Visibility.Visible;
		}

		/// <summary>
		/// Изменен выбранный мультфильм
		/// </summary>
		public async void CartoonsSelectionChanged()
		{
			Seasons.Clear();
			NotifyOfPropertyChange(() => CanCancelCartoonSelection);

			if (Cartoon == null)
			{
				if (SeasonsVisibility == Visibility.Visible)
				{
					SeasonsVisibility = Visibility.Hidden;
				}

				ChangeActiveItem(null);
				return;
			}

			if (Cartoon.Name == NewElementString)
			{
				if (SeasonsVisibility == Visibility.Visible)
				{
					SeasonsVisibility = Visibility.Hidden;
				}

				ChangeActiveItem(new CartoonsEditingViewModel(Cartoon));
				return;
			}

			using (var ctx = new CVDbContext())
			{
				await ctx.Seasons
						 .LoadAsync();

				Seasons.AddRange(ctx.Seasons.Local.Where(s => s.CartoonId == _cartoon.CartoonId));
			}

			Seasons.Add(new Season { Name = NewElementString });
			ChangeActiveItem(new CartoonsEditingViewModel(Cartoon));

			SeasonsVisibility = Visibility.Visible;
		}

		/// <summary>
		/// Изменен выбранный сезон
		/// </summary>
		public void SeasonsSelectionChanged()
		{
			NotifyOfPropertyChange(() => CanCancelSeasonSelection);

			if (Season == null)
			{
				ChangeActiveItem(new CartoonsEditingViewModel(Cartoon));
				return;
			}

			ChangeActiveItem(new SeasonsEditingViewModel(Season));
		}

		public void CancelCartoonSelection()
		{
			Cartoon = null;
			NotifyOfPropertyChange(() => CanCancelCartoonSelection);
		}

		public bool CanCancelCartoonSelection => Cartoon != null;

		public void CancelSeasonSelection()
		{
			Season = null;
			NotifyOfPropertyChange(() => CanCancelSeasonSelection);
		}

		public bool CanCancelSeasonSelection => Season != null;

		#endregion

		#region Methods

		public void ChangeActiveItem(Screen viewModel)
		{
			if (((ISettingsViewModel)ActiveItem)?.HasChanges ?? false)
			{
				var result = WinMan.ShowDialog(new DialogViewModel(
												   message: "Сохранить ваши изменения?",
												   currentState: DialogState.YES_NO_CANCEL));

				switch (result)
				{
					case true:
						//SaveChanges
						break;
					case false:
						ActiveItem.TryClose();
						break;
					case null:
						return;
				}
			}
			else
			{
				ActiveItem?.TryClose();

				if (viewModel == null) return;

				ActiveItem = viewModel;
			}
		}

		#endregion

		#region Properties

		public WebSite WebSite
		{
			get => _webSite;
			set
			{
				_webSite = value;
				NotifyOfPropertyChange(() => WebSite);
			}
		}

		public Cartoon Cartoon
		{
			get => _cartoon;
			set
			{
				_cartoon = value;
				NotifyOfPropertyChange(() => Cartoon);
			}
		}

		public Season Season
		{
			get => _season;
			set
			{
				_season = value;
				NotifyOfPropertyChange(() => Season);
			}
		}

		public BindableCollection<WebSite> WebSites
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

		public BindableCollection<Season> Seasons
		{
			get => _seasons;
			set
			{
				_seasons = value;
				NotifyOfPropertyChange(() => Seasons);
			}
		}

		public Visibility CartoonsVisibility
		{
			get => _cartoonsVisibility;
			set
			{
				_cartoonsVisibility = value;
				NotifyOfPropertyChange(() => CartoonsVisibility);
			}
		}

		public Visibility SeasonsVisibility
		{
			get => _seasonsVisibility;
			set
			{
				_seasonsVisibility = value;
				NotifyOfPropertyChange(() => SeasonsVisibility);
			}
		}

		#endregion
	}
}
