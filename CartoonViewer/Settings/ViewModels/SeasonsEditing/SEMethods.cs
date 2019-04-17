namespace CartoonViewer.Settings.ViewModels
{
	using System;
	using System.Data.Entity;
	using System.Linq;
	using System.Windows;
	using Caliburn.Micro;
	using Database;
	using Models.CartoonModels;
	using Models.SettingModels;
	using static Helpers.Cloner;

	public partial class SeasonsEditingViewModel : Screen, ISettingsViewModel
	{
		#region Private methods

		private TimeSpan CalculatingDuration(CartoonEpisode cartoonEpisode) =>
			cartoonEpisode.CreditsStart - (cartoonEpisode.DelayedSkip +
										   new TimeSpan(
											   0,
											   0,
											   cartoonEpisode.SkipCount * 5));

		/// <summary>
		/// Загрузка данных ссезона
		/// </summary>
		private void LoadSeasonData()
		{
			CartoonSeason result;

			using(var ctx = new CVDbContext())
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

			using(var ctx = new CVDbContext())
			{
				ctx.CartoonEpisodes
				   .Where(e => e.CartoonEpisodeId == EpisodeId)
				   //.Include(e => e.EpisodeVoiceOvers)
				   .Load();
				result = ctx.CartoonEpisodes.FirstOrDefault();
			}

			SelectedCartoonEpisode = CloneEpisode(result);
			TempCartoonEpisode = CloneEpisode(result);

			if(((CartoonsControlViewModel)Parent).SelectedWebSite.Url == "http://freehat.cc")
			{
				using(var ctx = new CVDbContext())
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
			if(SelectedCartoonEpisode == null)
			{
				if(EpisodeEditingVisibility == Visibility.Visible)
				{
					EpisodeEditingVisibility = Visibility.Hidden;
				}
				return;
			}

			if(EpisodeEditingVisibility == Visibility.Hidden)
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

		private void NotifySeasonList()
		{
			NotifyOfPropertyChange(() => Episodes);
			NotifyOfPropertyChange(() => CanEditEpisode);
			NotifyOfPropertyChange(() => CanRemoveEpisode);
			NotifyOfPropertyChange(() => CanCancelSelection);
		}

		#endregion

		#region Public methods

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

		#endregion
	}
}
