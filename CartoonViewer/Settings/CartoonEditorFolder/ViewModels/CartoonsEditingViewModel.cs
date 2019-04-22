namespace CartoonViewer.Settings.CartoonEditorSetting.ViewModels
{
	using Helpers;
	using Models.CartoonModels;
	using Screen = Caliburn.Micro.Screen;

	public partial class CartoonsEditingViewModel : Screen, ISettingsViewModel
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="cartoon"></param>
		public CartoonsEditingViewModel(Cartoon cartoon)
		{
			if(cartoon.Name == Helper.NewElementString)
			{
				SelectedCartoon = Cloner.CloneCartoon(cartoon);
				
				TempCartoon = Cloner.CloneCartoon(SelectedCartoon);
				SelectedCartoonUrl = new CartoonUrl
				{
					CartoonWebSiteId = Helper.GlobalIdList.WebSiteId
				};
				TempCartoonUrl = Cloner.CloneCartoonUrl(SelectedCartoonUrl);
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
