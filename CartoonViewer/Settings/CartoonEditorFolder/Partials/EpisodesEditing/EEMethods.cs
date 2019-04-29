// ReSharper disable CheckNamespace
namespace CartoonViewer.Settings.CartoonEditorFolder.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.Data.Entity;
	using System.Linq;
	using System.Windows;
	using Caliburn.Micro;
	using CartoonViewer.ViewModels;
	using Database;
	using Helpers;
	using Models.CartoonModels;
	using Models.SettingModels;
	using static Helpers.Cloner;
	using static Helpers.Helper;
	using static Helpers.SettingsHelper;


	public partial class EpisodesEditingViewModel : Screen, ISettingsViewModel
	{
		/// <summary>
		/// Загрузка данных сезона
		/// </summary>
		private async void LoadSeasonData()
		{
			List<CartoonEpisode> episodes;

			using(var ctx = new CVDbContext(AppDataPath))
			{
				episodes = await ctx.CartoonEpisodes
									.Where(ce => ce.CartoonSeasonId == GlobalIdList.SeasonId).ToListAsync();
			}

			Episodes = new BindableCollection<CartoonEpisode>(episodes);

			SelectedEpisode = Episodes.FirstOrDefault();
		}

		///// <summary>
		///// Загрузка данных эпизодов
		///// </summary>
		//private void LoadEpisodeData()
		//{
		//	CartoonEpisode episode;

		//	using(var ctx = new CVDbContext(AppDataPath))
		//	{
		//		episode = ctx.CartoonEpisodes
		//						   .Include(ce => ce.Jumpers)
		//						   .Include(e => e.EpisodeVoiceOvers)
		//						   .Single(e => e.CartoonEpisodeId == GlobalIdList.EpisodeId);
		//	}

		//	EditableEpisode = CloneEpisode(episode);
		//	TempEpisode = CloneEpisode(episode);

		//	Jumpers = new BindableCollection<Jumper>(episode.Jumpers);
		//	EditableJumper = episode.Jumpers.FirstOrDefault();

		//	EpisodeTime = ConvertToEpisodeTime(EditableJumper, EditableEpisode);
		//	TempEpisodeTime = ConvertToEpisodeTime(EditableJumper, EditableEpisode);

		//	VoiceOvers = new BindableCollection<CartoonVoiceOver>(CloneVoiceOverList(episode.EpisodeVoiceOvers));

		//	IsNotEditing = false;

		//	NotifyOfPropertyChange(() => CanSaveChanges);
		//	//NotifyOfPropertyChange(() => CanSaveChanges);
		//	//NotifyOfPropertyChange(() => CanCancelChanges);
		//}

		private CartoonEpisode GetDefaultEpisode(int count)
		{
			var result = new CartoonEpisode
			{
				CartoonSeasonId = GlobalIdList.SeasonId,
				CartoonId = GlobalIdList.CartoonId,
				Jumpers = new List<Jumper> { new Jumper { Number = 1 } },
				Name = $"Название {count} эпизода",
				Description = $"Описание {count} эпизода",
				Number = count
			};

			result.Duration = CalculatingDuration();

			return result;
		}

		/// <summary>
		/// Конвертировать значения Эпизода в значения класса EpisodeTime
		/// </summary>
		/// <param name="jumper">редактируемый джампер</param>
		/// <param name="episode">редактируемый эпизод</param>
		/// <returns></returns>
		private EpisodeTime ConvertToEpisodeTime(Jumper jumper, CartoonEpisode episode) => new EpisodeTime
		{
			JumperTimeHours = jumper?.JumperStartTime.Hours,
			JumperTimeMinutes = jumper?.JumperStartTime.Minutes,
			JumperTimeSeconds = jumper?.JumperStartTime.Seconds,
			SkipCount = jumper?.SkipCount,
			CreditsTimeHours = episode?.CreditsStart.Hours,
			CreditsTimeMinutes = episode?.CreditsStart.Minutes,
			CreditsTimeSeconds = episode?.CreditsStart.Seconds
		};

		/// <summary>
		/// Подсчет длительности эпизода
		/// </summary>
		/// <returns></returns>
		private TimeSpan CalculatingDuration()
		{
			if(EditableEpisode == null)
				return new TimeSpan();

			var totalSkipCount = EditableEpisode.Jumpers.Sum(j => j.SkipCount);

			return EditableEpisode.CreditsStart - new TimeSpan(0, 0, totalSkipCount * 5);
		}

		/// <summary>
		/// Обновить список озвучек
		/// </summary>
		public async void UpdateVoiceOverList()
		{
			if(GlobalIdList.EpisodeId == 0)
				return;

			CartoonEpisode episode;

			using(var ctx = new CVDbContext(AppDataPath))
			{
				episode = await ctx.CartoonEpisodes
								   .Include(e => e.EpisodeVoiceOvers)
								   .SingleAsync(e => e.CartoonEpisodeId == GlobalIdList.EpisodeId);
			}

			VoiceOvers = new BindableCollection<CartoonVoiceOver>(CloneVoiceOverList(episode.EpisodeVoiceOvers));
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
						return true;
					default:
						return false;
				}
			}

			return true;
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


		#region Notifications

		/// <summary>
		/// Уведомить кнопки списка эпизодов об изменениях
		/// </summary>
		private void NotifyEpisodeListButtons()
		{
			NotifyOfPropertyChange(() => CanAddEpisode);
			NotifyOfPropertyChange(() => CanEditEpisode);
			NotifyOfPropertyChange(() => CanRemoveEpisode);
			NotifyOfPropertyChange(() => CanCancelSelection);
		}

		/// <summary>
		/// Уведомить кнопки редактирования об изменении значений
		/// </summary>
		private void NotifyEditingButtons()
		{
			NotifyOfPropertyChange(() => HasChanges);
			NotifyOfPropertyChange(() => CanSaveChanges);
			NotifyOfPropertyChange(() => CanCancelChanges);
			NotifyOfPropertyChange(() => CanCancelEditing);
		}

		private void NotifyEditingProperties()
		{
			NotifyOfPropertyChange(() => CanCancelEditing);
			NotifyOfPropertyChange(() => EpisodeEditingVisibility);
			NotifyOfPropertyChange(() => CanSaveChanges);
			NotifyOfPropertyChange(() => CanCancelChanges);
		}



		private void NotifyTimeProperties()
		{
			NotifyOfPropertyChange(() => CanAddJumper);
			NotifyOfPropertyChange(() => CanRemoveJumper);
			NotifyOfPropertyChange(() => JumperEditingVisibility);
			NotifyOfPropertyChange(() => CanSetOnTodayLastDateViewed);
			NotifyOfPropertyChange(() => CanResetLastDateViewed);
			NotifyOfPropertyChange(() => EpisodeDuration);
		}

		#endregion

	}
}
