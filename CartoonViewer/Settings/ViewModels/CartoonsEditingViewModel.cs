namespace CartoonViewer.Settings.ViewModels
{
	using System.Collections.Generic;
	using System.Data.Entity;
	using System.Linq;
	using System.Windows;
	using Caliburn.Micro;
	using Database;
	using Helpers;
	using Models.CartoonModels;
	using static Helpers.Helper;
	using Screen = Caliburn.Micro.Screen;

	public class CartoonsEditingViewModel : Screen, ISettingsViewModel
	{
		private readonly int WebSiteId;
		
		private readonly int CartoonId;
		private string _name;
		private string _url;
		private string _description;

		private Visibility _cartoonTypesVisibility = Visibility.Hidden;
		private CartoonSeason _selectedCartoonSeason;

		private BindableCollection<CartoonSeason> _seasons = new BindableCollection<CartoonSeason>();


		//TODO Изменить значения имени, адреса и описания в свойства класса Cartoon (Cartoon.Name etc.)

		public CartoonsEditingViewModel(Cartoon cartoon,int webSiteId)
		{
			if (cartoon.Name == NewElementString)
			{
				AddCartoonVisibility = Visibility.Visible;
				SaveChangesVisibility = Visibility.Hidden;

				return;
			}

			WebSiteId = webSiteId;
			CartoonId = cartoon.CartoonId;
			LoadData();
			AddCartoonVisibility = Visibility.Hidden;
			SaveChangesVisibility = Visibility.Visible;
		}

		public CartoonsEditingViewModel()
		{
			
		}


		public Visibility AdvancedSettingsVisibility { get; set; }

		public void LoadData()
		{
			Cartoon result;
			using (var ctx = new CVDbContext())
			{
					ctx.Cartoons
				         .Where(c => c.CartoonId == CartoonId)
				         .Include(c => c.CartoonUrls)
				         .Include(c => c.CartoonSeasons)
				         .Load();

				result = ctx.Cartoons.Local.First();
			}

			Seasons.Clear();
			Seasons.AddRange(result.CartoonSeasons);
			Url = result.CartoonUrls.Find(cu => cu.CartoonWebSiteId == WebSiteId).Url;
			Name = result.Name;
			Description = result.Description;
			TempUrl = Url;
			TempName = Name;
			TempDescription = Description;
		}

		private void NotifySeasonList()
		{
			NotifyOfPropertyChange(() => Seasons);
			NotifyOfPropertyChange(() => CanEditSeason);
			NotifyOfPropertyChange(() => CanRemoveSeason);
			NotifyOfPropertyChange(() => CanCancelSelection);
		}

		#region EventsActions

		/// <summary>
		/// Добавить Сезон в список
		/// </summary>
		public void AddSeason()
		{
			var count = Seasons.Count + 1;

			Seasons.Add(new CartoonSeason
			{
				CartoonId = CartoonId,
				Number = count,
				Checked = true,
				CartoonEpisodes = new List<CartoonEpisode>()
			});

			using (var ctx = new CVDbContext())
			{
				ctx.CartoonSeasons.Add(Seasons.Last());
				ctx.SaveChanges();
				Seasons.Last().CartoonSeasonId = ctx.CartoonSeasons.ToList().Last().CartoonSeasonId;
			}

			SelectedCartoonSeason = Seasons.Count > 0
				? Seasons.Last()
				: null;

			NotifySeasonList();
		}

		/// <summary>
		/// Редактировать выбранный сезон
		/// </summary>
		public void EditSeason()
		{
			((CartoonsControlViewModel)Parent).ChangeSelectedSeason(SelectedCartoonSeason.CartoonSeasonId);
		}

		public bool CanEditSeason => SelectedCartoonSeason != null;

		/// <summary>
		/// Удалить выбранный сезон
		/// </summary>
		public void RemoveSeason()
		{
			using (var ctx = new CVDbContext())
			{
				var temp = ctx.CartoonSeasons.Find(SelectedCartoonSeason.CartoonSeasonId);
				ctx.Entry(temp).State = EntityState.Deleted;
				ctx.SaveChanges();
			}

			Seasons.Remove(SelectedCartoonSeason);

			SelectedCartoonSeason = Seasons.Count > 0
				? Seasons.Last()
				: null;

			NotifySeasonList();
		}

		public bool CanRemoveSeason => SelectedCartoonSeason != null;

		/// <summary>
		/// Отменить выделение сезона
		/// </summary>
		public void CancelSelection()
		{
			SelectedCartoonSeason = null;
			NotifySeasonList();
		}

		public bool CanCancelSelection => SelectedCartoonSeason != null;

		/// <summary>
		/// Сохранить изменения
		/// </summary>
		public async void SaveChanges()
		{
			using (var ctx = new CVDbContext())
			{
				var temp = ctx.Cartoons.Find(CartoonId);
				temp.CartoonUrls.Find(cu => cu.CartoonWebSiteId == WebSiteId).Url = Url;
				temp.Name = Name;
				temp.Description = Description;
				ctx.Entry(temp).State = EntityState.Modified;
				await ctx.SaveChangesAsync();
			}

			TempUrl = Url;
			TempName = Name;
			TempDescription = Description;

			NotifyOfPropertyChange(() => CanSaveChanges);
			NotifyOfPropertyChange(() => HasChanges);
		}

		public bool CanSaveChanges
		{
			get
			{
				if ((string.IsNullOrWhiteSpace(Url) || string.IsNullOrWhiteSpace(Name)) ||
				    (TempUrl == Url && TempName == Name && TempDescription == Description))
				{
					return false;
				}

				return true;
			}
		}



		/// <summary>
		/// Действие при изменении текста
		/// </summary>
		public void TextChanged()
		{
			var t = Description;
			var tt = TempDescription;
			NotifyOfPropertyChange(() => CanSaveChanges);
			NotifyOfPropertyChange(() => HasChanges);
		}

		/// <summary>
		/// Действие при изменении выбора сезона
		/// </summary>
		public void SelectionChanged()
		{
			NotifySeasonList();
		}

		#endregion

		#region Properties

		public Visibility SaveChangesVisibility { get; set; } = Visibility.Hidden;
		public Visibility AddCartoonVisibility { get; set; } = Visibility.Hidden;
		public BindableCollection<string> CartoonTypes { get; set; } = Helper.CartoonTypes;


		public bool HasChanges
		{
			get
			{
				if ((string.IsNullOrWhiteSpace(Url) || string.IsNullOrWhiteSpace(Name)) ||
				    TempUrl == Url && TempName == Name && TempDescription == Description)
				{
					return false;
				}

				return true;
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

		public CartoonSeason SelectedCartoonSeason
		{
			get => _selectedCartoonSeason;
			set
			{
				_selectedCartoonSeason = value;
				NotifyOfPropertyChange(() => SelectedCartoonSeason);
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
