namespace CartoonViewer.Settings.CartoonEditorSetting.ViewModels
{
	using Caliburn.Micro;

	public interface ISettingsViewModel : IScreen
	{
		//TODO: Дополнить интерфейс общими методами. 
		//TODO: Переименовать в ICartoonEditorSetting
		bool HasChanges { get; }
		//void LoadData();
		void SaveChanges();
	}
}
