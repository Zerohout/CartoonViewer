﻿// ReSharper disable CheckNamespace
namespace CartoonViewer.Settings.CartoonEditorFolder.ViewModels
{
	using System.Collections.Generic;
	using System.Data.Entity;
	using System.Linq;
	using System.Threading.Tasks;
	using Caliburn.Micro;
	using Database;
	using Helpers;
	using Models.CartoonModels;
	using static Helpers.Helper;

	public partial class VoiceOversEditingViewModel : Screen, ISettingsViewModel
	{
		public void UpdateVoiceOverList()
		{
			var tempId = SelectedVoiceOverId;
			var tempValues = IdList;

			CancelCartoonSelection();
			IdList = tempValues;
			LoadGlobalVoiceOverList();
			SelectedVoiceOverId = tempId;
			LoadData();
			

			SelectVoiceOver();


			if(IsNotEditing is false)
			{
				EditedVoiceOver = CloneObject<CartoonVoiceOver>(SelectedGlobalVoiceOver);
				TempEditedVoiceOver = CloneObject<CartoonVoiceOver>(SelectedGlobalVoiceOver);
			}
		}

		private void SelectVoiceOver()
		{
			if(CartoonVoiceOvers.Count == 0 &&
			   EpisodeVoiceOvers.Count == 0)
			{
				SelectedGlobalVoiceOver = GlobalVoiceOvers.Count > 0
					? GlobalVoiceOvers.Last()
					: null;
				return;
			}

			if(EpisodeVoiceOvers.Count > 0)
			{
				SelectedEpisodeVoiceOver = EpisodeVoiceOvers.Last();
				return;
			}

			if(CartoonVoiceOvers.Count > 0)
			{
				SelectedCartoonVoiceOver = CartoonVoiceOvers.Last();
				return;
			}

			if(SelectedVoiceOverId > 0)
			{
				SelectedGlobalVoiceOver = GlobalVoiceOvers
					.FirstOrDefault(gvo => gvo.CartoonVoiceOverId == SelectedVoiceOverId);
			}

		}

		#region Global voice overs methods

		/// <summary>
		/// Загрузить список глобальных озвучек из БД
		/// </summary>
		private void LoadGlobalVoiceOverList()
		{
			List<CartoonVoiceOver> voiceOvers;
			using(var ctx = new CVDbContext(Helpers.SettingsHelper.AppDataPath))
			{
				voiceOvers = ctx.VoiceOvers.ToList();
			}

			GlobalVoiceOvers = new BindableCollection<CartoonVoiceOver>(voiceOvers);
		}

