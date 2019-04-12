namespace CartoonViewer.Settings.ViewModels
{
	using System.Windows;
	using Caliburn.Micro;

	public interface ISettingsViewModel : IScreen
    {
	    bool HasChanges { get; }
		Visibility AdvancedSettingsVisibility { get; set; }

		void LoadData();
	    void SaveChanges();
    }
}
