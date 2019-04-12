namespace CartoonViewer.Settings.ViewModels
{
	using System.Collections.Generic;
	using System.Windows.Input;
	using Caliburn.Micro;
	using CartoonViewer.ViewModels;
	using Helpers;
	using MainMenu.ViewModels;
	using static Helpers.Helper;

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
			var settings = ((CartoonsControlViewModel) ActiveItem).ActiveItem as ISettingsViewModel;



			if (settings?.HasChanges ?? false)
			{
				var result = WinMan.ShowDialog(new DialogViewModel("Сохранить ваши изменения?", Helper.DialogState.YES_NO_CANCEL));

				if (result == true)
				{
					settings.SaveChanges();
				}
				else if (result == false)
				{
					var repeatResult = WinMan.ShowDialog(
						new DialogViewModel("Ваши изменения не будут сохранены. Вы точно хотите продолжить?", Helper.DialogState.YES_NO));
					if (repeatResult == false || repeatResult == null)
					{
						return;
					}
				}
				else
				{
					return;
				}

			}



			((CartoonsControlViewModel) ActiveItem)?.ActiveItem?.TryClose();
			ActiveItem?.TryClose();
			((MainViewModel)Parent).ChangeActiveItem(new MainMenuViewModel());
		}
		
	}
}
