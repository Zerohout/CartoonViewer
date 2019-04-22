namespace CartoonViewer.Settings.GeneralSettingsFolder.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;
	using System.Windows.Input;

	public partial class GeneralSettingsViewModel
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
		private static readonly Regex _regex = new Regex("[^0-9.-]+");
		private static bool IsTextAllowed(string text)
		{
			return !_regex.IsMatch(text);
		}

		public void NumericValidation(KeyEventArgs e)
		{
			e.Handled = (e.Key.GetHashCode() >= 34 && e.Key.GetHashCode() <= 43 ||
			             e.Key.GetHashCode() >= 74 && e.Key.GetHashCode() <= 83) is false;
			return;
		}
	}
}
