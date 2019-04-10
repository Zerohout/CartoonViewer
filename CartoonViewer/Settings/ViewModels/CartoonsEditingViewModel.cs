namespace CartoonViewer.Settings.ViewModels
{
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
		private BindableCollection<Cartoon> _cartoons = new BindableCollection<Cartoon>();
		private Cartoon _cartoon = new Cartoon();
		private Cartoon _tempCartoon = new Cartoon();
		
		public Visibility SaveChangesVisibility { get; set; } = Visibility.Hidden;
		public Visibility AddCartoonVisibility { get; set; } = Visibility.Hidden;

		public CartoonsEditingViewModel(Cartoon cartoon)
		{
			if (cartoon.Name == NewElementString)
			{
				AddCartoonVisibility = Visibility.Visible;
				SaveChangesVisibility = Visibility.Hidden;
				return;
			}

			LoadDataAsync(cartoon.CartoonId);
			AddCartoonVisibility = Visibility.Hidden;
			SaveChangesVisibility = Visibility.Visible;
		}

		public CartoonsEditingViewModel()
		{
			
		}

		public Visibility AdvancedSettingsVisibility
		{
			get => Helper.AdvancedSettingsVisibility;

			set => Helper.AdvancedSettingsVisibility = value;
		}

		public async void LoadDataAsync(int id)
		{
			Cartoon result;
			using (var ctx = new CVDbContext())
			{
				await ctx.Cartoons
				   .Where(c => c.CartoonId == id)
				   //.Include(c => c.ElementValues)
				   //.Include(c => c.CartoonUrl)
				   .Include(c => c.Seasons)
				   .LoadAsync();
				result = ctx.Cartoons.Local.FirstOrDefault();
			}

			Cartoon = CopyCartoon(result);
			TempCartoon = CopyCartoon(result);
		}

		#region EventsActions

		public async void SaveChanges()
		{
			using (var ctx = new CVDbContext())
			{
				ctx.Entry(Cartoon).State = EntityState.Modified;
				await ctx.SaveChangesAsync();
			}

			NotifyOfPropertyChange(() => CanSaveChanges);
		}

		public bool CanSaveChanges => false;

		public async void AddCartoon()
		{
			using (var ctx = new CVDbContext())
			{
				ctx.Cartoons.Add(Cartoon);
				await ctx.SaveChangesAsync();
			}

			NotifyOfPropertyChange(() => CanAddCartoon);
		}

		public bool CanAddCartoon => false;

		#endregion

		#region Properties

		public bool HasChanges { get; set; }

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

		public Cartoon TempCartoon
		{
			get => _tempCartoon;
			set
			{
				_tempCartoon = value;
				NotifyOfPropertyChange(() => TempCartoon);
			}
		}

		

		#endregion

	}
}
