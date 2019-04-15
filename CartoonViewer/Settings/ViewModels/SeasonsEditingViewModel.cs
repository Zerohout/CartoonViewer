namespace CartoonViewer.Settings.ViewModels
{
	using System;
	using System.Data.Entity;
	using System.Linq;
	using System.Windows;
	using Caliburn.Micro;
	using Database;
	using Helpers;
	using Models.CartoonModels;
	using Models.SettingModels;
	using static Helpers.Cloner;
	using static Helpers.Helper;

	public class SeasonsEditingViewModel : Screen, ISettingsViewModel
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

		public SeasonsEditingViewModel(CartoonSeason cartoonSeason)
		{
			if (cartoonSeason.Name == NewElementString) return;

			_cartoonSeason = CloneSeason(cartoonSeason);
			_tempCartoonSeason = CloneSeason(cartoonSeason);

			SeasonId = cartoonSeason.CartoonSeasonId;

			LoadData();

			//_episodeTime = ConvertToEpisodeTime(SelectedCartoonEpisode);

		}

		public SeasonsEditingViewModel()
		{
			
		}

		/// <summary>
		/// Загрузка данных
		/// </summary>
		public void LoadData()
		{
			if(SelectedCartoonEpisode == null)
			{
				LoadSeasonData();
			}
			else
			{
				LoadEpisodeData();
			}

			ChangeEpisodeEditingFrame();
		}

		public async void SaveChanges()
		{
			if (SeasonHaveChanges)
			{
				using (var ctx = new CVDbContext())
				{
					ctx.Entry(_cartoonSeason).State = EntityState.Modified;
					await ctx.SaveChangesAsync();
				}

				_tempCartoonSeason = CloneSeason(_cartoonSeason);
			}
		}

		public bool CanSaveChanges
		{
			get
			{
				if (EpisodeEditingVisibility == Visibility.Hidden)
				{
					if (_cartoonSeason.Name == null &&
					    _cartoonSeason.Description == null)
					{
						SeasonHaveChanges = false;
						return false;
					}

					if (_cartoonSeason.Name == _tempCartoonEpisode.Name &&
					    _cartoonSeason.Description == _tempCartoonSeason.Description)
					{
						SeasonHaveChanges = false;
						return false;
					}

					SeasonHaveChanges = true;
					return true;
				}


				return false;
			}
		}

		public void AddEpisode()
		{
			var count = Episodes.Count + 1;

			var defaultEpisode = new CartoonEpisode
			{
				CartoonSeasonId = SeasonId,
				Checked = true,
				DelayedSkip = new TimeSpan(),
				SkipCount = 7,
				CreditsStart = new TimeSpan(0, 21, 30),
				Name = $"Название {count} эпизода",
				Description = $"Описание {count} эпизода",
				Number = count
			};

			using (var ctx = new CVDbContext())
			{
				
			}

			//CartoonEpisodes.Add();
		}

		private TimeSpan CalculatingDuration(CartoonEpisode cartoonEpisode) => 
			cartoonEpisode.CreditsStart - (cartoonEpisode.DelayedSkip + new TimeSpan(0, 0, cartoonEpisode.SkipCount * 5));

		public void EditEpisode()
		{

		}

		public bool CanEditEpisode => SelectedCartoonEpisode != null;

		public void RemoveEpisode()
		{

		}

		public bool CanRemoveEpisode => SelectedCartoonEpisode != null;

		public void CancelSelection()
		{
			SelectedCartoonEpisode = null;
			NotifySeasonList();
		}

		public bool CanCancelSelection => SelectedCartoonEpisode != null;

		private void NotifySeasonList()
		{
			NotifyOfPropertyChange(() => Episodes);
			NotifyOfPropertyChange(() => CanEditEpisode);
			NotifyOfPropertyChange(() => CanRemoveEpisode);
			NotifyOfPropertyChange(() => CanCancelSelection);
		}

		

		#region Private Methods

		/// <summary>
		/// Загрузка данных ссезона
		/// </summary>
		private void LoadSeasonData()
		{
			CartoonSeason result;

			using (var ctx = new CVDbContext())
			{
				ctx.CartoonSeasons
				   .Where(s => s.CartoonSeasonId == SeasonId)
				   .Include(s => s.CartoonEpisodes)
				   .Load();
				result = ctx.CartoonSeasons.Local.FirstOrDefault();
			}

			CartoonSeason = CloneSeason(result);
			TempCartoonSeason = CloneSeason(result);
			Episodes.AddRange(result?.CartoonEpisodes);
		}

		/// <summary>
		/// Загрузка данных эпизодов
		/// </summary>
		private void LoadEpisodeData()
		{
			CartoonEpisode result;

			using (var ctx = new CVDbContext())
			{
				ctx.CartoonEpisodes
				   .Where(e => e.CartoonEpisodeId == EpisodeId)
				   //.Include(e => e.CartoonVoiceOvers)
				   .Load();
				result = ctx.CartoonEpisodes.FirstOrDefault();
			}

			SelectedCartoonEpisode = CloneEpisode(result);
			TempCartoonEpisode = CloneEpisode(result);

			if (((CartoonsControlViewModel) Parent).CartoonWebSite.Url == "http://freehat.cc")
			{
				using (var ctx = new CVDbContext())
				{
					ctx.VoiceOvers.Load();
					VoiceOvers.Clear();
					VoiceOvers.AddRange(CloneVoiceOverList(ctx.VoiceOvers.Local.ToList()));
				}
			}
		}
		
		/// <summary>
		/// Изменение зоны редактирования эпизода
		/// </summary>
		private void ChangeEpisodeEditingFrame()
		{
			if (SelectedCartoonEpisode == null)
			{
				if (EpisodeEditingVisibility == Visibility.Visible)
				{
					EpisodeEditingVisibility = Visibility.Hidden;
				}
				return;
			}

			if (EpisodeEditingVisibility == Visibility.Hidden)
			{
				EpisodeEditingVisibility = Visibility.Visible;
			}

			EpisodeId = SelectedCartoonEpisode.CartoonEpisodeId;
			EpisodeTime = ConvertToEpisodeTime(SelectedCartoonEpisode);
		}

		/// <summary>
		/// Конвертировать значения Эпизода в значения класса EpisodeTime
		/// </summary>
		/// <param name="cartoonEpisode">Эпизод с необходимыми для конвертации данными</param>
		/// <returns></returns>
		private EpisodeTime ConvertToEpisodeTime(CartoonEpisode cartoonEpisode) => new EpisodeTime
		{
			DelayedSkipMinutesString = (cartoonEpisode?.DelayedSkip.Minutes).ToString(),
			DelayedSkipSecondsString = (cartoonEpisode?.DelayedSkip.Seconds).ToString(),
			SkipCountString = cartoonEpisode?.SkipCount.ToString(),
			CreditsStartHoursString = cartoonEpisode?.CreditsStart.Hours.ToString(),
			CreditsStartMinutesString = cartoonEpisode?.CreditsStart.Minutes.ToString(),
			CreditsStartSecondsString = cartoonEpisode?.CreditsStart.Seconds.ToString()
		};

		#endregion

		

		

		#region EventsActions

		public void SelectionChanged()
		{
			ChangeEpisodeEditingFrame();
		}

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

		public Visibility AdvancedSettingsVisibility
		{
			get => Helper.AdvancedSettingsVisibility;
			set => Helper.AdvancedSettingsVisibility = value;
		}

		#endregion



		
	}
}
