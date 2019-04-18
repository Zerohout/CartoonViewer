namespace CartoonViewer.Settings.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.Windows;
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

		public void BackToMainMenu(EventArgs ea)
		{
			var settings = ((CartoonsControlViewModel) ActiveItem)?.ActiveItem as ISettingsViewModel;

			if (settings?.HasChanges ?? false)
			{
				var vm = new DialogViewModel(null, DialogState.SAVE_CHANGES);

				_ = WinMan.ShowDialog(vm);


				switch (vm.DialogResult)
				{
					case DialogResult.YES_ACTION:
						settings.SaveChanges();
						settings.TryClose();
						break;
					case DialogResult.NO_ACTION:
						settings.TryClose();
						break;
					default:
						((RoutedEventArgs)ea).Handled = true;
						return;
				}
			}
			
			ActiveItem?.TryClose();
			((MainViewModel)Parent).ChangeActiveItem(new MainMenuViewModel());
		}
	}
}
