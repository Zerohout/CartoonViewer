// ReSharper disable CheckNamespace
namespace CartoonViewer.Settings.GeneralSettingsFolder.ViewModels
{
	using System.Collections.Generic;
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
				GeneralSettings = LoadGeneralSettings();
				var episodes = ctx.CartoonEpisodes
								  .Include(ce => ce.CartoonVoiceOver)
								  .Include(ce => ce.CartoonSeason)
								  .Include(ce => ce.EpisodeOptions)
								  .Include(ce => ce.Cartoon)
								  .ToList();

				var cartoons = ctx.Cartoons
				                  .Include(c => c.CartoonEpisodes)
				                  .Where(c => c.CartoonEpisodes.Count > 0).ToList();

				Cartoons = new BindableCollection<Cartoon>(cartoons) {new Cartoon {Name = "всех"}};
				Episodes = new BindableCollection<CartoonEpisode>(episodes);
				
				GeneralSettings.AvailableEpisodesCount =
					ctx.CartoonEpisodes
					   .Include(ce => ce.CartoonVoiceOver)
					   .Include(ce => ce.CartoonSeason)
					   .Include(ce => ce.EpisodeOptions)
					   .Include(ce => ce.Cartoon)
					   .Where(ce => ce.Cartoon.Checked && ce.CartoonSeason.Checked)
					   .ToList().Count;


				WriteClassInFile(GeneralSettings, DefaultGeneralSettingsFileName, GeneralSettingsFileExtension, AppDataPath);
				
				GeneralSettings = LoadGeneralSettings();
				TempGeneralSettings = CloneObject<GeneralSettingsValue>(GeneralSettings);
				
				NotifyOfPropertyChange(() => GeneralSettings);
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
