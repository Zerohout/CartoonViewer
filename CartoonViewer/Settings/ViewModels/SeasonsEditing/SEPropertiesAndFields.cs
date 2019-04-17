namespace CartoonViewer.Settings.ViewModels
{
	using System;
	using System.Windows;
	using Caliburn.Micro;
	using Models.CartoonModels;
	using Models.SettingModels;

	public partial class SeasonsEditingViewModel : Screen, ISettingsViewModel
	{
		#region Private fields

		private BindableCollection<CartoonSeason> _seasons = new BindableCollection<CartoonSeason>();
		private readonly int SeasonId;
		private CartoonSeason _cartoonSeason = new CartoonSeason();
		private CartoonSeason _tempCartoonSeason = new CartoonSeason();
		private bool SeasonHaveChanges;
		private BindableCollection<CartoonEpisode> _episodes = new BindableCollection<CartoonEpisode>();
		private int EpisodeId;
		private CartoonEpisode _selectedCartoonEpisode;
		private CartoonEpisode _tempCartoonEpisode;
		private EpisodeTime _episodeTime;
		private Visibility _episodeEditingVisibility = Visibility.Hidden;
		private BindableCollection<CartoonVoiceOver> _voiceOvers = new BindableCollection<CartoonVoiceOver>();
		private Visibility _voiceOversVisibility = Visibility.Hidden;

		#endregion

		#region Properties

		public Visibility EpisodeEditingVisibility
		{
			get => _episodeEditingVisibility;
			set
			{
				_episodeEditingVisibility = value;
				NotifyOfPropertyChange(() => EpisodeEditingVisibility);
			}
		}

		public Visibility VoiceOversVisibility
		{
			get => _voiceOversVisibility;
			set
			{
				_voiceOversVisibility = value;
				NotifyOfPropertyChange(() => VoiceOversVisibility);
			}
		}

		public BindableCollection<CartoonVoiceOver> VoiceOvers
		{
			get => _voiceOvers;
			set
			{
				_voiceOvers = value;
				NotifyOfPropertyChange(() => VoiceOvers);
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

		public CartoonSeason CartoonSeason
		{
			get => _cartoonSeason;
			set
			{
				_cartoonSeason = value;
				NotifyOfPropertyChange(() => CartoonSeason);
			}
		}

		public CartoonSeason TempCartoonSeason
		{
			get => _tempCartoonSeason;
			set
			{
				_tempCartoonSeason = value;
				NotifyOfPropertyChange(() => TempCartoonSeason);
			}
		}

		public BindableCollection<CartoonEpisode> Episodes
		{
			get => _episodes;
			set
			{
				_episodes = value;
				NotifyOfPropertyChange(() => Episodes);
			}
		}

		public CartoonEpisode SelectedCartoonEpisode
		{
			get => _selectedCartoonEpisode;
			set
			{
				_selectedCartoonEpisode = value;
				NotifyOfPropertyChange(() => SelectedCartoonEpisode);
			}
		}

		public CartoonEpisode TempCartoonEpisode
		{
			get => _tempCartoonEpisode;
			set
			{
				_tempCartoonEpisode = value;
				NotifyOfPropertyChange(() => TempCartoonEpisode);
			}
		}

		public EpisodeTime EpisodeTime
		{
			get => _episodeTime;
			set
			{
				_episodeTime = value;
				NotifyOfPropertyChange(() => EpisodeTime);
			}
		}

		public TimeSpan EpisodeDuration =>
			SelectedCartoonEpisode.CreditsStart - (SelectedCartoonEpisode.DelayedSkip + new TimeSpan(0, 0, SelectedCartoonEpisode.SkipCount * 5));

		public bool HasChanges { get; set; }


		#endregion
	}
}
