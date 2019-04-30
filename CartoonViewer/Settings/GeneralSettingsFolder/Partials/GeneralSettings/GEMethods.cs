// ReSharper disable CheckNamespace
namespace CartoonViewer.Settings.GeneralSettingsFolder.ViewModels
{
	using System.Data.Entity;
	using System.Linq;
	using Caliburn.Micro;
	using CartoonEditorFolder.ViewModels;
	using Database;
	using Models.CartoonModels;
	using Models.SettingModels;
	using static Helpers.SettingsHelper;
	using static Helpers.ClassWriterReader;
	using static Helpers.Helper;

	public partial class GeneralSettingsViewModel : Screen, ISettingsViewModel
	{

		private void LoadData()
		{
			using(var ctx = new CVDbContext(AppDataPath))
			{
				GeneralValue = LoadGeneralSettings();
				var episodes = ctx.CartoonEpisodes
								  .Include(ce => ce.CartoonVoiceOver)
								  .Include(ce => ce.CartoonSeason)
								  .Include(ce => ce.EpisodeOptions)
								  .Include(ce => ce.Cartoon)
								  .ToList();
				Episodes = new BindableCollection<CartoonEpisode>(episodes);

				GeneralValue.AvailableEpisodesCount =
					ctx.CartoonEpisodes
					   .Include(ce => ce.CartoonVoiceOver)
					   .Include(ce => ce.CartoonSeason)
					   .Include(ce => ce.EpisodeOptions)
					   .Include(ce => ce.Cartoon)
					   .Where(ce => ce.Cartoon.Checked && ce.CartoonSeason.Checked)
					   .ToList().Count;


				WriteClassInFile(GeneralValue, DefaultGeneralSettingsFileName, GeneralSettingsFileExtension, AppDataPath);
				
				GeneralValue = LoadGeneralSettings();
				TempGeneralValue = CloneObject<GeneralSettingsValue>(GeneralValue);
				
				NotifyOfPropertyChange(() => GeneralValue);
				NotifyOfPropertyChange(() => CanSaveChanges);
				NotifyOfPropertyChange(() => CanCancelChanges);
			}


		}

		private void NotifyButtons()
		{
			NotifyOfPropertyChange(() => HasChanges);
			NotifyOfPropertyChange(() => CanSaveChanges);
			NotifyOfPropertyChange(() => CanCancelChanges);
			NotifyOfPropertyChange(() => CanImportSettingsToFile);
			NotifyOfPropertyChange(() => CanSetDefaultValues);
		}
	}
}
