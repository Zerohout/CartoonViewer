namespace CartoonViewer.ViewModels.SettingsViewModels
{
	using Caliburn.Micro;
	using Models.CartoonModels;

	public class AddCartoonViewModel : Screen
	{
		public AddCartoonViewModel(Cartoon cartoon)
		{
			_cartoon = cartoon;
		}

		public AddCartoonViewModel()
		{

		}

		private Cartoon _cartoon = new Cartoon();

		public Cartoon Cartoon
		{
			get => _cartoon;
			set
			{
				_cartoon = value;
				NotifyOfPropertyChange(() => Cartoon);
			}
		}


	}
}
