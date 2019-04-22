namespace CartoonViewer.Settings.CartoonEditorSetting.ViewModels
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
			NotifyOfPropertyChange<bool>(() => CanCancelSeasonSelection);
			NotifyOfPropertyChange<bool>(() => CanCancelCartoonSelection);
			base.OnInitialize();
		}
	}
}
