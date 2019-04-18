namespace CartoonViewer.Settings.ViewModels
{
	using System;
	using System.Data.Entity;
	using System.Linq;
	using System.Windows.Input;
	using Caliburn.Micro;
	using CartoonViewer.ViewModels;
	using Database;
	using Dialogs.ViewModels;
	using Models.CartoonModels;
	using static Helpers.Helper;

	public partial class CartoonsControlViewModel : Conductor<Screen>.Collection.OneActive
	{
		#region ComboBoxes

		public void CBoxMouseDown(EventArgs ea)
		{
			WinMan.ShowDialog(new DialogViewModel(null,DialogState.SAVE_CHANGES));
			//((MouseButtonEventArgs) ea).Handled = !ExitWithoutSave() ?? false;
		}

		/// <summary>
		/// Изменен выбранный адрес сайта
		/// </summary>
		public void WebSitesSelectionChanged()
		{
			if(SelectedWebSite == null)
				return;

			GlobalIdList.WebSiteId = SelectedWebSite.CartoonWebSiteId;

			LoadList();
		}

		

		/// <summary>
		/// Изменен выбранный мультфильм
		/// </summary>
		public async void CartoonsSelectionChanged()
		{
			if(SelectedCartoon == null)
				return;

			GlobalIdList.CartoonId = SelectedCartoon.CartoonId;

			LoadList();

			Seasons.Clear();
			NotifyOfPropertyChange(() => CanCancelCartoonSelection);

			if(SelectedCartoon == null)
			{
				ChangeActiveItem(null);
				return;
			}

			GlobalIdList.CartoonId = SelectedCartoon.CartoonId;

			if(SelectedCartoon.Name == NewElementString)
			{


				ChangeActiveItem(new CartoonsEditingViewModel(SelectedCartoon, SelectedWebSite.CartoonWebSiteId));
				return;
			}

			using(var ctx = new CVDbContext())
			{
				await ctx.CartoonSeasons
						 .LoadAsync();

				Seasons.AddRange(ctx.CartoonSeasons.Local.Where(s => s.CartoonId == _selectedCartoon.CartoonId));
			}

			ChangeActiveItem(new CartoonsEditingViewModel(SelectedCartoon, SelectedWebSite.CartoonWebSiteId));


		}

		/// <summary>
		/// Изменен выбранный сезон
		/// </summary>
		public void SeasonsSelectionChanged()
		{
			NotifyOfPropertyChange(() => CanCancelSeasonSelection);

			if(SelectedSeason == null)
			{
				ChangeActiveItem(new CartoonsEditingViewModel(SelectedCartoon, SelectedWebSite.CartoonWebSiteId));
				return;
			}

			GlobalIdList.SeasonId = SelectedSeason.CartoonSeasonId;

			ChangeActiveItem(new SeasonsEditingViewModel(SelectedSeason));
		}

		#endregion


		#region Buttons

		/// <summary>
		/// Снять выделение с выбранного сайта
		/// </summary>
		public void CancelWebSiteSelection()
		{
			if(SelectedCartoon != null)
			{
				CancelCartoonSelection();
				// Проверка, было ли отменено снятие выбора мультфильма
				if(SelectedCartoon != null)
					return;
			}

			Cartoons = new BindableCollection<Cartoon>();
			SelectedWebSite = null;
			NotifyOfPropertyChange(() => CanCancelWebSiteSelection);
		}

		public bool CanCancelWebSiteSelection => SelectedWebSite != null;

		/// <summary>
		/// Снять выделение с выбранного мультфильма
		/// </summary>
		public void CancelCartoonSelection()
		{
			if(SelectedSeason != null)
			{
				CancelSeasonSelection();
				// Проверка, было ли отменено снятие выбора сезона
				if(SelectedSeason != null)
					return;
			}

			var result = ChangeActiveItem(null);
			if(!result)
				return;

			Seasons = new BindableCollection<CartoonSeason>();
			SelectedCartoon = null;
			NotifyOfPropertyChange(() => CartoonsVisibility);
			NotifyOfPropertyChange(() => CanCancelCartoonSelection);
		}

		public bool CanCancelCartoonSelection => SelectedCartoon != null;

		/// <summary>
		/// Снять выделение с выбранного сезона
		/// </summary>
		public void CancelSeasonSelection()
		{
			var result = ChangeActiveItem(
				new CartoonsEditingViewModel(SelectedCartoon, GlobalIdList.WebSiteId));
			if(!result)
				return;

			SelectedSeason = null;
			NotifyOfPropertyChange(() => CanCancelSeasonSelection);
		}

		public bool CanCancelSeasonSelection => SelectedSeason != null;

		#endregion
	}
}
