namespace CartoonViewer.Settings.ViewModels
{
	using System.Data.Entity;
	using System.Linq;
	using Caliburn.Micro;
	using Database;
	using Models.CartoonModels;

	public partial class VoiceOversEditingViewModel : Screen
	{
		#region Private Methods

		private void ChangeData()
		{
			if(SelectedCartoon == null && SelectedSeason != null)
			{

				SelectedCartoonVoiceOver = null;
				CartoonVoiceOvers = new BindableCollection<CartoonVoiceOver>();

				SelectedSeason = null;
				CartoonSeasons = new BindableCollection<CartoonSeason>();
				NotifyOfPropertyChange(() => SeasonsAndCartoonVoiceOversVisibility);

				return;
			}

			if(SelectedSeason == null && SelectedEpisode != null)
			{
				SelectedEpisode = null;
				CartoonEpisodes = new BindableCollection<CartoonEpisode>();
				NotifyOfPropertyChange(() => EpisodesVisibility);

				return;
			}

			if(SelectedEpisode == null)
			{
				SelectedEpisodeVoiceOver = null;
				EpisodeVoiceOvers = new BindableCollection<CartoonVoiceOver>();
				NotifyOfPropertyChange(() => EpisodeVoiceOversVisibility);

				return;
			}
		}

		/// <summary>
		/// Загрузка из базы данных всех необходимых данных
		/// </summary>
		private void LoadData()
		{
			// --Начальная, когда ни один мульфильм не выбран
			if(SelectedCartoon == null)
			{
				LoadCartoonsData();

				var id = IdList.CartoonId;

				// --При загруженных с конструктора данных мультфильма
				if(id > 0)
				{
					SelectedCartoon = Cartoons.First(c => c.CartoonId == id);
					LoadData();
				}

				return;
			}

			// --При выборе мультфильма
			if(SelectedSeason == null)
			{
				LoadSelectedCartoonVoiceOverData();
				LoadSelectedCartoonSeasonsData();

				var id = IdList.SeasonId;

				if(id > 0)
				{
					SelectedSeason = CartoonSeasons.First(c => c.CartoonSeasonId == id);
					LoadData();
				}

				return;
			}

			// --При выборе сезона
			if(SelectedEpisode == null)
			{
				LoadSelectedSeasonEpisodesData();

				var id = IdList.EpisodeId;

				if(id > 0)
				{
					SelectedEpisode = CartoonEpisodes.First(ce => ce.CartoonEpisodeId == id);
					LoadData();
				}
				return;
			}

			// --При выборе эпизода
			LoadSelectedEpisodeVoiceOversData();
		}

		/// <summary>
		/// Загрузка из базы данных списка мультфильмов по ID сайта
		/// </summary>
		private void LoadCartoonsData()
		{
			BindableCollection<Cartoon> cartoons;

			using(var ctx = new CVDbContext())
			{
				ctx.Cartoons
				   .Where(c => c.CartoonWebSites
								.Any(cws => cws.CartoonWebSiteId == IdList.WebSiteId))
				   .Load();
				cartoons = new BindableCollection<Cartoon>(ctx.Cartoons.Local);
			}

			Cartoons = new BindableCollection<Cartoon>(cartoons);
		}

		/// <summary>
		/// Загрузка озвучек выбранного мультфильма
		/// </summary>
		private void LoadSelectedCartoonVoiceOverData()
		{
			BindableCollection<CartoonVoiceOver> voiceOvers;

			using(var ctx = new CVDbContext())
			{
				ctx.VoiceOvers
				   .Where(vo => vo.Cartoons
								  .Any(c => c.CartoonId == IdList.CartoonId))
				   .Load();
				voiceOvers = new BindableCollection<CartoonVoiceOver>(ctx.VoiceOvers.Local);
			}

			CartoonVoiceOvers = new BindableCollection<CartoonVoiceOver>(voiceOvers);
		}

		/// <summary>
		/// Загрузка из базы данны списка сезонов выбранного мультфильма
		/// </summary>
		private void LoadSelectedCartoonSeasonsData()
		{
			BindableCollection<CartoonSeason> seasons;

			using(var ctx = new CVDbContext())
			{
				ctx.CartoonSeasons
				   .Where(cs => cs.CartoonId == IdList.CartoonId)
				   .Load();
				seasons = new BindableCollection<CartoonSeason>(ctx.CartoonSeasons.Local);
			}

			CartoonSeasons = new BindableCollection<CartoonSeason>(seasons);
		}

		/// <summary>
		/// Загрузка из базы данных списка эпизодов выбранного сезона
		/// </summary>
		private void LoadSelectedSeasonEpisodesData()
		{
			BindableCollection<CartoonEpisode> episodes;

			using(var ctx = new CVDbContext())
			{
				ctx.CartoonEpisodes
				   .Where(ce => ce.CartoonSeasonId == IdList.SeasonId)
				   .Load();
				episodes = new BindableCollection<CartoonEpisode>(ctx.CartoonEpisodes.Local);
			}

			CartoonEpisodes.Clear();
			CartoonEpisodes.AddRange(episodes);
		}

		/// <summary>
		/// Загрузка из базы данных списка озвучек выбранного эпизода
		/// </summary>
		private void LoadSelectedEpisodeVoiceOversData()
		{
			BindableCollection<CartoonVoiceOver> voiceOvers;

			using(var ctx = new CVDbContext())
			{
				ctx.VoiceOvers
				   .Where(vo => vo.CartoonEpisodes
								  .Any(ce => ce.CartoonEpisodeId == IdList.EpisodeId))
				   .Load();
				voiceOvers = new BindableCollection<CartoonVoiceOver>(ctx.VoiceOvers.Local);
			}

			EpisodeVoiceOvers = new BindableCollection<CartoonVoiceOver>(voiceOvers);
		}

		#endregion
	}
}
