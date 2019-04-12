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
	using static Helpers.Cloner;
	using static Helpers.Helper;
	using Screen = Caliburn.Micro.Screen;

	public class CartoonsEditingViewModel : Screen, ISettingsViewModel
	{
		private BindableCollection<Cartoon> _cartoons = new BindableCollection<Cartoon>();
		private Cartoon _cartoon = new Cartoon();

		private Visibility _cartoonTypesVisibility = Visibility.Hidden;
		private Season _selectedSeason;

		private BindableCollection<Season> _seasons = new BindableCollection<Season>();
		public BindableCollection<Season> Seasons
		{
			get => _seasons;
			//new BindableCollection<Season>(Cartoon.Seasons);
			set
			{
				_seasons = value;
				//Cartoon.Seasons.Clear();
				//Cartoon.Seasons.AddRange(value);
				NotifyOfPropertyChange(() => Seasons);
			}
		}


		public Season SelectedSeason
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
		
		private readonly int WebSiteId;
		private readonly int CartoonId;

		public BindableCollection<string> CartoonTypes { get; set; } = Helper.CartoonTypes;
		private string _url;

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

		private string _name;

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

		private string _description;

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
				         .Include(c => c.Seasons)
				         .Load();

				result = ctx.Cartoons.Local.First();
			}

			Seasons.Clear();
			Seasons.AddRange(result.Seasons);
			Url = result.CartoonUrls.Find(cu => cu.WebSiteId == WebSiteId).Url;
			Name = result.Name;
			Description = result.Description;
			TempUrl = Url;
			TempName = Name;
			TempDescription = Description;

			//Cartoon = CloneCartoon(result);
		}

		private void NotifySeasonList()
		{
			NotifyOfPropertyChange(() => Seasons);
			NotifyOfPropertyChange(() => CanAddSeason);
			NotifyOfPropertyChange(() => CanEditSeason);
			NotifyOfPropertyChange(() => CanRemoveSeason);
			NotifyOfPropertyChange(() => CanCancelSelection);
		}

		#region EventsActions

		public void AddSeason()
		{
			var count = Seasons.Last().Number + 1;

			Seasons.Add(new Season
			{
				CartoonId = CartoonId,
				Number = count,
				Checked = true,
				Episodes = new List<Episode>()
			});

			using (var ctx = new CVDbContext())
			{
				ctx.Seasons.Add(Seasons.Last());
				ctx.SaveChanges();
				Seasons.Last().SeasonId = ctx.Seasons.ToList().Last().SeasonId;
			}

			SelectedSeason = Seasons.Count > 0
				? Seasons.Last()
				: null;

			NotifySeasonList();
		}

		public bool CanAddSeason => true;

		public void EditSeason()
		{
			((CartoonsControlViewModel)Parent).ChangeSelectedSeason(SelectedSeason.SeasonId);
		}

		public bool CanEditSeason => SelectedSeason != null;

		public void RemoveSeason()
		{
			using (var ctx = new CVDbContext())
			{
				var temp = ctx.Seasons.Find(SelectedSeason.SeasonId);
				ctx.Entry(temp).State = EntityState.Deleted;
				ctx.SaveChanges();
			}

			Seasons.Remove(SelectedSeason);

			SelectedSeason = Seasons.Count > 0
				? Seasons.Last()
				: null;

			NotifySeasonList();
		}

		public bool CanRemoveSeason => SelectedSeason != null;

		public void CancelSelection()
		{
			SelectedSeason = null;
			NotifySeasonList();
		}

		public bool CanCancelSelection => SelectedSeason != null;

		

		public async void SaveChanges()
		{
			//Cartoon.CartoonUrls.Find(cu => cu.WebSiteId == WebSiteId).Url = Url;
			using (var ctx = new CVDbContext())
			{
				var temp = ctx.Cartoons.Find(CartoonId);
				temp.CartoonUrls.Find(cu => cu.WebSiteId == WebSiteId).Url = Url;
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

		public void TextChanged()
		{
			var t = Description;
			var tt = TempDescription;
			NotifyOfPropertyChange(() => CanSaveChanges);
			NotifyOfPropertyChange(() => HasChanges);
		}

		public void SelectionChanged()
		{
			NotifySeasonList();
		}

		#endregion

		#region Properties

		public Visibility SaveChangesVisibility { get; set; } = Visibility.Hidden;
		public Visibility AddCartoonVisibility { get; set; } = Visibility.Hidden;

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

		public BindableCollection<Cartoon> Cartoons
		{
			get => _cartoons;
			set
			{
				_cartoons = value;
				NotifyOfPropertyChange(() => Cartoons);
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

		//public Cartoon TempCartoon
		//{
		//	get => _tempCartoon;
		//	set
		//	{
		//		_tempCartoon = value;
		//		NotifyOfPropertyChange(() => TempCartoon);
		//	}
		//}

		

		#endregion

	}
}
