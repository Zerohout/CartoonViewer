namespace CartoonViewer.Settings.ViewModels
{
	using Caliburn.Micro;
	using Models.CartoonModels;
	using static Helpers.Cloner;
	using static Helpers.Helper;

	public partial class EpisodesEditingViewModel : Screen, ISettingsViewModel
	{


		public EpisodesEditingViewModel()
		{
			
		}

		protected override void OnInitialize()
		{
			LoadSeasonData();
			base.OnInitialize();
		}
	}
}
