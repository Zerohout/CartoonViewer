// ReSharper disable CheckNamespace
namespace CartoonViewer.Settings.GeneralSettingsFolder.ViewModels
{
	using System.Collections.Generic;
	using Caliburn.Micro;
	using CartoonEditorFolder.ViewModels;
	using Models.CartoonModels;
	using Models.SettingModels;
	using static Helpers.Helper;

	public partial class GeneralSettingsViewModel : Screen, ISettingsViewModel
	{
		private GeneralSettingsValue _generalSettings;
		private GeneralSettingsValue _tempGeneralSettings;

		private BindableCollection<CartoonEpisode> _episodes;
		/// <summary>
		/// Список эпизодов
		/// </summary>
		public BindableCollection<CartoonEpisode> Episodes
		{
			get => _episodes;
			set
			{
				_episodes = value;
				NotifyOfPropertyChange(() => Episodes);
			}
		}

		/// <summary>
		/// Временный экземпляр настроек для фиксации изменений
		/// </summary>
		public GeneralSettingsValue TempGeneralSettings
		{
			get => _tempGeneralSettings;
			set
			{
				_tempGeneralSettings = value;
				NotifyOfPropertyChange(() => TempGeneralSettings);
			}
		}
		/// <summary>
		/// Общие настройки
		/// </summary>
		public GeneralSettingsValue GeneralSettings
		{
			get => _generalSettings;
			set
			{
				_generalSettings = value;
				NotifyOfPropertyChange(() => GeneralSettings);
			}
		}
		/// <summary>
		/// Список м/с
		/// </summary>
		private BindableCollection<Cartoon> _cartoons = new BindableCollection<Cartoon>();

		public BindableCollection<Cartoon> Cartoons
		{
			get => _cartoons;
			set
			{
				_cartoons = value;
				NotifyOfPropertyChange(() => Cartoons);
			}
		}


		private bool _isSelectedAllCartoonsToReset;

		public bool IsSelectedAllCartoonsToReset
		{
			get => _isSelectedAllCartoonsToReset;
			set
			{
				_isSelectedAllCartoonsToReset = value;
				NotifyOfPropertyChange(() => IsSelectedAllCartoonsToReset);
			}
		}



		private Cartoon _selectedGlobalResetType;
		/// <summary>
		/// Выбранный м/с для сброса необходимых данных 
		/// </summary>
		public Cartoon SelectedGlobalResetCartoon
		{
			get => _selectedGlobalResetType;
			set
			{
				if (value == null)
				{
					return;
				}


				IsSelectedAllCartoonsToReset = value.Name == "всех";


				_selectedGlobalResetType = value;
				NotifyOfPropertyChange(() => SelectedGlobalResetCartoon);
				NotifyOfPropertyChange(() => CanResetLastDateViewed);
			}
		}


		public bool HasChanges => IsEquals(GeneralSettings, TempGeneralSettings) is false;
	}
}
