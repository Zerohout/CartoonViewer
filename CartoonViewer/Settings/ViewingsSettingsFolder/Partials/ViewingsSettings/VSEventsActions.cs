// ReSharper disable CheckNamespace
namespace CartoonViewer.Settings.ViewingsSettingsFolder.ViewModels
{
	using System.Windows.Input;
	using Caliburn.Micro;
	using CartoonEditorFolder.ViewModels;
	using Database;
	using static Helpers.Helper;

	public partial class ViewingsSettingsViewModel : Screen
	{

		public void KeyDown(KeyEventArgs e)
		{
			switch(e.KeyboardDevice.Modifiers)
			{
				case ModifierKeys.Control:
					switch(e.Key)
					{
						case Key.OemPlus:
							if(Episodes.Count > 0)
							{
								if (EpisodeIndexes.CurrentIndex < EpisodeIndexes.EndIndex)
								{
									SelectedEpisode = Episodes[EpisodeIndexes.CurrentIndex + 1];
									return;
								}
							}
							break;
						case Key.OemMinus:
							if (Episodes.Count > 0)
							{
								if (EpisodeIndexes.CurrentIndex > 0)
								{
									SelectedEpisode = Episodes[EpisodeIndexes.CurrentIndex - 1];
									return;
								}
							}

							break;
					}

					break;
				case ModifierKeys.None:
					break;
			}
		}


		public void SeasonCheckValidation()
		{
			CvDbContext.ChangeTracker.DetectChanges();

			if(CvDbContext.ChangeTracker.HasChanges())
			{
				CvDbContext.SaveChanges();

			}
		}

		public void EpisodeCheckValidation()
		{
			CvDbContext.ChangeTracker.DetectChanges();

			if(CvDbContext.ChangeTracker.HasChanges())
			{
				CvDbContext.SaveChanges();

			}
		}

		public void VoiceOverCheck()
		{
			CvDbContext.ChangeTracker.DetectChanges();

			if (CvDbContext.ChangeTracker.HasChanges())
			{
				CvDbContext.SaveChanges();
			}
			
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

		private (int CurrentIndex, int EndIndex) EpisodeIndexes;

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
