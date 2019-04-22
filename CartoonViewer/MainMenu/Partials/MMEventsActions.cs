// ReSharper disable once CheckNamespace
namespace CartoonViewer.MainMenu.ViewModels
{
	using System;
	using System.Data.Entity;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Windows;
	using Caliburn.Micro;
	using CartoonViewer.ViewModels;
	using Database;
	using Helpers;
	using Settings.CartoonEditorSetting.ViewModels;
	using ViewModels;
	using Action = System.Action;
	using static Helpers.Helper;

	public partial class MainMenuViewModel : Screen
	{
		/// <summary>
		/// Действие при выходе
		/// </summary>
		public void Exit()
		{
			((MainViewModel)Parent).Exit();
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

			Helper.StartBrowser();

			await Task.Run((Action) (() => StartWatch()));
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

		//private void SetDefaultValues()
		//{
		//	foreach (var c in Cartoons)
		//	{
		//		if ((c.Name == "Южный парк" || c.Name == "Гриффины") && !c.Checked)
		//		{
		//			c.Checked = true;
		//			using (var ctx = new CVDbContext())
		//			{
		//				ctx.Entry(c).State = EntityState.Modified;
		//				ctx.SaveChanges();
		//			}
					
		//		}
		//	}
		//}
	}
}
