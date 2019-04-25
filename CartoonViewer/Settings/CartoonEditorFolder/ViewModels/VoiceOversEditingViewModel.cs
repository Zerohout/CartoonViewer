namespace CartoonViewer.Settings.CartoonEditorFolder.ViewModels
{
	using Caliburn.Micro;

	public partial class VoiceOversEditingViewModel : Screen, ISettingsViewModel
	{

		/// <summary>
		/// Конструктор при выборе озвучек мультсериала (необходим выбор сезона и эпизода)
		/// </summary>
		/// <param name="websiteId">ID выбранного сайта</param>
		/// <param name="cartoonId">ID необходимого мультсериала</param>
		public VoiceOversEditingViewModel(int websiteId, int cartoonId)
		{
			IdList.WebSiteId = websiteId;
			IdList.CartoonId = cartoonId;
			LoadData();
		}

		/// <summary>
		/// Конструктор при выборе озвучек эпизода (загрузка минуя выбор мультсериала, сезона и эпизода)
		/// </summary>
		/// <param name="idList"></param>
		public VoiceOversEditingViewModel((int WebSiteId, int CartoonId, int SeasonId, int EpisodeId) idList)
		{
			IdList = idList;
			LoadData();
		}

		protected override void OnInitialize()
		{
			LoadGlobalVoiceOverList();

			base.OnInitialize();
		}



		public VoiceOversEditingViewModel()
		{

		}
	}
}
