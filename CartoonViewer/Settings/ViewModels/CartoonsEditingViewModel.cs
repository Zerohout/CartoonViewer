namespace CartoonViewer.Settings.ViewModels
{
	using System.Linq;
	using System.Windows;
	using Models.CartoonModels;
	using static Helpers.Helper;
	using Screen = Caliburn.Micro.Screen;
	using static Helpers.Cloner;

	public partial class CartoonsEditingViewModel : Screen, ISettingsViewModel
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="cartoon"></param>
		public CartoonsEditingViewModel(Cartoon cartoon)
		{
			if(cartoon.Name == NewElementString)
			{
				SelectedCartoon = CloneCartoon(cartoon);
				
				TempCartoon = CloneCartoon(SelectedCartoon);
				SelectedCartoonUrl = new CartoonUrl
				{
					CartoonWebSiteId = GlobalIdList.WebSiteId
				};
				TempCartoonUrl = CloneCartoonUrl(SelectedCartoonUrl);
				//CreateNewCartoonVisibility = Visibility.Visible;
				//SaveChangesVisibility = Visibility.Hidden;
				return;
			}

			LoadData();
			//CreateNewCartoonVisibility = Visibility.Hidden;
			//SaveChangesVisibility = Visibility.Visible;
			NotifySeasonList();
		}

		

		public CartoonsEditingViewModel()
		{

		}











	}
}
