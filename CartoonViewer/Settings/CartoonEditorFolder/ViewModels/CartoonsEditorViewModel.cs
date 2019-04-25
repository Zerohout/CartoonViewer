namespace CartoonViewer.Settings.CartoonEditorFolder.ViewModels
{
	using Caliburn.Micro;

	public partial class CartoonsEditorViewModel : Conductor<Screen>.Collection.OneActive
	{
		public CartoonsEditorViewModel()
		{

		}

		protected override void OnInitialize()
		{
			LoadList();
			NotifyOfPropertyChange(() => CanCancelSeasonSelection);
			NotifyOfPropertyChange(() => CanCancelCartoonSelection);
			base.OnInitialize();
		}
	}
}
