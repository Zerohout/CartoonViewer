namespace CartoonViewer.Settings.CartoonEditorFolder.ViewModels
{
	using System.Linq;
	using System.Windows;
	using Helpers;
	using Models.CartoonModels;
	using Newtonsoft.Json;
	using Screen = Caliburn.Micro.Screen;
	using static Helpers.Helper;

	public partial class CartoonsEditingViewModel : Screen, ISettingsViewModel
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="cartoon"></param>
		public CartoonsEditingViewModel(Cartoon cartoon)
		{
			if(cartoon.Name == SettingsHelper.NewElementString)
			{
				cartoon.CartoonUrls.Add(new CartoonUrl
				{
					CartoonWebSiteId = SettingsHelper.GlobalIdList.WebSiteId
				});


				SelectedCartoon = CloneObject<Cartoon>(cartoon);
				SelectedCartoonUrl = SelectedCartoon.CartoonUrls.First();

				TempCartoonSnapshot = JsonConvert.SerializeObject(SelectedCartoon);

				NotifyOfPropertyChange(() => CreateNewCartoonVisibility);
				NotifyOfPropertyChange(() => SaveChangesVisibility);
				return;
			}

			LoadData();
			NotifyOfPropertyChange(() => CreateNewCartoonVisibility);
			NotifyOfPropertyChange(() => SaveChangesVisibility);
			NotifySeasonList();
		}



		public CartoonsEditingViewModel()
		{

		}


		protected override void OnInitialize()
		{
			DisplayName = "Редактор мультсериалов";
			base.OnInitialize();
		}
	}
}
