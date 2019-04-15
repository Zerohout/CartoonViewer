namespace CartoonViewer.Settings.ViewModels
{
	using System.Data.Entity;
	using System.Linq;
	using System.Windows;
	using Caliburn.Micro;
	using Database;
	using Helpers;
	using Models.CartoonModels;
	using static Helpers.Cloner;
	using static Helpers.Helper;

	public class EpisodesEditingViewModel : Screen, ISettingsViewModel
	{
		public EpisodesEditingViewModel(CartoonEpisode cartoonEpisode)
		{
			if (cartoonEpisode.Name == NewElementString) return;

			LoadData();
		}

		public EpisodesEditingViewModel()
		{
			_cartoonEpisode = new CartoonEpisode();
			_tempCartoonEpisode = new CartoonEpisode();
		}

		private BindableCollection<CartoonEpisode> _episodes = new BindableCollection<CartoonEpisode>();

		public BindableCollection<CartoonEpisode> Episodes
		{
			get => _episodes;
			set
			{
				_episodes = value;
				NotifyOfPropertyChange(() => Episodes);
			}
		}

		public Visibility AdvancedSettingsVisibility
		{
			get => Helper.AdvancedSettingsVisibility;
			set => Helper.AdvancedSettingsVisibility = value;
		}

		private CartoonEpisode _tempCartoonEpisode = new CartoonEpisode();

		public CartoonEpisode TempCartoonEpisode
		{
			get => _tempCartoonEpisode;
			set
			{
				_tempCartoonEpisode = value;
				NotifyOfPropertyChange(() => TempCartoonEpisode);
			}
		}


		private CartoonEpisode _cartoonEpisode = new CartoonEpisode();

		public CartoonEpisode CartoonEpisode
		{
			get => _cartoonEpisode;
			set
			{
				_cartoonEpisode = value;
				NotifyOfPropertyChange(() => CartoonEpisode);
			}
		}

		public bool HasChanges { get; set; }

		public void LoadData()
		{
			//CartoonEpisode result;
			using (var ctx = new CVDbContext())
			{
				//await ctx.CartoonEpisodes
				//   .Where(e => e.CartoonEpisodeId == id)
				//   .Include(e => e.CartoonVoiceOvers)
				//   .LoadAsync();
				//result = ctx.CartoonEpisodes.Local.FirstOrDefault();
			}

			//CartoonEpisode = CloneEpisode(result);
			//TempCartoonEpisode = CloneEpisode(result);
		}

		public void SaveChanges() { throw new System.NotImplementedException(); }
	}
}
