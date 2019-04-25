// ReSharper disable CheckNamespace
namespace CartoonViewer.Settings.ViewingsSettingsFolder.ViewModels
{
	using Caliburn.Micro;
	using Database;
	using static Helpers.Helper;

	public partial class ViewingsSettingsViewModel : Screen
	{

		#region Check\uncheck actions

		

		#endregion


		public void SeasonCheckValidation()
		{
			using(var ctx = new CVDbContext(AppDataPath))
			{
				var test1 = ctx.Entry(Seasons).CurrentValues;

				var test2 = ctx.ChangeTracker.HasChanges();
			}
		}

		public void EpisodeCheckValidation()
		{
		}

		public void VoiceOverCheck()
		{

		}

		public void VoiceOverUncheck()
		{

		}

		#region Selection actions

		public void CartoonSelectionChanged()
		{

		}

		public void SeasonSelectionChanged()
		{

		}

		public void EpisodeSelectionChanged()
		{

		}

		public void VoiceOverSelectionChanged()
		{

		}

		#endregion


		#region Cancel selection buttons
		/// <summary>
		/// Снять выделение с выбранного м/с
		/// </summary>
		public void CancelCartoonSelection() => SelectedCartoon = null;
		public bool CanCancelCartoonSelection => SelectedCartoon != null;

		/// <summary>
		/// Снять выделение с выбранного сезона
		/// </summary>
		public void CancelSeasonSelection() => SelectedSeason = null;
		public bool CanCancelSeasonSelection => SelectedSeason != null;

		/// <summary>
		/// Снять выделение с выбранного эпизода
		/// </summary>
		public void CancelEpisodeSelection() => SelectedEpisode = null;
		public bool CanCancelEpisodeSelection => SelectedEpisode != null;

		/// <summary>
		/// Снять выделение с выбранной озвучки
		/// </summary>
		public void CancelVoiceOverSelection() => SelectedVoiceOver = null;
		public bool CanCancelVoiceOverSelection => SelectedVoiceOver != null;

		#endregion

	}
}
