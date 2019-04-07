// ReSharper disable once CheckNamespace
namespace CartoonViewer.ViewModels
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Windows;
	using Caliburn.Micro;
	using static Helpers.Helper;

	public partial class MainMenuViewModel : Screen
	{
		/// <summary>
		/// Действие при выходе
		/// </summary>
		public void Exit()
		{
			((MainViewModel)Parent).TryClose();
		}

		///// <summary>
		///// Действие при паузе
		///// </summary>
		//public void Pause()
		//{
		//	IsPause = !IsPause;
		//}

		/// <summary>
		/// Попадание курсора на кнопку выхода
		/// </summary>
		public void CursorOnExit()
		{
			Background = new Uri("../Resources/HDMMBackgroundOnExit.png", UriKind.Relative);
		}

		/// <summary>
		/// Выход курсора за пределы кнопки выхода
		/// </summary>
		public void CursorOutsideExit()
		{
			Background = new Uri("../Resources/HDMMBackground.png", UriKind.Relative);
		}

		/// <summary>
		/// Действие при старте просмотра
		/// </summary>
		public async void Start()
		{
			((MainViewModel)Parent).WindowState = WindowState.Minimized;

			StartBrowser();

			await Task.Run(() => StartBrowsing());
		}

		public bool CanStart => CheckedCartoons.Count > 0;

		/// <summary>
		/// Действие при выборе/снятия выбора с мультфильма
		/// </summary>
		public void CheckedValidation()
		{
			CheckedCartoons.Clear();
			CheckedCartoons.AddRange(Cartoons.Where(c => c.Checked));
			NotifyOfPropertyChange(() => CanStart);
		}

		/// <summary>
		/// Оповещение свойств
		/// </summary>
		private void NotifyEpisodesTime()
		{
			NotifyOfPropertyChange(() => EpisodeCountString);
			NotifyOfPropertyChange(() => FinalYear);
			NotifyOfPropertyChange(() => FinalMonth);
			NotifyOfPropertyChange(() => FinalDay);
			NotifyOfPropertyChange(() => FinalHour);
			NotifyOfPropertyChange(() => FinalMinute);
		}

		private void SetDefaultValues()
		{
			foreach (var c in Cartoons)
			{
				if (c.Name == "Южный парк" || c.Name == "Гриффины")
				{
					c.Checked = true;
				}
			}

			CheckedValidation();
			
			NotifyOfPropertyChange(() => Cartoons);
		}
	}
}
