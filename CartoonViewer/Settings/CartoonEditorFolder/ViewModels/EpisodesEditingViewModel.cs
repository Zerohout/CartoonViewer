namespace CartoonViewer.Settings.CartoonEditorSetting.ViewModels
{
	using Caliburn.Micro;

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
