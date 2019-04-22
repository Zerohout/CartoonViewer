namespace CartoonViewer.Settings.CartoonEditorSetting.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.Windows;
	using System.Windows.Input;
	using Caliburn.Micro;
	using CartoonViewer.ViewModels;
	using GeneralSettingsFolder.ViewModels;
	using Helpers;
	using MainMenu.ViewModels;
	using ViewingsSettingsFolder.ViewModels;

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
				new CartoonsEditorViewModel{DisplayName = "Редактор мультсериалов", Parent = this},
				new ViewingsSettingsViewModel{DisplayName = "Настройки просмотра", Parent = this}
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

		public void SelectionChanged(MouseButtonEventArgs eventArgs)
		{





			//eventArgs.Handled = true;
		}



		public void BackToMainMenu(EventArgs ea)
		{
			if(ActiveItem == null)
				return;

			if(ActiveItem is CartoonsEditorViewModel)
			{
				var ccvm = (CartoonsEditorViewModel)ActiveItem;

				var settings = ccvm?.ActiveItem as ISettingsViewModel;

				if(settings?.HasChanges ?? false)
				{
					var vm = new DialogViewModel(null, Helper.DialogState.SAVE_CHANGES);

					_ = Helper.WinMan.ShowDialog(vm);

					switch(vm.DialogResult)
					{
						case Helper.DialogResult.YES_ACTION:
							settings.SaveChanges();
							settings.TryClose();
							break;
						case Helper.DialogResult.NO_ACTION:
							settings.TryClose();
							break;
						default:
							((RoutedEventArgs)ea).Handled = true;
							return;
					}
				}

			}

			ActiveItem?.TryClose();
			((MainViewModel)Parent).ChangeActiveItem(new MainMenuViewModel());
		}
	}
}
