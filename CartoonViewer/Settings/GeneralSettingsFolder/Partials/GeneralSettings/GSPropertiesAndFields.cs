﻿// ReSharper disable CheckNamespace
namespace CartoonViewer.Settings.GeneralSettingsFolder.ViewModels
{
	using Caliburn.Micro;
	using CartoonEditorFolder.ViewModels;
	using Models.SettingModels;
	using static Helpers.Helper;

	public partial class GeneralSettingsViewModel : Screen, ISettingsViewModel
	{
		private GeneralSettingsValue _generalValue;
		private GeneralSettingsValue _tempGeneralValue;

		public GeneralSettingsValue TempGeneralValue
		{
			get => _tempGeneralValue;
			set
			{
				_tempGeneralValue = value;
				NotifyOfPropertyChange(() => TempGeneralValue);
			}
		}

		public GeneralSettingsValue GeneralValue
		{
			get => _generalValue;
			set
			{
				_generalValue = value;
				NotifyOfPropertyChange(() => GeneralValue);
			}
		}

		public bool HasChanges => IsEquals(GeneralValue, TempGeneralValue) is false;
	}
}
