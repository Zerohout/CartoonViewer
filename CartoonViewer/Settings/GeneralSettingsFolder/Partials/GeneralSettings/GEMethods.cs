// ReSharper disable CheckNamespace
namespace CartoonViewer.Settings.GeneralSettingsFolder.ViewModels
{
	using Caliburn.Micro;
	using CartoonEditorFolder.ViewModels;

	public partial class GeneralSettingsViewModel : Screen, ISettingsViewModel
	{
		private void NotifyButtons()
		{
			NotifyOfPropertyChange(() => HasChanges);
			NotifyOfPropertyChange(() => CanSaveChanges);
			NotifyOfPropertyChange(() => CanCancelChanges);
			NotifyOfPropertyChange(() => CanImportSettingsToFile);
			NotifyOfPropertyChange(() => CanSetDefaultValues);
		}
	}
}
