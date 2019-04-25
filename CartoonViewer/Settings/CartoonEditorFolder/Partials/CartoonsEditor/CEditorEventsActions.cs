// ReSharper disable CheckNamespace
namespace CartoonViewer.Settings.CartoonEditorFolder.ViewModels
{
	using Caliburn.Micro;

	public partial class CartoonsEditorViewModel : Conductor<Screen>.Collection.OneActive
	{
		#region Buttons

		/// <summary>
		/// Снять выделение с выбранного сайта
		/// </summary>
		public void CancelWebSiteSelection()
		{
			SelectedWebSite = null;
		}

		public bool CanCancelWebSiteSelection => SelectedWebSite != null;

		/// <summary>
		/// Снять выделение с выбранного мультсериала
		/// </summary>
		public void CancelCartoonSelection()
		{
			SelectedCartoon = null;
		}

		public bool CanCancelCartoonSelection => SelectedCartoon != null;

		/// <summary>
		/// Снять выделение с выбранного сезона
		/// </summary>
		public void CancelSeasonSelection()
		{
			SelectedSeason = null;
		}

		public bool CanCancelSeasonSelection => SelectedSeason != null;

		#endregion
	}
}
