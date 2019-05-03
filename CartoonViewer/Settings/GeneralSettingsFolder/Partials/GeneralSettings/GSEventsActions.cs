// ReSharper disable CheckNamespace
namespace CartoonViewer.Settings.GeneralSettingsFolder.ViewModels
{
	using System.Windows.Input;
	using Caliburn.Micro;
	using CartoonEditorFolder.ViewModels;
	using CartoonViewer.ViewModels;
	using Models.SettingModels;
	using static Helpers.Helper;
	using static Helpers.SettingsHelper;
	using static Helpers.ClassWriterReader;

	public partial class GeneralSettingsViewModel : Screen, ISettingsViewModel
	{
		public void ExpandCollapseNightHelperShutdownRemark()
		{
			if(GeneralValue.IsNightHelperShutdownRemarkExpand)
			{
				GeneralValue.IsNightHelperShutdownRemarkExpand = false;
				NotifyOfPropertyChange(() => GeneralValue);
			}
			else
			{
				GeneralValue.IsNightHelperShutdownRemarkExpand = true;
				NotifyOfPropertyChange(() => GeneralValue);
			}
		}

		public void TotalEpisodesReset()
		{
			var dvm = new DialogViewModel(
				"Внимание, данное действие сбросит дату последнего просмотра у ВСЕХ эпизодов.\nВы хотите продолжить?",
				DialogType.QUESTION);
			WinMan.ShowDialog(dvm);

			if(dvm.DialogResult == DialogResult.YES_ACTION)
			{
				//reset time
			}
		}

		public void SetDefaultValues()
		{
			var dvm = new DialogViewModel("Данная операция безвозвратна. Вы действительно хотите установить настройки по умолчанию?",
										  DialogType.QUESTION);
			WinMan.ShowDialog(dvm);

			if(dvm.DialogResult == DialogResult.NO_ACTION)
			{
				return;
			}

			GeneralValue = new GeneralSettingsValue();
			SaveChanges();
		}

		public bool CanSetDefaultValues => IsEquals(new GeneralSettingsValue(), GeneralValue) is false;

		public void CancelChanges()
		{
			GeneralValue = CloneObject<GeneralSettingsValue>(TempGeneralValue);

		}

		public bool CanCancelChanges => HasChanges;

		public void SaveChanges()
		{
			WriteClassInFile(GeneralValue, SavedGeneralSettingsFileName, GeneralSettingsFileExtension, AppDataPath);
			TempGeneralValue = CloneObject<GeneralSettingsValue>(GeneralValue);
			NotifyButtons();
		}

		public bool CanSaveChanges => HasChanges;

		public void ImportSettingsToFile()
		{

		}

		public bool CanImportSettingsToFile => IsEquals(new GeneralSettingsValue(), GeneralValue) is false;

		public void ExportSettingsFromFile()
		{

		}

		public bool CanExportSettingsFromFile => true;

		public void NumericValidation(KeyEventArgs e)
		{
			e.Handled = (e.Key.GetHashCode() >= 34 && e.Key.GetHashCode() <= 43 ||
						 e.Key.GetHashCode() >= 74 && e.Key.GetHashCode() <= 83) is false;
		}

		public void SelectionChanged()
		{
			NotifyOfPropertyChange(() => GeneralValue);
			NotifyButtons();
		}

		public void CheckedStatusChanged()
		{
			NotifyButtons();
		}

		public void TextChanged()
		{
			NotifyOfPropertyChange(() => GeneralValue);
			NotifyButtons();
		}
	}
}
