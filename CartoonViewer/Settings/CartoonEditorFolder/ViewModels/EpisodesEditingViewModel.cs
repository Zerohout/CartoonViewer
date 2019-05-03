namespace CartoonViewer.Settings.CartoonEditorFolder.ViewModels
{
	using Caliburn.Micro;

	public partial class EpisodesEditingViewModel : Screen, ISettingsViewModel
	{
		public EpisodesEditingViewModel()
		{

		}

		protected override void OnInitialize()
		{
			DisplayName = "Редактор эпизодов";
			LoadSeasonData();
			UpdateCartoonVoiceOvers();
			base.OnInitialize();
		}
	}
}
