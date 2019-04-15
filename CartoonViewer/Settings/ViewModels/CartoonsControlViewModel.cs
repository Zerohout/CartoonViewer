namespace CartoonViewer.Settings.ViewModels
{
	using System.Data.Entity;
	using System.Linq;
	using System.Windows;
	using Caliburn.Micro;
	using CartoonViewer.ViewModels;
	using Database;
	using Models.CartoonModels;
	using static Helpers.Helper;

	public class CartoonsControlViewModel : Conductor<Screen>.Collection.OneActive
	{
		private Visibility _cartoonsVisibility = Visibility.Hidden;
		private Visibility _seasonsVisibility = Visibility.Hidden;
		private CartoonWebSite _cartoonWebSite;
		private Cartoon _cartoon;
		private CartoonSeason _cartoonSeason;
		private BindableCollection<CartoonWebSite> _webSites = new BindableCollection<CartoonWebSite>();
		private BindableCollection<Cartoon> _cartoons = new BindableCollection<Cartoon>();
		private BindableCollection<CartoonSeason> _seasons = new BindableCollection<CartoonSeason>();

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
				await ctx.CartoonWebSites.LoadAsync();
				WebSites.AddRange(ctx.CartoonWebSites.Local);
				NotifyOfPropertyChange(() => WebSites);
			}

		}

		#region EventsActions

		/// <summary>
		/// Изменен выбранный адрес сайта
		/// </summary>
		public async void WebSitesSelectionChanged()
		{
			Cartoons.Clear();
			Seasons.Clear();

			if (CartoonWebSite == null)
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
						 .Include(c => c.CartoonWebSites)
						 .LoadAsync();

				Cartoons.AddRange(ctx.Cartoons.Local
										   .Where(c => c.CartoonWebSites.Any(w => w.Url == CartoonWebSite.Url))
										   .ToList());
			}

			Cartoons.Add(new Cartoon { Name = NewElementString });


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

				ChangeActiveItem(new CartoonsEditingViewModel(Cartoon, CartoonWebSite.CartoonWebSiteId));
				return;
			}

			using (var ctx = new CVDbContext())
			{
				await ctx.CartoonSeasons
						 .LoadAsync();

				Seasons.AddRange(ctx.CartoonSeasons.Local.Where(s => s.CartoonId == _cartoon.CartoonId));
			}

			ChangeActiveItem(new CartoonsEditingViewModel(Cartoon, CartoonWebSite.CartoonWebSiteId));

			SeasonsVisibility = Visibility.Visible;
		}

		/// <summary>
		/// Изменен выбранный сезон
		/// </summary>
		public void SeasonsSelectionChanged()
		{
			NotifyOfPropertyChange(() => CanCancelSeasonSelection);

			if (CartoonSeason == null)
			{
				ChangeActiveItem(new CartoonsEditingViewModel(Cartoon, CartoonWebSite.CartoonWebSiteId));
				return;
			}

			ChangeActiveItem(new SeasonsEditingViewModel(CartoonSeason));
		}

		/// <summary>
		/// Изменить выбранный сезон (проверка на несохраненные изменения)
		/// </summary>
		/// <param name="id"></param>
		public void ChangeSelectedSeason(int id)
		{
			if (((CartoonsEditingViewModel)ActiveItem).HasChanges)
			{
				var result = WinMan.ShowDialog(new DialogViewModel("Сохранить ваши изменения?", DialogState.YES_NO_CANCEL));

				if (result == true)
				{
					((CartoonsEditingViewModel)ActiveItem).SaveChanges();
				}
				else if (result == false)
				{
					var repeatResult = WinMan.ShowDialog(
						new DialogViewModel("Ваши изменения не будут сохранены. Вы точно хотите продолжить?", DialogState.YES_NO));
					if (repeatResult == false || repeatResult == null)
					{
						return;
					}
				}
				else
				{
					return;
				}
			}

			CartoonSeason = Seasons.First(s => s.CartoonSeasonId == id);
		}

		public void CancelCartoonSelection()
		{
			Cartoon = null;
			NotifyOfPropertyChange(() => CanCancelCartoonSelection);
		}

		public bool CanCancelCartoonSelection => Cartoon != null;

		public void CancelSeasonSelection()
		{
			CartoonSeason = null;
			NotifyOfPropertyChange(() => CanCancelSeasonSelection);
		}

		public bool CanCancelSeasonSelection => CartoonSeason != null;

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
						((ISettingsViewModel)ActiveItem).SaveChanges();
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

		public CartoonWebSite CartoonWebSite
		{
			get => _cartoonWebSite;
			set
			{
				_cartoonWebSite = value;
				NotifyOfPropertyChange(() => CartoonWebSite);
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

		public CartoonSeason CartoonSeason
		{
			get => _cartoonSeason;
			set
			{
				_cartoonSeason = value;
				NotifyOfPropertyChange(() => CartoonSeason);
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
