namespace CartoonViewer.Settings.CartoonEditorSetting.ViewModels
{
	using System;
	using System.Data.Entity;
	using System.Linq;
	using System.Windows;
	using Caliburn.Micro;
	using CartoonEditorSetting.ViewModels;
	using Database;
	using Models.CartoonModels;
	using Models.SettingModels;
	using static Helpers.Cloner;
	using static Helpers.Helper;


	public partial class EpisodesEditingViewModel : Screen, ISettingsViewModel
	{
		#region Private methods

		private TimeSpan CalculatingDuration(CartoonEpisode episode) =>
			episode == null
				? new TimeSpan()
				: episode.CreditsStart -
				  (episode.DelayedSkip +
				   new TimeSpan(0, 0, episode.SkipCount * 5));

		/// <summary>
		/// Загрузка данных сезона
		/// </summary>
		private async void LoadSeasonData()
		{
			CartoonSeason season;

			using(var ctx = new CVDbContext())
			{
				season = await ctx.CartoonSeasons
							.Include(s => s.CartoonEpisodes)
							.SingleAsync(s => s.CartoonSeasonId == GlobalIdList.SeasonId);
			}

			Episodes = new BindableCollection<CartoonEpisode>(season.CartoonEpisodes);
			if(Episodes.Count > 0)
			{
				SelectedEpisode = Episodes.First();
			}
		}

		/// <summary>
		/// Загрузка данных эпизодов
		/// </summary>
		private async void LoadEpisodeData()
		{
			CartoonEpisode episode;

			using(var ctx = new CVDbContext())
			{
				episode = await ctx.CartoonEpisodes
								   .Include(e => e.EpisodeVoiceOvers)
								   .SingleAsync(e => e.CartoonEpisodeId == GlobalIdList.EpisodeId);
			}

			EditingEpisode = CloneEpisode(episode);
			TempEpisode = CloneEpisode(episode);
			VoiceOvers = new BindableCollection<CartoonVoiceOver>(CloneVoiceOverList(episode.EpisodeVoiceOvers));
			EpisodeTime = ConvertToEpisodeTime(_editingEpisode);
			
			EpisodeEditingVisibility = Visibility.Visible;
			IsNotEditing = false;
			NotifyEpisodeListButtons();
		}

		/// <summary>
		/// Конвертировать значения Эпизода в значения класса EpisodeTime
		/// </summary>
		/// <param name="selectedEpisode">Выбранный эпизод</param>
		/// <returns></returns>
		private EpisodeTime ConvertToEpisodeTime(CartoonEpisode selectedEpisode) => new EpisodeTime
		{
			DelayedSkipMinutesString = (selectedEpisode?.DelayedSkip.Minutes).ToString(),
			DelayedSkipSecondsString = (selectedEpisode?.DelayedSkip.Seconds).ToString(),
			SkipCountString = selectedEpisode?.SkipCount.ToString(),
			CreditsStartHoursString = selectedEpisode?.CreditsStart.Hours.ToString(),
			CreditsStartMinutesString = selectedEpisode?.CreditsStart.Minutes.ToString(),
			CreditsStartSecondsString = selectedEpisode?.CreditsStart.Seconds.ToString()
		};

		private (TimeSpan DelayedSkip, int SkipCount, TimeSpan CreditsStart) ConvertFromEpisodeTime(
			EpisodeTime episodeTime)
		{
			var delayedSkip = new TimeSpan(
				0,
				int.Parse(episodeTime.DelayedSkipMinutesString),
				int.Parse(episodeTime.DelayedSkipSecondsString));
			var skipCount = int.Parse(episodeTime.SkipCountString);
			var creditsStart = new TimeSpan(
				int.Parse(episodeTime.CreditsStartHoursString),
				int.Parse(episodeTime.CreditsStartMinutesString),
				int.Parse(episodeTime.CreditsStartSecondsString));

			return (delayedSkip, skipCount, creditsStart);
		}

		private void NotifyEpisodeListButtons()
		{
			NotifyOfPropertyChange(() => CanAddEpisode);
			NotifyOfPropertyChange(() => CanEditEpisode);
			NotifyOfPropertyChange(() => CanRemoveEpisode);
			NotifyOfPropertyChange(() => CanCancelSelection);
		}

		private void NotifyEditingButtons()
		{
			NotifyOfPropertyChange(() => HasChanges);
			NotifyOfPropertyChange(() => CanSaveChanges);
			NotifyOfPropertyChange(() => CanCancelChanges);
			NotifyOfPropertyChange(() => CanCancelEditing);
		}
		
		#endregion
	}
}
