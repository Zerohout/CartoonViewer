// ReSharper disable once CheckNamespace
namespace CartoonViewer.MainMenu.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Windows;
	using System.Windows.Input;
	using CartoonViewer.ViewModels;
	using Models.CartoonModels;
	using Settings.CartoonEditorFolder.ViewModels;
	using static Helpers.Helper;
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
			((MainViewModel)Parent).WindowState = WindowState.Minimized;

			StartBrowser();

			await Task.Run(StartWatch);
		}

		public void GoToSettings()
		{
			((MainViewModel)Parent).ChangeActiveItem(new SettingsViewModel());
		}

		public bool CanStart => CheckedCartoons.Count > 0;

		/// <summary>
		/// Действие при выборе/снятия выбора с мультсериала
		/// </summary>
		public void CheckedValidation()
		{
			if (CvDbContext.ChangeTracker.HasChanges())
			{
				CvDbContext.SaveChanges();
			}

			CheckedEpisodes = new List<CartoonEpisode>(
				CvDbContext.CartoonEpisodes
						   .Where(ce => ce.CartoonSeason.Cartoon.Checked));

			if(CheckedEpisodes.Count > 0)
			{
				FilterCheckedEpisodes();
			}

			GeneralSettings.AvailableEpisodesCount = CheckedEpisodes.Count;



			//CheckedCartoons.Clear();
			//CheckedCartoons.AddRange(Cartoons.Where(c => c.Checked));
			NotifyOfPropertyChange(() => CanStart);
		}

		private void FilterCheckedEpisodes()
		{
			var count = 0;
			var approximateTime = new TimeSpan();
			foreach(var ce in CheckedEpisodes.ToList())
			{
				if(DateTime.Now.Subtract(ce.LastDateViewed) >
				   GeneralSettings.NonRepeatTime)
				{
					approximateTime += ce.Duration;
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
			NotifyOfPropertyChange(() => GeneralSettings.EpisodesCount);
		}


		public List<CartoonEpisode> CheckedEpisodes { get; set; } = new List<CartoonEpisode>();
	}
}
