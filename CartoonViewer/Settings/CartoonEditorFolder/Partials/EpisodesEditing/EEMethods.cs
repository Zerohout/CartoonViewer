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
				                    .Include(ce => ce.EpisodeVoiceOvers)
									.Where(ce => ce.CartoonSeasonId == GlobalIdList.SeasonId).ToListAsync();
			}

			Episodes = new BindableCollection<CartoonEpisode>(episodes);

			SelectedEpisode = Episodes.FirstOrDefault();
		}

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

			if (episodeOption != null)
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

			foreach (var vo in episode.EpisodeVoiceOvers)
			{
				vo.SelectedEpisodeId = episode.CartoonEpisodeId;
			}

			VoiceOvers = new BindableCollection<CartoonVoiceOver>(episode.EpisodeVoiceOvers);
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
			NotifyOfPropertyChange(() => CanEditPreviousEpisode);
			NotifyOfPropertyChange(() => CanEditNextEpisode);
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
			NotifyOfPropertyChange(() => JumperEditingVisibility);
			NotifyOfPropertyChange(() => CanSetOnTodayLastDateViewed);
			NotifyOfPropertyChange(() => CanResetLastDateViewed);
			NotifyOfPropertyChange(() => EpisodeDuration);
		}

		#endregion

	}
}
