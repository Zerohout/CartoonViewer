// ReSharper disable CheckNamespace
namespace CartoonViewer.Settings.GeneralSettingsFolder.ViewModels
{
	using System.Windows.Input;
	using Caliburn.Micro;
	using CartoonEditorFolder.ViewModels;
	using Models.SettingModels;
	using static Helpers.Helper;

	public partial class GeneralSettingsViewModel : Screen, ISettingsViewModel
	{
		public void ExpandCollapseIntellectualShutdownRemark()
		{
			if(GeneralValue.IsIntellectualShutdownRemarkExpand)
			{
				GeneralValue.IsIntellectualShutdownRemarkExpand = false;
				NotifyOfPropertyChange(() => GeneralValue);
			}
			else
			{
				GeneralValue.IsIntellectualShutdownRemarkExpand = true;
				NotifyOfPropertyChange(() => GeneralValue);
			}
		}

		public void SetDefaultValues()
		{

		}

		public bool CanSetDefaultValues => IsEquals(new GeneralSettingsValue(), GeneralValue) is false;

		public void CancelChanges()
		{

		}

		public bool CanCancelChanges => HasChanges;

		public void SaveChanges()
		{

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
			NotifyButtons();
		}

		public void CheckedStatusChanged()
		{
			NotifyButtons();
		}

		public void TextChanged()
		{
			NotifyButtons();
		}
	}
}
