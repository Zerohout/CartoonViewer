namespace CartoonViewer.Settings.GeneralSettingsFolder.ViewModels
{
	using Caliburn.Micro;
	using CartoonEditorFolder.ViewModels;
	using Models.SettingModels;
	using static Helpers.Helper;

	public partial class GeneralSettingsViewModel : Screen, ISettingsViewModel
	{
		public GeneralSettingsViewModel()
		{

		}

		protected override void OnInitialize()
		{
			GeneralValue = LoadGeneralSettings();
			TempGeneralValue = GeneralValue.Clone() as GeneralSettingsValue;

			base.OnInitialize();
		}





	}


}
