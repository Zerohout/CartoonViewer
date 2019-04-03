namespace CartoonViewer.ViewModels
{
	using System.Windows;
	using Caliburn.Micro;
	using static Helpers.Helper;

	public class MainViewModel : Conductor<Screen>.Collection.OneActive
	{
		private WindowState _windowState = WindowState.Normal;
		
		public MainViewModel()
		{
			ActiveItem = new MenuViewModel
			{
				Parent = this
			};
		}

		protected override void OnInitialize()
		{
			
			base.OnInitialize();
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

		/// <summary>
		/// Закрытие программы
		/// </summary>
		public void Closing()
		{
			Browser?.Quit();
		}
	}
}
