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

	public class EpisodesEditingViewModel : Screen, ISettingsViewModel
	{
		public EpisodesEditingViewModel(Episode episode)
		{
			if (episode.Name == NewElementString) return;

			LoadDataAsync(episode.EpisodeId);
		}

		public EpisodesEditingViewModel()
		{
			_episode = new Episode();
			_tempEpisode = new Episode();
		}

		private BindableCollection<Episode> _episodes = new BindableCollection<Episode>();

		public BindableCollection<Episode> Episodes
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

		private Episode _tempEpisode = new Episode();

		public Episode TempEpisode
		{
			get => _tempEpisode;
			set
			{
				_tempEpisode = value;
				NotifyOfPropertyChange(() => TempEpisode);
			}
		}


		private Episode _episode = new Episode();

		public Episode Episode
		{
			get => _episode;
			set
			{
				_episode = value;
				NotifyOfPropertyChange(() => Episode);
			}
		}

		public bool HasChanges { get; set; }

		public async void LoadDataAsync(int id)
		{
			Episode result;
			using (var ctx = new CVDbContext())
			{
				await ctx.Episodes
				   .Where(e => e.EpisodeId == id)
				   .Include(e => e.VoiceOvers)
				   .LoadAsync();
				result = ctx.Episodes.Local.FirstOrDefault();
			}

			Episode = CopyEpisode(result);
			TempEpisode = CopyEpisode(result);
		}
	}
}
