namespace CartoonViewer.Settings.ViewModels
{
	using Caliburn.Micro;
	using Models.CartoonModels;
	using static Helpers.Cloner;
	using static Helpers.Helper;

	public partial class SeasonsEditingViewModel : Screen, ISettingsViewModel
	{


		public SeasonsEditingViewModel(CartoonSeason cartoonSeason)
		{
			if(cartoonSeason.Name == NewElementString)
				return;

			_cartoonSeason = CloneSeason(cartoonSeason);
			_tempCartoonSeason = CloneSeason(cartoonSeason);

			SeasonId = cartoonSeason.CartoonSeasonId;

			LoadData();

			//_episodeTime = ConvertToEpisodeTime(SelectedCartoonEpisode);

		}

		public SeasonsEditingViewModel()
		{

		}
	}
}
