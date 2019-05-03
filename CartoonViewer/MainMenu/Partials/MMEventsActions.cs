// ReSharper disable once CheckNamespace
namespace CartoonViewer.MainMenu.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.Data.Entity;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Windows;
	using System.Windows.Input;
	using CartoonViewer.ViewModels;
	using Helpers;
	using Models.CartoonModels;
	using Settings.CartoonEditorFolder.ViewModels;
	using Settings.SettingsFolder.ViewModels;
	using static Helpers.Helper;
	using static Helpers.SettingsHelper;
	using static Helpers.ClassWriterReader;
	using Screen = Caliburn.Micro.Screen;

	public partial class MainMenuViewModel : Screen
	{
		/// <summary>
		/// Действие при выходе
		/// </summary>
		public void Exit()
		{
			((MainViewModel)Parent).Exit();
		}

		public void TextChanged()
		{
			NotifyOfPropertyChange(() => GeneralSettings);
			NotifyOfPropertyChange(() => EndDate);
			NotifyOfPropertyChange(() => EndTime);
		}

		public void NumericValidation(KeyEventArgs e)
		{
			e.Handled = (e.Key.GetHashCode() >= 34 && e.Key.GetHashCode() <= 43 ||
						 e.Key.GetHashCode() >= 74 && e.Key.GetHashCode() <= 83) is false;

		}

		/// <summary>
		/// Попадание курсора на кнопку выхода
		/// </summary>
		public void CursorOnExit()
		{
			Background = new Uri(MMBackgroundOnExitUri, UriKind.Relative);
		}

		/// <summary>
		/// Выход курсора за пределы кнопки выхода
		/// </summary>
		public void CursorOutsideExit()
		{
			Background = new Uri(MMBackgroundUri, UriKind.Relative);
		}

		/// <summary>
		/// Действие при старте просмотра
		/// </summary>
		public async void Start()
		{
			if (CanStart is false) return;

			var cartoonsCount = Cartoons.Count(c => c.Checked);

			if (GeneralSettings.WatchingInRow is true &&
			    cartoonsCount > 1)
			{
				WinMan.ShowDialog(new DialogViewModel(
					                  "На данный момент нельзя смотреть подряд эпизоды, если выбрано более одного мультсериала."
					                  , DialogType.INFO));
				return;
			}

			((MainViewModel)Parent).WindowState = WindowState.Minimized;

			StartBrowser();

			await Task.Run(StartWatch);
		}

		public void GoToSettings()
		{
			((MainViewModel)Parent).ChangeActiveItem(new SettingsViewModel());
		}

		public bool CanStart => CheckedEpisodes.Count > 0;

		/// <summary>
		/// Действие при выборе/снятия выбора с мультсериала
		/// </summary>
		public void CheckedValidation()
		{
			if(CvDbContext.ChangeTracker.HasChanges())
			{
				CvDbContext.SaveChanges();
			}

			CheckedEpisodes = new List<CartoonEpisode>(
				CvDbContext.CartoonEpisodes
						   .Include(ce => ce.CartoonVoiceOver)
						   .Include(ce => ce.CartoonSeason)
						   .Include(ce => ce.EpisodeOptions)
						   .Include(ce => ce.Cartoon)
						   .Where(ce => ce.Cartoon.Checked && ce.CartoonSeason.Checked && ce.Checked));

			if(CheckedEpisodes.Count > 0)
			{
				FilterCheckedEpisodes();
			}

			GeneralSettings.AvailableEpisodesCount = CheckedEpisodes.Count;
			WriteClassInFile(GeneralSettings,SavedGeneralSettingsFileName,GeneralSettingsFileExtension,AppDataPath);

			NotifyOfPropertyChange(() => GeneralSettings);
			NotifyOfPropertyChange(() => CanStart);
		}
		/// <summary>
		/// Отфильтровать серии от просмотренных и не выбранных
		/// </summary>
		private void FilterCheckedEpisodes()
		{
			var count = 0;
			var approximateTime = new TimeSpan();
			foreach(var ce in CheckedEpisodes.ToList())
			{
				var option = ce.EpisodeOptions
							   .FirstOrDefault(eo => eo.CartoonVoiceOverId == ce.CartoonVoiceOver.CartoonVoiceOverId);

				if(option == null)
				{
					CheckedEpisodes.Remove(ce);

					continue;
				}

				if(DateTime.Now.Subtract(option.LastDateViewed) >
				   GeneralSettings.NonRepeatTime)
				{
					approximateTime += option.Duration;
					count++;
					continue;
				}

				CheckedEpisodes.Remove(ce);
			}

			ApproximateEpisodeDuration =
				new TimeSpan(0, 0,
							 (int)(approximateTime.TotalSeconds / count));

			NotifyOfPropertyChange(() => EndTime);
			NotifyOfPropertyChange(() => EndDate);

		}


		
	}
}
