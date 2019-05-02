// ReSharper disable CheckNamespace
namespace CartoonViewer.Settings.CartoonEditorFolder.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.Data.Entity;
	using System.Linq;
	using Caliburn.Micro;
	using CartoonViewer.ViewModels;
	using Database;
	using Models.CartoonModels;
	using Models.SettingModels;
	using Newtonsoft.Json;
	using static Helpers.Helper;
	using static Helpers.SettingsHelper;


	public partial class EpisodesEditingViewModel : Screen, ISettingsViewModel
	{
		#region Load/Update Data

		/// <summary>
		/// Загрузка данных сезона
		/// </summary>
		private void LoadSeasonData()
		{
			List<CartoonEpisode> episodes;

			using(var ctx = new CVDbContext(AppDataPath))
			{
				episodes = ctx.CartoonEpisodes
									.Include(ce => ce.EpisodeVoiceOvers)
									.Where(ce => ce.CartoonSeasonId == GlobalIdList.SeasonId).ToList();
			}

			Episodes = new BindableCollection<CartoonEpisode>(episodes);

			SelectedEpisode = Episodes.FirstOrDefault();
		}

		/// <summary>
		/// Загрузка данных выбранного эпизода для редактирования
		/// </summary>
		/// <returns></returns>
		private bool LoadSelectedEpisodeData()
		{
			using(var ctx = new CVDbContext(AppDataPath))
			{
				var episode = ctx.CartoonEpisodes
								 .Include(ce => ce.EpisodeVoiceOvers)
								 .First(ce => ce.CartoonEpisodeId == GlobalIdList.EpisodeId);

				if(episode.EpisodeVoiceOvers.Count == 0)
				{
					WinMan.ShowDialog(new DialogViewModel(
										  "У выбранного эпизода отсутствуют озвучки, добавьте одну или более для продолжения редактирования",
										  DialogType.INFO));
					return false;
				}

				var voiceOvers = ctx.VoiceOvers
									.Include(vo => vo.CartoonEpisodes)
									.Include(vo => vo.CheckedEpisodes)
									.Where(vo => vo.CartoonEpisodes
												   .Any(ce => ce.CartoonEpisodeId == GlobalIdList.EpisodeId));

				foreach(var vo in voiceOvers)
				{
					vo.SelectedEpisodeId = episode.CartoonEpisodeId;
				}

				var options = ctx.EpisodeOptions
								 .Include(eo => eo.Jumpers)
								 .Where(eo => eo.CartoonEpisodeId == GlobalIdList.EpisodeId);

				EditableEpisode = CloneObject<CartoonEpisode>(episode);
				VoiceOvers = new BindableCollection<CartoonVoiceOver>(voiceOvers);
				EpisodeOptions = new BindableCollection<EpisodeOption>(options);
				return true;
			}
		}

		/// <summary>
		/// Обновить список озвучек
		/// </summary>
		public void UpdateVoiceOverList()
		{
			if(GlobalIdList.EpisodeId == 0)
				return;

			CartoonEpisode episode;

			using(var ctx = new CVDbContext(AppDataPath))
			{
				episode = ctx.CartoonEpisodes
								   .Include(e => e.EpisodeVoiceOvers)
								   .Single(e => e.CartoonEpisodeId == GlobalIdList.EpisodeId);
			}

			foreach(var vo in episode.EpisodeVoiceOvers)
			{
				vo.SelectedEpisodeId = episode.CartoonEpisodeId;
			}

			VoiceOvers = new BindableCollection<CartoonVoiceOver>(episode.EpisodeVoiceOvers);
			NotifyOfPropertyChange(() => CanEditPreviousEpisode);
			NotifyOfPropertyChange(() => CanEditNextEpisode);
		}

		#endregion

		#region Creating/removing methods

		/// <summary>
		/// Создание нового эпизода с активной озвучкой
		/// </summary>
		/// <param name="ctx">база данных</param>
		/// <param name="voiceOver">активная озвучка</param>
		private CartoonEpisode CreateNewEpisode(CVDbContext ctx, CartoonVoiceOver voiceOver)
		{
			var count = _episodes.Count + 1;

			// создание эпизода с активной озвучкой
			var episode = new CartoonEpisode
			{
				CartoonSeasonId = GlobalIdList.SeasonId,
				CartoonId = GlobalIdList.CartoonId,
				CartoonVoiceOver = voiceOver,
				Name = $"{count}-й эпизод",
				Description = $"{count}-й эпизод",
				Number = count
			};

			ctx.CartoonEpisodes.Add(episode);
			ctx.SaveChanges();

			// загрузка эпизода с ID
			episode = ctx.CartoonEpisodes.ToList().Last();
			// добавление эпизода в список выбранных эпизодов озвучки
			voiceOver.CheckedEpisodes.Add(episode);
			episode.EpisodeVoiceOvers.Add(voiceOver);
			ctx.SaveChanges();
			return episode;
		}

		/// <summary>
		/// Создание новой опции эпизода с джампером
		/// </summary>
		/// <param name="ctx">база данных</param>
		/// <param name="episode">созданный эпизод</param>
		/// <param name="voiceOver">активная озвучка</param>
		private void CreateNewEpisodeOption(CVDbContext ctx, CartoonEpisode episode, CartoonVoiceOver voiceOver)
		{
			// создание новой опции
			var episodeOption = new EpisodeOption
			{
				CartoonEpisodeId = episode.CartoonEpisodeId,
				CartoonVoiceOverId = voiceOver.CartoonVoiceOverId,
				Name = $"{voiceOver.Name}_Option"
			};

			ctx.EpisodeOptions.Add(episodeOption);
			ctx.SaveChanges();

			episodeOption = ctx.EpisodeOptions.ToList().Last();
			// создание нового джампера
			var jumper = new Jumper
			{
				EpisodeOptionId = episodeOption.EpisodeOptionId,
				Number = 1
			};

			ctx.Jumpers.Add(jumper);
			ctx.SaveChanges();

			episodeOption = ctx.EpisodeOptions.ToList().Last();

			episodeOption.Duration = CalculatingDuration(episodeOption);
			ctx.SaveChanges();
		}

		/// <summary>
		/// Удалить выбранный эпизод из БД
		/// </summary>
		private void RemoveEpisodeFromDb()
		{
			using(var ctx = new CVDbContext(AppDataPath))
			{
				var episode = ctx.CartoonEpisodes
								 .FirstOrDefault(ce => ce.CartoonEpisodeId == GlobalIdList.EpisodeId);

				if(episode == null)
				{
					throw new Exception("Удаляемый эпизод не найден");
				}

				ctx.CartoonEpisodes.Remove(episode);
				ctx.SaveChanges();
			}
		}

		/// <summary>
		/// Добавить новый джампер в БД
		/// </summary>
		/// <param name="jumper">добавляемый джампер</param>
		private void AddJumperToDb(Jumper jumper)
		{
			using(var ctx = new CVDbContext(AppDataPath))
			{
				ctx.EpisodeOptions
				   .First(eo => eo.EpisodeOptionId == SelectedEpisodeOption
									.EpisodeOptionId).Duration = CalculatingDuration();
				ctx.Jumpers.Add(jumper);
				ctx.SaveChanges();

				Jumpers.Last().JumperId = ctx.Jumpers.ToList().Last().JumperId;
				NotifyOfPropertyChange(() => Jumpers);
			}
		}

		/// <summary>
		/// Удалить выбранный джампер из БД
		/// </summary>
		private void RemoveJumperFromDb()
		{
			using(var ctx = new CVDbContext(AppDataPath))
			{
				var jumper = ctx.Jumpers.First(j => j.JumperId == SelectedJumper.JumperId);

				ctx.Jumpers.Remove(jumper);
				ctx.SaveChanges();

				SelectedEpisodeOption.Jumpers.Remove(SelectedJumper);
				Jumpers.Remove(SelectedJumper);

				ctx.EpisodeOptions
				   .First(eo => eo.EpisodeOptionId == SelectedEpisodeOption
									.EpisodeOptionId).Duration = CalculatingDuration();
				ctx.SaveChanges();
			}
		}

		#endregion

		#region EpisodeTime methods

		/// <summary>
		/// Конвертировать значения Эпизода в значения класса EpisodeTime
		/// </summary>
		/// <param name="episodeOption">редактируемая опция</param>
		/// <param name="jumper">редактируемый джампер</param>
		/// <returns></returns>
		private EpisodeTime ConvertToEpisodeTime(EpisodeOption episodeOption, Jumper jumper) => new EpisodeTime
		{
			JumperTimeHours = jumper?.StartTime.Hours,
			JumperTimeMinutes = jumper?.StartTime.Minutes,
			JumperTimeSeconds = jumper?.StartTime.Seconds,
			SkipCount = jumper?.SkipCount,
			CreditsTimeHours = episodeOption?.CreditsStart.Hours,
			CreditsTimeMinutes = episodeOption?.CreditsStart.Minutes,
			CreditsTimeSeconds = episodeOption?.CreditsStart.Seconds
		};

		/// <summary>
		/// Подсчет длительности эпизода
		/// </summary>
		/// <returns></returns>
		private TimeSpan CalculatingDuration(EpisodeOption episodeOption = null)
		{
			int totalSkipCount;

			if(episodeOption != null)
			{
				totalSkipCount = episodeOption.Jumpers.Sum(j => j?.SkipCount ?? 0);

				return episodeOption.CreditsStart - new TimeSpan(0, 0, totalSkipCount * 5);
			}


			if(SelectedEpisodeOption == null)
				return new TimeSpan();

			totalSkipCount = Jumpers.Sum(j => j.SkipCount);

			var duration = SelectedEpisodeOption.CreditsStart - new TimeSpan(0, 0, totalSkipCount * 5);

			//SelectedEpisodeOption.Duration = duration;

			return duration;
		}

		/// <summary>
		/// Получение данных из EpisodeTime
		/// </summary>
		/// <param name="episodeTime">Класс с данными</param>
		/// <returns></returns>
		private (TimeSpan JumperTime, int SkipCount, TimeSpan CreditsStart) ConvertFromEpisodeTime(EpisodeTime episodeTime)
		{
			var jumperTime = new TimeSpan(
				episodeTime.JumperTimeHours ?? 0,
				episodeTime.JumperTimeMinutes ?? 0,
				episodeTime.JumperTimeSeconds ?? 0);
			var skipCount = episodeTime.SkipCount ?? 7;
			var creditsStart = new TimeSpan(
				episodeTime.CreditsTimeHours ?? 0,
				episodeTime.CreditsTimeMinutes ?? 21,
				episodeTime.CreditsTimeSeconds ?? 30);

			return (jumperTime, skipCount, creditsStart);
		}

		#endregion

		#region Saving changes

		/// <summary>
		/// Сохранение изменений джамперов
		/// </summary>
		/// <param name="ctx"></param>
		private void SaveJumpersChanges(CVDbContext ctx)
		{
			var tempOption = JsonConvert.DeserializeObject<EpisodeOption>(TempEpisodeOptionSnapshot);
			var jumpers = ctx.Jumpers.Where(j => j.EpisodeOptionId == tempOption.EpisodeOptionId).ToList();

			for(var i = 0; i < Jumpers.Count; i++)
			{
				if(IsEquals(jumpers[i], Jumpers[i]) is false)
				{
					jumpers[i].SkipCount = Jumpers[i].SkipCount;
					jumpers[i].StartTime = Jumpers[i].StartTime;

					SelectedEpisodeOption.Jumpers[i] = Jumpers[i];

					ctx.Entry(jumpers[i]).State = EntityState.Modified;
					ctx.SaveChanges();
				}
			}

			ctx.SaveChanges();
		}
		/// <summary>
		/// Сохранение изменений эпизода
		/// </summary>
		/// <param name="ctx"></param>
		private void SaveEpisodeChanges(CVDbContext ctx)
		{
			var episode = ctx.CartoonEpisodes
							 .Include(ce => ce.EpisodeOptions)
							 .Include(ce => ce.EpisodeOptions)
							 .Include(c => c.EpisodeVoiceOvers)
							 .First(c => c.CartoonEpisodeId == GlobalIdList.EpisodeId);

			episode.Name = EditableEpisode.Name;
			episode.Description = EditableEpisode.Description;
			ctx.SaveChanges();
		}

		private void SaveEpisodeOptionChanges(CVDbContext ctx)
		{
			var option = ctx.EpisodeOptions
							.Include(eo => eo.Jumpers)
							.First(eo => eo.EpisodeOptionId == SelectedEpisodeOption.EpisodeOptionId);
			SelectedEpisodeOption.Duration = EpisodeDuration;


			if(IsEquals(option, SelectedEpisodeOption) is false)
			{
				option.Duration = SelectedEpisodeOption.Duration;
				option.CreditsStart = SelectedEpisodeOption.CreditsStart;

				ctx.Entry(option).State = EntityState.Modified;
				ctx.SaveChanges();
			}
		}

		/// <summary>
		/// Проверка на наличие изменений через диалоговое окно
		/// </summary>
		/// <returns></returns>
		private bool HasChangesValidation()
		{
			if(HasChanges)
			{
				var dvm = new DialogViewModel(null, DialogType.SAVE_CHANGES);
				WinMan.ShowDialog(dvm);

				switch(dvm.DialogResult)
				{
					case DialogResult.YES_ACTION:
						SaveChanges();
						return true;
					case DialogResult.NO_ACTION:
						CancelChanges();
						return true;
					default:
						return false;
				}
			}

			return true;
		}

		#endregion

		#region Notifications

		/// <summary>
		/// Уведомить кнопки списка эпизодов об изменениях
		/// </summary>
		private void NotifyEpisodeListButtons()
		{
			NotifyOfPropertyChange(() => CanAddEpisode);
			NotifyOfPropertyChange(() => CanEditEpisode);
			NotifyOfPropertyChange(() => CanRemoveEpisode);
			NotifyOfPropertyChange(() => CanCancelEpisodeSelection);
		}

		/// <summary>
		/// Уведомить кнопки редактирования об изменении значений
		/// </summary>
		private void NotifyEditingButtons()
		{
			NotifyOfPropertyChange(() => CanSaveChanges);
			NotifyOfPropertyChange(() => CanCancelChanges);
			NotifyOfPropertyChange(() => CanCancelEditing);
			NotifyOfPropertyChange(() => CanEditPreviousEpisode);
			NotifyOfPropertyChange(() => CanEditNextEpisode);
			NotifyOfPropertyChange(() => CanCancelEpisodeSelection);
		}

		private void NotifyEditingProperties()
		{
			NotifyOfPropertyChange(() => CanCancelEditing);
			NotifyOfPropertyChange(() => CanEditPreviousEpisode);
			NotifyOfPropertyChange(() => CanEditNextEpisode);
			NotifyOfPropertyChange(() => EpisodeEditingVisibility);
			NotifyOfPropertyChange(() => CanSaveChanges);
			NotifyOfPropertyChange(() => CanCancelChanges);
		}



		private void NotifyTimeProperties()
		{
			NotifyOfPropertyChange(() => CanAddJumper);
			NotifyOfPropertyChange(() => CanRemoveJumper);
			NotifyOfPropertyChange(() => CanSetOnTodayLastDateViewed);
			NotifyOfPropertyChange(() => CanResetLastDateViewed);
			NotifyOfPropertyChange(() => EpisodeDuration);
		}

		#endregion

	}
}