		/// <summary>
		/// Удаление глобальной озвучки из БД
		/// </summary>
		private void RemoveSelectedGlobalVoiceOverFromDb()
		{

			using(var ctx = new CVDbContext(Helpers.SettingsHelper.AppDataPath))
			{
				var voiceOver = ctx.VoiceOvers.Find(SelectedVoiceOverId);

				if(IdList.EpisodeId == 0)
				{
					ctx.CartoonEpisodes
					   .SingleOrDefault(ce => ce.EpisodeVoiceOvers
												.Any(evo => evo.CartoonVoiceOverId == SelectedVoiceOverId))
					   ?.EpisodeVoiceOvers.Remove(voiceOver);
				}
				else
				{
					ctx.CartoonEpisodes
					   .Single(ce => ce.CartoonEpisodeId == IdList.EpisodeId)
					   .EpisodeVoiceOvers.Remove(voiceOver);
				}

				ctx.Entry(voiceOver).State = EntityState.Deleted;

				ctx.SaveChanges();
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
		/// Загрузка озвучек выбранного мультсериала из БД
		/// </summary>
		private void LoadCartoonVoiceOverList()
		{
			BindableCollection<CartoonVoiceOver> voiceOvers;

			using(var ctx = new CVDbContext(Helpers.SettingsHelper.AppDataPath))
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
		private void RemoveSelectedCartoonVoiceOverFromDb()
		{
			using(var ctx = new CVDbContext(Helpers.SettingsHelper.AppDataPath))
			{
				var cartoon = ctx.Cartoons
									   .Include(ce => ce.CartoonVoiceOvers)
									   .Single(ce => ce.CartoonId == IdList.CartoonId);

				ctx.VoiceOvers
				   .Include(vo => vo.Cartoons)
				   .Single(vo => vo.CartoonVoiceOverId == SelectedVoiceOverId)
				   .Cartoons.Remove(cartoon);
				ctx.SaveChanges();
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

			using(var ctx = new CVDbContext(Helpers.SettingsHelper.AppDataPath))
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
		private void RemoveSelectedEpisodeVoiceOverFromDb()
		{
			using(var ctx = new CVDbContext(Helpers.SettingsHelper.AppDataPath))
			{
				var episode = ctx.CartoonEpisodes
									   .Include(ce => ce.EpisodeVoiceOvers)
									   .Single(ce => ce.CartoonEpisodeId == IdList.EpisodeId);

				ctx.VoiceOvers
				   .Include(vo => vo.CartoonEpisodes)
				   .Single(vo => vo.CartoonVoiceOverId == SelectedVoiceOverId)
				   .CartoonEpisodes.Remove(episode);
				ctx.SaveChanges();
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
		/// Загрузка из базы данных списка мультсериалов по ID сайта
		/// </summary>
		private void LoadCartoonList()
		{
			BindableCollection<Cartoon> cartoons;

			using(var ctx = new CVDbContext(Helpers.SettingsHelper.AppDataPath))
			{
				cartoons = new BindableCollection<Cartoon>(ctx.Cartoons
				                                              .Where(c => c.CartoonWebSites
				                                                           .Any(cws => cws.CartoonWebSiteId == 
				                                                                       IdList.WebSiteId)));
			}

			Cartoons = new BindableCollection<Cartoon>(cartoons);
			CartoonIndexes.EndIndex = Cartoons.Count - 1;
			NotifyOfPropertyChange(() => CanSelectNextCartoon);
			NotifyOfPropertyChange(() => CanSelectPreviousCartoon);
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

			CartoonIndexes.CurrentIndex = value == null
				? -1
				: Cartoons.IndexOf(value);
			NotifyOfPropertyChange(() => CanSelectNextCartoon);
			NotifyOfPropertyChange(() => CanSelectPreviousCartoon);

			if(_selectedCartoon == value)
			{
				return;
			}

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
		/// Загрузка из базы данны списка сезонов выбранного мультсериала
		/// </summary>
		private void LoadSeasonList()
		{
			BindableCollection<CartoonSeason> seasons;

			using(var ctx = new CVDbContext(Helpers.SettingsHelper.AppDataPath))
			{
				seasons = new BindableCollection<CartoonSeason>(ctx.CartoonSeasons
				                                                   .Where(cs => cs.CartoonId == 
				                                                                IdList.CartoonId));
			}

			Seasons = new BindableCollection<CartoonSeason>(seasons);
			SeasonIndexes.EndIndex = Seasons.Count - 1;
			NotifyOfPropertyChange(() => CanSelectNextSeason);
			NotifyOfPropertyChange(() => CanSelectPreviousSeason);
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

			SeasonIndexes.CurrentIndex = value == null
				? -1
				: Seasons.IndexOf(value);
			NotifyOfPropertyChange(() => CanSelectNextSeason);
			NotifyOfPropertyChange(() => CanSelectPreviousSeason);

			if(_selectedSeason == value)
			{
				return;
			}

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

			using(var ctx = new CVDbContext(Helpers.SettingsHelper.AppDataPath))
			{
				episodes = new BindableCollection<CartoonEpisode>(ctx.CartoonEpisodes
				                                                     .Where(ce => ce.CartoonSeasonId == 
				                                                                  IdList.SeasonId)
				                                                     .ToList());
			}

			Episodes = new BindableCollection<CartoonEpisode>(episodes);

			EpisodeIndexes.EndIndex = Episodes.Count - 1;
			NotifyOfPropertyChange(() => CanSelectNextEpisode);
			NotifyOfPropertyChange(() => CanSelectPreviousEpisode);
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

			EpisodeIndexes.CurrentIndex = value == null
				? -1
				: Episodes.IndexOf(value);
			NotifyOfPropertyChange(() => CanSelectNextEpisode);
			NotifyOfPropertyChange(() => CanSelectPreviousEpisode);

			if(_selectedEpisode == value)
			{
				return;
			}

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

		#region VoiceOver methods

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
		private CartoonVoiceOver CreateNewVoiceOver()
		{
			using(var ctx = new CVDbContext(Helpers.SettingsHelper.AppDataPath))
			{
				var count = ctx.VoiceOvers.Max(vo => vo.CartoonVoiceOverId) + 1;

				var newVoiceOver = new CartoonVoiceOver
				{
					Name = $"Озвучка_{count}",
					UrlParameter = "",
					Description = $"Описание озвучки_{count}"
				};

				ctx.VoiceOvers.Add(newVoiceOver);
				ctx.SaveChanges();

				var id = ctx.VoiceOvers.ToList().Last().CartoonVoiceOverId;

				return ctx.VoiceOvers.Find(id);
			}
		}

		#endregion

		#region General methods

		/// <summary>
		/// Загрузка из базы данных всех необходимых данных
		/// </summary>
		private void LoadData()
		{
			// --Начальная, когда ни один мульсериал не выбран
			if(_selectedCartoon == null)
			{
				LoadCartoonList();

				// --При загруженных с конструктора данных мультсериала
				if(IdList.CartoonId > 0)
				{
					var tempId = 0;
					if(_selectedCartoon != null)
					{
						tempId = _selectedCartoon.CartoonId;
					}

					_selectedCartoon = _cartoons.FirstOrDefault(c => c.CartoonId == IdList.CartoonId);

					if(_selectedCartoon == null)
					{
						IdList.CartoonId = tempId;
						_selectedCartoon = _cartoons.FirstOrDefault(c => c.CartoonId == IdList.CartoonId);
					}
					
					CartoonIndexes.CurrentIndex = Cartoons.IndexOf(_selectedCartoon);
					LoadData();
				}
				else
				{
					SelectedCartoon = _cartoons.FirstOrDefault();
				}

				NotifyCartoonData();
				return;
			}

			// --При выборе мультсериала
			if(_selectedSeason == null)
			{
				LoadCartoonVoiceOverList();
				LoadSeasonList();

				// установка значения выбранному мультфильму при смене его на другой
				_selectedCartoon = _cartoons.FirstOrDefault(c => c.CartoonId == IdList.CartoonId);
				CartoonIndexes.CurrentIndex = Cartoons.IndexOf(_selectedCartoon);
				NotifyCartoonData();

				if(IdList.SeasonId > 0)
				{
					var tempId = 0;
					if(_selectedSeason != null)
					{
						tempId = _selectedSeason.CartoonSeasonId;
					}

					_selectedSeason = _seasons.FirstOrDefault(cs => cs.CartoonSeasonId == IdList.SeasonId);

					if(_selectedSeason == null)
					{
						IdList.SeasonId = tempId;
						_selectedSeason = _seasons.FirstOrDefault(cs => cs.CartoonSeasonId == IdList.SeasonId);
					}
					
					SeasonIndexes.CurrentIndex = Seasons.IndexOf(_selectedSeason);
					LoadData();
				}
				else
				{
					SelectedSeason = _seasons.FirstOrDefault();
				}

				NotifySeasonData();
				return;
			}

			// --При выборе сезона
			if(_selectedEpisode == null)
			{
				LoadEpisodeList();

				_selectedSeason = _seasons.FirstOrDefault(cs => cs.CartoonSeasonId == IdList.SeasonId);
				SeasonIndexes.CurrentIndex = Seasons.IndexOf(_selectedSeason);
				NotifySeasonData();

				if(IdList.EpisodeId > 0)
				{
					var tempId = 0;
					if (_selectedEpisode != null)
					{
						tempId = _selectedEpisode.CartoonEpisodeId;
					}

					_selectedEpisode = _episodes.FirstOrDefault(ce => ce.CartoonEpisodeId == IdList.EpisodeId);

					if (_selectedEpisode == null)
					{
						IdList.EpisodeId = tempId;
						_selectedEpisode = _episodes.FirstOrDefault(ce => ce.CartoonEpisodeId == IdList.EpisodeId);
					}

					EpisodeIndexes.CurrentIndex = Episodes.IndexOf(_selectedEpisode);
					LoadData();
				}
				else
				{
					SelectedEpisode = _episodes.FirstOrDefault();
				}

				NotifyEpisodeData();
				return;
			}

			// --При выборе эпизода
			LoadEpisodeVoiceOverList();
			_selectedEpisode = _episodes.FirstOrDefault(ce => ce.CartoonEpisodeId == IdList.EpisodeId);
			EpisodeIndexes.CurrentIndex = Episodes.IndexOf(_selectedEpisode);
			NotifyEpisodeData();
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
			NotifyOfPropertyChange(() => CanSelectNextCartoon);
			NotifyOfPropertyChange(() => CanSelectPreviousCartoon);
		}
		/// <summary>
		/// Оповестить свойства зависимые от выбранного сезона
		/// </summary>
		private void NotifySeasonData()
		{
			NotifyOfPropertyChange(() => SelectedSeason);
			NotifyOfPropertyChange(() => SelectedSeasonVisibility);
			NotifyOfPropertyChange(() => CanCancelSeasonSelection);
			NotifyOfPropertyChange(() => CanSelectNextSeason);
			NotifyOfPropertyChange(() => CanSelectPreviousSeason);
		}
		/// <summary>
		/// Оповестить свойства зависимые от выбранного эпизода
		/// </summary>
		private void NotifyEpisodeData()
		{
			NotifyOfPropertyChange(() => SelectedEpisode);
			NotifyOfPropertyChange(() => SelectedEpisodeVisibility);
			NotifyOfPropertyChange(() => CanCancelEpisodeSelection);
			NotifyOfPropertyChange(() => CanSelectNextEpisode);
			NotifyOfPropertyChange(() => CanSelectPreviousEpisode);
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
