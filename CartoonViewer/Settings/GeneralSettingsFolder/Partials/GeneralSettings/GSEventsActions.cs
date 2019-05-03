// ReSharper disable CheckNamespace
namespace CartoonViewer.Settings.GeneralSettingsFolder.ViewModels
{
	using System.Collections.Generic;
	using System.Data.Entity;
	using System.Linq;
	using System.Windows.Input;
	using Caliburn.Micro;
	using CartoonEditorFolder.ViewModels;
	using CartoonViewer.ViewModels;
	using Database;
	using Models.CartoonModels;
	using Models.SettingModels;
	using static Helpers.Helper;
	using static Helpers.SettingsHelper;
	using static Helpers.ClassWriterReader;

	public partial class GeneralSettingsViewModel : Screen, ISettingsViewModel
	{
		public void ExpandCollapseNightHelperShutdownRemark()
		{
			if(GeneralSettings.IsNightHelperShutdownRemarkExpand)
			{
				GeneralSettings.IsNightHelperShutdownRemarkExpand = false;
				NotifyOfPropertyChange(() => GeneralSettings);
			}
			else
			{
				GeneralSettings.IsNightHelperShutdownRemarkExpand = true;
				NotifyOfPropertyChange(() => GeneralSettings);
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

			GeneralSettings = new GeneralSettingsValue();
			SaveChanges();
		}

		public bool CanSetDefaultValues => IsEquals(new GeneralSettingsValue(), GeneralSettings) is false;

		public void CancelChanges()
		{
			GeneralSettings = CloneObject<GeneralSettingsValue>(TempGeneralSettings);

		}

		public bool CanCancelChanges => HasChanges;

		public void SaveChanges()
		{
			WriteClassInFile(GeneralSettings, SavedGeneralSettingsFileName, GeneralSettingsFileExtension, AppDataPath);
			TempGeneralSettings = CloneObject<GeneralSettingsValue>(GeneralSettings);
			NotifyButtons();
		}

		public bool CanSaveChanges => HasChanges;

		public void ResetLastDateViewed()
		{
			if (CanResetLastDateViewed is false) return;

			using(var ctx = new CVDbContext(AppDataPath))
			{
				var dvm = new DialogViewModel("Вы уверены что хотите сбросить дату последнего просмотра? Эта операция необратима.",
					DialogType.QUESTION);

				WinMan.ShowDialog(dvm);

				switch (dvm.DialogResult)
				{
					case DialogResult.YES_ACTION:
						break;
					default:
						return;
				}


				List<CartoonEpisode> episodes;

				if (IsSelectedAllCartoonsToReset)
				{
					episodes = new List<CartoonEpisode>(ctx.CartoonEpisodes
														.Include(ce => ce.EpisodeOptions)
														.Include(ce => ce.CartoonVoiceOver));
				}
				else
				{
					episodes = new List<CartoonEpisode>(ctx.CartoonEpisodes
					              .Include(ce => ce.EpisodeOptions)
					              .Include(ce => ce.CartoonVoiceOver)
					              .Where(ce => ce.CartoonId == SelectedGlobalResetCartoon.CartoonId));
				}

				var succesfull = false;

				foreach (var episode in episodes)
				{
					var option = episode.EpisodeOptions
					                    .FirstOrDefault(eo => eo.CartoonVoiceOverId ==
					                                 episode.CartoonVoiceOver.CartoonVoiceOverId && 
					                                 eo.LastDateViewed > ResetTime);
					if(option == null) continue;

					option.LastDateViewed = ResetTime;
					ctx.SaveChanges();
					succesfull = true;
				}

				WinMan.ShowDialog(new DialogViewModel(succesfull ? "Сброс успешно завершен."
				                                      : "Дата уже сброшена.", DialogType.INFO));
			}

		}

		public bool CanResetLastDateViewed => SelectedGlobalResetCartoon != null;

		public void ImportSettingsToFile()
		{

		}

		public bool CanImportSettingsToFile => IsEquals(new GeneralSettingsValue(), GeneralSettings) is false;

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
			NotifyOfPropertyChange(() => GeneralSettings);
			NotifyButtons();
		}

		public void CheckedStatusChanged()
		{
			NotifyButtons();
		}

		public void TextChanged()
		{
			NotifyOfPropertyChange(() => GeneralSettings);
			NotifyButtons();
		}
	}
}
