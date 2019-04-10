namespace CartoonViewer.Settings.ViewModels
{
	using System.Collections.Generic;
	using System.Windows.Input;
	using Caliburn.Micro;
	using CartoonViewer.ViewModels;
	using MainMenu.ViewModels;

	public class SettingsViewModel : Conductor<Screen>.Collection.OneActive
	{
		public SettingsViewModel()
		{
			AddSettingsToList();
		}


		private void AddSettingsToList()
		{
			Settings.Clear();
			Settings.AddRange(new List<Screen>
			{
				new GeneralSettingsViewModel{DisplayName = "Основные настройки", Parent = this},
				new CartoonsControlViewModel{DisplayName = "Настройки мультфильмов", Parent = this}
			});
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

		public void ChangeActiveItem(Screen viewModel)
		{
			ActiveItem?.TryClose();
			ActiveItem = viewModel;
		}

		public void BackToMainMenu()
		{
			ActiveItem?.TryClose();
			((MainViewModel)Parent).ChangeActiveItem(new MainMenuViewModel());
		}
		
	}
}
