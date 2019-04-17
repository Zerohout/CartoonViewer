namespace CartoonViewer.Settings.ViewModels
{
	using Caliburn.Micro;

	public partial class VoiceOversEditingViewModel : Screen
	{
		#region EventsActions


		public void CancelCartoonSelection()
		{
			SelectedCartoon = null;

		}

		public bool CanCancelCartoonSelection => SelectedCartoon != null;
		public void CartoonVoiceOversSelectionChanged()
		{

		}

		public void EpisodeVoiceOversSelectionChanged()
		{

		}

		public void SeasonSelectionChanged()
		{
			if(SelectedSeason == null)
			{
				return;
			}

			//StandartSelectedSettingsIds.SeasonId = SelectedSeason.CartoonSeasonId;
		}

		public void CancelSeasonSelection() { SelectedSeason = null; }

		public bool CanCancelSeasonSelection => SelectedSeason != null;


		public void EpisodeSelectionChanged()
		{

		}

		public void CancelEpisodeSelection() { SelectedEpisode = null; }

		public bool CanCancelEpisodeSelection => SelectedEpisode != null;

		public void AddCartoonVoiceOver()
		{

		}

		public void EditCartoonVoiceOver()
		{

		}

		public bool CanEditCartoonVoiceOver => SelectedCartoonVoiceOver != null;

		public void RemoveCartoonVoiceOver()
		{

		}

		public bool CanRemoveCartoonVoiceOver => SelectedCartoonVoiceOver != null;

		public void CancelCartoonVoiceOverSelection() { SelectedCartoonVoiceOver = null; }

		public bool CanCancelCartoonVoiceOverSelection => SelectedCartoonVoiceOver != null;

		public void EditEpisodeVoiceOver()
		{

		}

		public bool CanEditEpisodeVoiceOver => SelectedEpisodeVoiceOver != null;

		public void RemoveEpisodeVoiceOver()
		{

		}

		public bool CanRemoveEpisodeVoiceOver => SelectedEpisodeVoiceOver != null;

		public void CancelEpisodeVoiceOverSelection() { SelectedEpisodeVoiceOver = null; }

		public bool CanCancelEpisodeVoiceOverSelection => SelectedEpisodeVoiceOver != null;



		#endregion
	}
}
