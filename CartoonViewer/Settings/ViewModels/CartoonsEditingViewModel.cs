namespace CartoonViewer.Settings.ViewModels
{
	using System.Windows;
	using Models.CartoonModels;
	using static Helpers.Helper;
	using Screen = Caliburn.Micro.Screen;

	public partial class CartoonsEditingViewModel : Screen, ISettingsViewModel
	{
		//TODO Изменить значения имени, адреса и описания в свойства класса Cartoon (Cartoon.Name etc.)

		// Комментарий

		#region Регион

		#endregion

		#region MyRegion

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cartoon"></param>
		/// <param name="webSiteId"></param>
		public CartoonsEditingViewModel(Cartoon cartoon, int webSiteId)
		{
			if(cartoon.Name == NewElementString)
			{
				AddCartoonVisibility = Visibility.Visible;
				SaveChangesVisibility = Visibility.Hidden;

				return;
			}

			WebSiteId = webSiteId;
			CartoonId = cartoon.CartoonId;
			LoadData();
			AddCartoonVisibility = Visibility.Hidden;
			SaveChangesVisibility = Visibility.Visible;
			NotifySeasonList();
		}

		public CartoonsEditingViewModel()
		{

		}











	}
}
