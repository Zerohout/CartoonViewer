namespace CartoonViewer.ViewModels
{
	using System.Diagnostics;
	using System.Windows;
	using Caliburn.Micro;
	using Helpers;
	using static Helpers.Helper;

	public class MainViewModel : Conductor<Screen>.Collection.OneActive
	{
		private WindowState _windowState = WindowState.Normal;
		private HotkeysRegistrator _hotReg;
		
		public MainViewModel()
		{
			
		}

		protected override void OnViewLoaded(object view)
		{
			_hotReg = new HotkeysRegistrator(GetView() as Window);
			
			ActiveItem = new MenuViewModel(_hotReg)
			{
				Parent = this
			};
			base.OnViewLoaded(view);
		}



		/// <summary>
		/// Состояние окна
		/// </summary>
		public WindowState WindowState
		{
			get => _windowState;
			set
			{
				_windowState = value;
				NotifyOfPropertyChange(() => WindowState);
			}
		}

		public void MouseDown()
		{
			(GetView() as Window)?.DragMove();
		}

		/// <summary>
		/// Закрытие программы
		/// </summary>
		public void Closing()
		{
			_hotReg.UnregisterHotkeys();
			Browser?.Quit();
		}
	}
}
