namespace CartoonViewer.Settings.ViewModels
{
	using System.Windows;
	using Caliburn.Micro;

	public interface ISettingsViewModel : IScreen
    {
	    bool HasChanges { get; set; }
		Visibility AdvancedSettingsVisibility { get; set; }

		void LoadDataAsync(int id);
    }
}
