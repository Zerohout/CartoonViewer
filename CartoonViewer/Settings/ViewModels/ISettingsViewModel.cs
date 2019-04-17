namespace CartoonViewer.Settings.ViewModels
{
	using Caliburn.Micro;

	public interface ISettingsViewModel : IScreen
	{
		bool HasChanges { get; }
		void LoadData();
		void SaveChanges();
	}
}
