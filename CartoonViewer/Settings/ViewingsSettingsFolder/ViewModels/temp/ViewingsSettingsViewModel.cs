namespace CartoonViewer.Settings.ViewingsSettingsFolder.ViewModels
{
	using Caliburn.Micro;

	public partial class ViewingsSettingsViewModel : Screen
	{
		public ViewingsSettingsViewModel()
		{
			
		}

		protected override void OnInitialize()
		{
			LoadData();
			base.OnInitialize();
		}
	}
}
