namespace CartoonViewer.Settings.ViewModels
{
	using System.Data.Entity;
	using System.Linq;
	using System.Xml.Serialization;
	using Caliburn.Micro;
	using Database;
	using Models.CartoonModels;

	public partial class VoiceOversEditingViewModel : Screen
	{
		#region Private Methods
		/// <summary>
		/// Изменить выбранный м/ф и все связанные данные
		/// </summary>
		/// <param name="value">Конечное значение м/ф</param>
		private void ChangeSelectedCartoon(Cartoon value)
		{
			if(IsDesignTime)
			{
				_selectedCartoon = value;
				NotifyOfPropertyChange(() => SelectedCartoon);
				return;
			}

			if(_selectedCartoon == value)
				return;

			IdList.CartoonId = value?.CartoonId ?? 0;
			SelectedCartoonVoiceOver = null;
			ChangeSelectedSeason(null);


			if(value == null)
			{
				_selectedCartoon = null;
				NotifyOfPropertyChange(() => SelectedCartoon);
			}
			else
			{
				LoadData();
			}
			NotifyOfPropertyChange(() => CanCancelCartoonVoiceOverSelection);
			NotifyOfPropertyChange(() => SeasonsAndCartoonVoiceOversVisibility);
		}
		/// <summary>
		/// Изменить выбранный сезон и все связные данные
		/// </summary>
		/// <param name="value">Конечное значение сезона</param>
		private void ChangeSelectedSeason(CartoonSeason value)
		{
			if(IsDesignTime)
			{
				_selectedSeason = value;
				NotifyOfPropertyChange(() => SelectedSeason);
				return;
			}

			if(_selectedSeason == value)
				return;

			IdList.SeasonId = value?.CartoonSeasonId ?? 0;
			ChangeSelectedEpisode(null);

			if(value == null)
			{
				_selectedSeason = null;
				NotifyOfPropertyChange(() => SelectedSeason);
			}
			else
			{
				LoadData();
			}
			NotifyOfPropertyChange(() => EpisodesVisibility);
			NotifyOfPropertyChange(() => CanCancelSeasonSelection);
		}
		/// <summary>
		/// Изменить выбранный эпизод и все связанные данные
		/// </summary>
		/// <param name="value">Конечное значение эпизода</param>
		private void ChangeSelectedEpisode(CartoonEpisode value)
		{
			if(IsDesignTime)
			{
				_selectedEpisode = value;
				NotifyOfPropertyChange(() => SelectedEpisode);
				return;
			}

			if(_selectedEpisode == value)
				return;

			IdList.EpisodeId = value?.CartoonEpisodeId ?? 0;
			SelectedEpisodeVoiceOver = null;

			if(value == null)
			{
				_selectedEpisode = null;
				NotifyOfPropertyChange(() => SelectedEpisode);
			}
			else
			{
				LoadData();
			}
			NotifyOfPropertyChange(() => EpisodeVoiceOversVisibility);
			NotifyOfPropertyChange(() => CanCancelEpisodeSelection);
		}



		/// <summary>
		/// Загрузка из базы данных всех необходимых данных
		/// </summary>
		private void LoadData()
		{
			// --Начальная, когда ни один мульфильм не выбран
			if(_selectedCartoon == null)
			{
				LoadCartoonsData();

				// --При загруженных с конструктора данных мультфильма
				if(IdList.CartoonId > 0)
				{
					_selectedCartoon = _cartoons.First(c => c.CartoonId == IdList.CartoonId);
					NotifyOfPropertyChange(() => SelectedCartoon);
					LoadData();
				}

				return;
			}

			// --При выборе мультфильма
			if(_selectedSeason == null)
			{
				LoadSelectedCartoonVoiceOverData();
				LoadSelectedCartoonSeasonsData();

				_selectedCartoon = _cartoons.First(c => c.CartoonId == IdList.CartoonId);
				NotifyOfPropertyChange(() => SelectedCartoon);

				if(IdList.SeasonId > 0)
				{
					_selectedSeason = _seasons.First(cs => cs.CartoonSeasonId == IdList.SeasonId);
					NotifyOfPropertyChange(() => SelectedSeason);

					LoadData();
				}

				return;
			}

			// --При выборе сезона
			if(_selectedEpisode == null)
			{
				LoadSelectedSeasonEpisodesData();

				_selectedSeason = _seasons.First(cs => cs.CartoonSeasonId == IdList.SeasonId);
				NotifyOfPropertyChange(() => SelectedSeason);

				if(IdList.EpisodeId > 0)
				{
					_selectedEpisode = _episodes.First(ce => ce.CartoonEpisodeId == IdList.EpisodeId);
					NotifyOfPropertyChange(() => SelectedEpisode);
					LoadData();
				}
				return;
			}

			// --При выборе эпизода
			LoadSelectedEpisodeVoiceOversData();
			_selectedEpisode = _episodes.First(ce => ce.CartoonEpisodeId == IdList.EpisodeId);
			NotifyOfPropertyChange(() => SelectedEpisode);
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

			Seasons = new BindableCollection<CartoonSeason>(seasons);
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

			Episodes.Clear();
			Episodes.AddRange(episodes);
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
		/// <summary>
		/// Оповестить кнопки списка озвучек м/ф об изменениях
		/// </summary>
		private void NotifyCartoonVoiceOversButtons()
		{
			NotifyOfPropertyChange(() => CanAddCartoonVoiceOver);
			NotifyOfPropertyChange(() => CanEditCartoonVoiceOver);
			NotifyOfPropertyChange(() => CanRemoveCartoonVoiceOver);
			NotifyOfPropertyChange(() => CanCancelCartoonVoiceOverSelection);
			NotifyOfPropertyChange(() => CanMoveToEpisodeVoiceOvers);
		}
		/// <summary>
		/// Оповестить кнопки списка озвучек эпизода об ихменениях
		/// </summary>
		private void NotifyEpisodeVoiceOversButtons()
		{
			NotifyOfPropertyChange(() => CanRemoveFromEpisodeVoiceOvers);
			NotifyOfPropertyChange(() => CanCancelEpisodeVoiceOverSelection);
		}
		/// <summary>
		/// Оповестить значения связанные с изменением редактируемой озвучки м/ф
		/// </summary>
		private void NotifyChanges()
		{
			NotifyOfPropertyChange(() => HasChanges);
			NotifyOfPropertyChange(() => CanSaveChanges);
			NotifyOfPropertyChange(() => CanCancelChanges);
			NotifyOfPropertyChange(() => CanUnlockEditingInterface);
		}
		/// <summary>
		/// Оповестить кнопки снятия выделения с выбранного объекта
		/// </summary>
		private void NotifyCancelButtons()
		{
			NotifyOfPropertyChange(() => CanCancelCartoonSelection);
			NotifyOfPropertyChange(() => CanCancelEpisodeSelection);
			NotifyOfPropertyChange(() => CanCancelSeasonSelection);
		}

		#endregion
	}
}
