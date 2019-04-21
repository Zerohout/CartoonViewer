namespace CartoonViewer.Settings.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.Data.Entity;
	using System.Linq;
	using System.Threading.Tasks;
	using Caliburn.Micro;
	using Database;
	using Models.CartoonModels;

	public partial class VoiceOversEditingViewModel : Screen
	{

		#region Global voice overs methods

		/// <summary>
		/// Загрузить список глобальных озвучек из БД
		/// </summary>
		private void LoadGlobalVoiceOverList()
		{
			List<CartoonVoiceOver> voiceOvers;
			using(var ctx = new CVDbContext())
			{
				voiceOvers = ctx.VoiceOvers.ToList();
			}

			GlobalVoiceOvers = new BindableCollection<CartoonVoiceOver>(voiceOvers);
		}

		/// <summary>
		/// Удаление глобальной озвучки из БД
		/// </summary>
		private async void RemoveSelectedGlobalVoiceOverFromDb()
		{
			using(var ctx = new CVDbContext())
			{
				var voiceOver = await ctx.VoiceOvers.FindAsync(SelectedVoiceOverId);

				if(voiceOver == null)
				{
					throw new Exception("Выбранная глобальная озвучка не найдена в БД.");
				}

				ctx.VoiceOvers.Remove(voiceOver);
				await ctx.SaveChangesAsync();
			}
		}

		/// <summary>
		/// Удалить выбранную озвучку из списка глобальных озвучек
		/// </summary>
		private void RemoveVoiceOverFromGlobalList()
		{
			var globalVoiceOver = GlobalVoiceOvers
				.First(gvo => gvo.CartoonVoiceOverId == SelectedVoiceOverId);

			GlobalVoiceOvers.Remove(globalVoiceOver);
			NotifyOfPropertyChange(() => CartoonVoiceOvers);
		}

		#endregion

		#region Cartoon voice overs methods

		/// <summary>
		/// Загрузка озвучек выбранного мультфильма из БД
		/// </summary>
		private void LoadCartoonVoiceOverList()
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
		/// Удалить  озвучку выбранного м/ф из БД
		/// </summary>
		private async void RemoveSelectedCartoonVoiceOverFromDb()
		{
			using(var ctx = new CVDbContext())
			{
				var cartoon = await ctx.Cartoons
									   .Include(ce => ce.CartoonVoiceOvers)
									   .SingleAsync(ce => ce.CartoonId == IdList.CartoonId);

				ctx.VoiceOvers
				   .Include(vo => vo.Cartoons)
				   .Single(vo => vo.CartoonVoiceOverId == SelectedVoiceOverId)
				   .Cartoons.Remove(cartoon);
				await ctx.SaveChangesAsync();
			}
		}

		/// <summary>
		/// Удалить выбранную озвучку из списка текущего м/ф
		/// </summary>
		private void RemoveVoiceOverFromCartoonList()
		{
			var cartoonVoiceOver = CartoonVoiceOvers
				.First(cvo => cvo.CartoonVoiceOverId == SelectedVoiceOverId);

			CartoonVoiceOvers.Remove(cartoonVoiceOver);
			NotifyOfPropertyChange(() => EpisodeVoiceOvers);
		}

		#endregion

		#region Episode voice overs methods

		/// <summary>
		/// Загрузка списка озвучек выбранного эпизода из БД
		/// </summary>
		private void LoadEpisodeVoiceOverList()
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
		/// Удалить выбранную озвучку текущего эпизода из БД
		/// </summary>
		private async void RemoveSelectedEpisodeVoiceOverFromDb()
		{
			using(var ctx = new CVDbContext())
			{
				var episode = await ctx.CartoonEpisodes
									   .Include(ce => ce.EpisodeVoiceOvers)
									   .SingleAsync(ce => ce.CartoonEpisodeId == IdList.EpisodeId);

				ctx.VoiceOvers
				   .Include(vo => vo.CartoonEpisodes)
				   .Single(vo => vo.CartoonVoiceOverId == SelectedVoiceOverId)
				   .CartoonEpisodes.Remove(episode);
				await ctx.SaveChangesAsync();
			}
		}

		/// <summary>
		/// Удалить выбранную озвучку из списка текущего эпизода
		/// </summary>
		private void RemoveVoiceOverFromEpisodeList()
		{
			var episodeVoiceOver = EpisodeVoiceOvers
				.First(cvo => cvo.CartoonVoiceOverId == SelectedVoiceOverId);

			EpisodeVoiceOvers.Remove(episodeVoiceOver);
			NotifyOfPropertyChange(() => EpisodeVoiceOvers);
		}

		#endregion

		#region Cartoons methods

		/// <summary>
		/// Загрузка из базы данных списка мультфильмов по ID сайта
		/// </summary>
		private void LoadCartoonList()
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
				NotifyCartoonData();
			}
			else
			{
				LoadData();
			}
		}

		#endregion

		#region Seasons methods

		/// <summary>
		/// Загрузка из базы данны списка сезонов выбранного мультфильма
		/// </summary>
		private void LoadSeasonList()
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
				NotifySeasonData();
			}
			else
			{
				LoadData();
			}
		}

		#endregion

		#region Episode methods

		/// <summary>
		/// Загрузка из базы данных списка эпизодов выбранного сезона
		/// </summary>
		private void LoadEpisodeList()
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
				NotifyEpisodeData();
			}
			else
			{
				LoadData();
			}
		}

		#endregion

		#region General methods

		/// <summary>
		/// Загрузка из базы данных всех необходимых данных
		/// </summary>
		private void LoadData()
		{
			// --Начальная, когда ни один мульфильм не выбран
			if(_selectedCartoon == null)
			{
				LoadCartoonList();

				// --При загруженных с конструктора данных мультфильма
				if(IdList.CartoonId > 0)
				{
					_selectedCartoon = _cartoons.First(c => c.CartoonId == IdList.CartoonId);
					LoadData();
					NotifyCartoonData();
				}

				return;
			}

			// --При выборе мультфильма
			if(_selectedSeason == null)
			{
				LoadCartoonVoiceOverList();
				LoadSeasonList();

				_selectedCartoon = _cartoons.First(c => c.CartoonId == IdList.CartoonId);
				NotifyCartoonData();

				if(IdList.SeasonId > 0)
				{
					_selectedSeason = _seasons.First(cs => cs.CartoonSeasonId == IdList.SeasonId);
					LoadData();
					NotifySeasonData();
				}

				return;
			}

			// --При выборе сезона
			if(_selectedEpisode == null)
			{
				LoadEpisodeList();

				_selectedSeason = _seasons.First(cs => cs.CartoonSeasonId == IdList.SeasonId);
				NotifySeasonData();

				if(IdList.EpisodeId > 0)
				{
					_selectedEpisode = _episodes.First(ce => ce.CartoonEpisodeId == IdList.EpisodeId);
					LoadData();
					NotifyEpisodeData();
				}
				return;
			}

			// --При выборе эпизода
			LoadEpisodeVoiceOverList();
			_selectedEpisode = _episodes.First(ce => ce.CartoonEpisodeId == IdList.EpisodeId);
			NotifyEpisodeData();
		}

		/// <summary>
		/// Задать новое значение выбранной озвучке
		/// </summary>
		private void SetVoiceOverValue()
		{
			_selectedGlobalVoiceOver = GlobalVoiceOvers
				.FirstOrDefault(gvo => gvo.CartoonVoiceOverId == SelectedVoiceOverId);
			NotifyOfPropertyChange(() => SelectedGlobalVoiceOver);
			NotifyGlobalVoiceOversButtons();

			if(SelectedCartoon != null)
			{
				if(SelectedEpisode != null)
				{
					_selectedEpisodeVoiceOver = EpisodeVoiceOvers
						.FirstOrDefault(evo => evo.CartoonVoiceOverId == SelectedVoiceOverId);
					NotifyOfPropertyChange(() => SelectedEpisodeVoiceOver);
					NotifyEpisodeVoiceOversButtons();
				}

				_selectedCartoonVoiceOver = CartoonVoiceOvers
					.FirstOrDefault(evo => evo.CartoonVoiceOverId == SelectedVoiceOverId);

				NotifyOfPropertyChange(() => SelectedCartoonVoiceOver);
				NotifyCartoonVoiceOversButtons();
			}

			NotifySharedVoiceOversButtons();
		}

		/// <summary>
		/// Создать новую озвучку в БД
		/// </summary>
		/// <returns></returns>
		private Task<CartoonVoiceOver> CreateNewVoiceOver()
		{
			using(var ctx = new CVDbContext())
			{
				var count = ctx.VoiceOvers.Max(vo => vo.CartoonVoiceOverId) + 1;

				var newVoiceOver = new CartoonVoiceOver
				{
					Name = $"Озвучка_{count}",
					UrlParameter = $"param_{count}",
					Description = $"Описание озвучки_{count}"
				};

				ctx.VoiceOvers.Add(newVoiceOver);
				ctx.SaveChanges();

				var id = ctx.VoiceOvers.ToList().Last().CartoonVoiceOverId;

				return ctx.VoiceOvers.FindAsync(id);
			}
		}

		/// <summary>
		/// Копировать изменения редактируемой озвучки
		/// </summary>
		/// <param name="original">Объект, данные которого требуется изменить</param>
		/// <param name="copy">Копия с последними изменениями</param>
		private void CopyChanges(ref CartoonVoiceOver original, CartoonVoiceOver copy)
		{
			original.Name = copy.Name;
			original.UrlParameter = copy.UrlParameter;
			original.Description = copy.Description;
		}

		#endregion

		#region Notification methods

		/// <summary>
		/// Оповестить свойства зависимые от выбранного м/ф
		/// </summary>
		private void NotifyCartoonData()
		{
			NotifyOfPropertyChange(() => SelectedCartoon);
			NotifyOfPropertyChange(() => SelectedCartoonVisibility);
			NotifyOfPropertyChange(() => CanCancelCartoonSelection);
		}
		/// <summary>
		/// Оповестить свойства зависимые от выбранного сезона
		/// </summary>
		private void NotifySeasonData()
		{
			NotifyOfPropertyChange(() => SelectedSeason);
			NotifyOfPropertyChange(() => SelectedSeasonVisibility);
			NotifyOfPropertyChange(() => CanCancelSeasonSelection);
		}
		/// <summary>
		/// Оповестить свойства зависимые от выбранного эпизода
		/// </summary>
		private void NotifyEpisodeData()
		{
			NotifyOfPropertyChange(() => SelectedEpisode);
			NotifyOfPropertyChange(() => SelectedEpisodeVisibility);
			NotifyOfPropertyChange(() => CanCancelEpisodeSelection);
		}
		/// <summary>
		/// Оповестить общие кнопки списков озвучек
		/// </summary>
		private void NotifySharedVoiceOversButtons()
		{
			NotifyOfPropertyChange(() => CanUnlockEditingInterface);
		}
		/// <summary>
		/// Оповестить кнопки глобального списка озвучек
		/// </summary>
		private void NotifyGlobalVoiceOversButtons()
		{
			NotifyOfPropertyChange(() => CanAddGlobalVoiceOver);
			NotifyOfPropertyChange(() => CanEditSelectedGlobalVoiceOver);
			NotifyOfPropertyChange(() => CanRemoveGlobalVoiceOverAction);
			NotifyOfPropertyChange(() => CanCancelGlobalVoiceOverSelection);
			NotifyOfPropertyChange(() => CanMoveToCartoonVoiceOvers);
			NotifyOfPropertyChange(() => CanMoveFromGlobalToEpisodeVoiceOvers);

		}

		/// <summary>
		/// Оповестить кнопки списка озвучек м/ф
		/// </summary>
		private void NotifyCartoonVoiceOversButtons()
		{
			NotifyOfPropertyChange(() => CanAddCartoonVoiceOver);
			NotifyOfPropertyChange(() => CanEditSelectedCartoonVoiceOver);
			NotifyOfPropertyChange(() => CanRemoveSelectedCartoonVoiceOver);
			NotifyOfPropertyChange(() => CanCancelCartoonVoiceOverSelection);
			NotifyOfPropertyChange(() => CanMoveToEpisodeVoiceOvers);
		}
		/// <summary>
		/// Оповестить кнопки списка озвучек эпизода
		/// </summary>
		private void NotifyEpisodeVoiceOversButtons()
		{
			NotifyOfPropertyChange(() => CanAddEpisodeVoiceOverAction);
			NotifyOfPropertyChange(() => CanEditSelectedEpisodeVoiceOver);
			NotifyOfPropertyChange(() => CanRemoveSelectedEpisodeVoiceOver);
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
			NotifyOfPropertyChange(() => GlobalVoiceOvers);
			NotifyOfPropertyChange(() => CartoonVoiceOvers);
			NotifyOfPropertyChange(() => EpisodeVoiceOvers);
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
