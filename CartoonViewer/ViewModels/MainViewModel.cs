namespace CartoonViewer.ViewModels
{
	using System.Windows;
	using System.Windows.Forms;
	using System.Windows.Input;
	using Caliburn.Micro;
	using Helpers;
	using MainMenu.ViewModels;
	using static Helpers.Helper;
	using Application = System.Windows.Application;
	using Screen = Caliburn.Micro.Screen;

	public class MainViewModel : Conductor<Screen>.Collection.OneActive
	{
		private WindowState _windowState = WindowState.Normal;

		protected override void OnViewLoaded(object view)
		{
			HotReg = new HotkeysRegistrator(GetView() as Window);
			HotReg.RegisterGlobalHotkey(Exit, Keys.Pause, ModifierKeys.Shift);



			ActiveItem = new MainMenuViewModel()
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

		public void ChangeActiveItem(Screen viewModel)
		{
			ActiveItem?.TryClose();
			ActiveItem = viewModel;
		}

		public void MouseDown()
		{
			(GetView() as Window)?.DragMove();
		}

		/// <summary>
		/// Закрытие программы
		/// </summary>
		public void Exit()
		{
			ActiveItem?.TryClose();
			HotReg.UnregisterHotkeys();
			Browser?.Quit();
			Application.Current.Shutdown();
		}
	}
}
