namespace CartoonViewer.Settings.ViewModels
{
	using System;
	using System.Data.Entity;
	using System.Linq;
	using System.Windows.Controls;
	using System.Windows.Input;
	using Caliburn.Micro;
	using CartoonViewer.ViewModels;
	using Database;
	using Dialogs.ViewModels;
	using Models.CartoonModels;
	using static Helpers.Helper;

	public partial class CartoonsControlViewModel : Conductor<Screen>.Collection.OneActive
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
		/// Снять выделение с выбранного мультфильма
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
