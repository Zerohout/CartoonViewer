namespace CartoonViewer.Settings.ViewModels
{
	using Caliburn.Micro;

	public partial class VoiceOversEditingViewModel : Screen
	{

		/// <summary>
		/// Конструктор при выборе озвучек мультфильма (необходим выбор сезона и эпизода)
		/// </summary>
		/// <param name="websiteId">ID выбранного сайта</param>
		/// <param name="cartoonId">ID необходимого мультфильма</param>
		public VoiceOversEditingViewModel(int websiteId, int cartoonId)
		{
			IdList.WebSiteId = websiteId;
			IdList.CartoonId = cartoonId;
			LoadData();
		}

		/// <summary>
		/// Конструктор при выборе озвучек эпизода (загрузка минуя выбор мультфильма, сезона и эпизода)
		/// </summary>
		/// <param name="idList"></param>
		public VoiceOversEditingViewModel((int WebSiteId, int CartoonId, int SeasonId, int EpisodeId) idList)
		{
			IdList = idList;
			LoadData();
		}

		public VoiceOversEditingViewModel()
		{

		}
	}
}
