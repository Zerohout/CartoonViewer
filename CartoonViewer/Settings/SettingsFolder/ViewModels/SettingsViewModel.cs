namespace CartoonViewer.Settings.SettingsFolder.ViewModels
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Windows.Input;
	using Caliburn.Micro;
	using CartoonEditorFolder.ViewModels;
	using CartoonViewer.ViewModels;
	using Database;
	using GeneralSettingsFolder.ViewModels;
	using Helpers;
	using MainMenu.ViewModels;
	using ViewingsSettingsFolder.ViewModels;


	public class SettingsViewModel : Conductor<Screen>.Collection.OneActive
	{
		private int currentWebSiteId;
		
		public SettingsViewModel()
		{
			
		}

		protected override void OnInitialize()
		{
			using (var ctx = new CVDbContext(SettingsHelper.AppDataPath))
			{
				currentWebSiteId = ctx.CartoonWebSites.First().CartoonWebSiteId;
			}
			AddSettingsToList();
			base.OnInitialize();
		}

		private void AddSettingsToList()
		{
			Settings.Clear();
			Settings.AddRange(new List<Screen>
			{
				new GeneralSettingsViewModel{DisplayName = "Основные настройки", Parent = this},
				new CartoonsEditorViewModel{DisplayName = "Редактор мультсериалов", Parent = this},
				new VoiceOversEditingViewModel(currentWebSiteId,0)
				{
					DisplayName = "Редактор озвучек",Parent = this
				},
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

		//private Screen lastActiveItem;

		public void SelectionChanged()
		{
			if (ActiveItem == null) return;

			if (ActiveItem is CartoonsEditorViewModel cevm)
			{
				if (cevm.ActiveItem == null) return;

				if (cevm.ActiveItem is CartoonsEditingViewModel ce)
				{
					ce.UpdateVoiceOverList();
					return;
				}

				if (cevm.ActiveItem is EpisodesEditingViewModel ee)
				{
					ee.UpdateVoiceOverList();
				}

				return;
			}

			if (ActiveItem is VoiceOversEditingViewModel voe)
			{
				voe.UpdateVoiceOverList();
			}
		}

		public void BackToMainMenu()
		{
			foreach(var activeItem in Settings)
			{
				if(activeItem is CartoonsEditorViewModel ce)
				{
					if(ce.ActiveItem is ISettingsViewModel CEsvm)
					{
						if(CancelBackToMainMenu(CEsvm) is true)
						{
							return;
						}
					}
				}

				if(activeItem is ISettingsViewModel svm)
				{
					if(CancelBackToMainMenu(svm) is true)
					{
						return;
					}
				}
			}

			ActiveItem?.TryClose();
			((MainViewModel)Parent).ChangeActiveItem(new MainMenuViewModel());
		}

		private bool CancelBackToMainMenu(ISettingsViewModel svm)
		{
			if(svm.HasChanges)
			{
				var dvm = new DialogViewModel($"В меню \"{svm.DisplayName}\" имеются не сохраненные изменения\nСохранить их?",
											  Helper.DialogType.SAVE_CHANGES);

				_ = Helper.WinMan.ShowDialog(dvm);

				switch(dvm.DialogResult)
				{
					case Helper.DialogResult.YES_ACTION:
						svm.SaveChanges();
						svm.TryClose();
						break;
					case Helper.DialogResult.NO_ACTION:
						svm.TryClose();
						break;
					default:
						return true;
				}

				return false;
			}

			return false;
		}
	}
}
