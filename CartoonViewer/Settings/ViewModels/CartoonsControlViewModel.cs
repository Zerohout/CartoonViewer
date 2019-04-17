namespace CartoonViewer.Settings.ViewModels
{
	using System.Data.Entity;
	using Caliburn.Micro;
	using Database;
	using Models.CartoonModels;

	public partial class CartoonsControlViewModel : Conductor<Screen>.Collection.OneActive
	{
		public CartoonsControlViewModel()
		{

		}

		protected override void OnInitialize()
		{
			LoadListData();
			NotifyOfPropertyChange(() => CanCancelSeasonSelection);
			NotifyOfPropertyChange(() => CanCancelCartoonSelection);
			base.OnInitialize();
		}
	}
}
