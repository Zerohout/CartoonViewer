namespace CartoonViewer.ViewModels
{
	using Caliburn.Micro;

	public class SettingsViewModel : Conductor<Screen>.Collection.OneActive
	{
		public SettingsViewModel()
		{
			
		}

		private BindableCollection<Screen> _settings = new BindableCollection<Screen>();

		public BindableCollection<Screen> Settings
		{
			get => _settings;
			set
			{
				_settings = value;
				NotifyOfPropertyChange(() => Settings);
			}
		}
	}
}
