namespace CartoonViewer.Settings.ViewModels
{
	using System.Collections.Generic;
	using System.Data.Entity;
	using System.Linq;
	using Caliburn.Micro;
	using Database;
	using Models.CartoonModels;
	using static Helpers.Helper;

	public partial class CartoonsEditingViewModel : Screen, ISettingsViewModel
	{
		#region EventsActions

		public void EditVoiceOvers()
		{
			WinMan.ShowWindow(
				new VoiceOversEditingViewModel(
					websiteId: GlobalIdList.WebSiteId,
					cartoonId: GlobalIdList.CartoonId));
		}

		/// <summary>
		/// Добавить Сезон в список
		/// </summary>
		public void AddSeason()
		{
			var count = Seasons.Count + 1;

			Seasons.Add(new CartoonSeason
			{
				CartoonId = CartoonId,
				Number = count,
				Checked = true,
				CartoonEpisodes = new List<CartoonEpisode>()
			});

			using(var ctx = new CVDbContext())
			{
				ctx.CartoonSeasons.Add(Seasons.Last());
				ctx.SaveChanges();
				Seasons.Last().CartoonSeasonId = ctx.CartoonSeasons.ToList().Last().CartoonSeasonId;
			}

			SelectedCartoonSeason = Seasons.Count > 0
				? Seasons.Last()
				: null;

			NotifySeasonList();
		}

		/// <summary>
		/// Редактировать выбранный сезон
		/// </summary>
		public void EditSeason()
		{
			((CartoonsControlViewModel)Parent).ChangeSelectedSeason(SelectedCartoonSeason.CartoonSeasonId);
		}

		public bool CanEditSeason => SelectedCartoonSeason != null;

		/// <summary>
		/// Удалить выбранный сезон
		/// </summary>
		public void RemoveSeason()
		{
			using(var ctx = new CVDbContext())
			{
				var temp = ctx.CartoonSeasons.Find(SelectedCartoonSeason.CartoonSeasonId);
				ctx.Entry(temp).State = EntityState.Deleted;
				ctx.SaveChanges();
			}

			Seasons.Remove(SelectedCartoonSeason);

			SelectedCartoonSeason = Seasons.Count > 0
				? Seasons.Last()
				: null;

			NotifySeasonList();
		}

		public bool CanRemoveSeason => SelectedCartoonSeason != null;

		/// <summary>
		/// Отменить выделение сезона
		/// </summary>
		public void CancelSelection()
		{
			SelectedCartoonSeason = null;
			NotifySeasonList();
		}

		public bool CanCancelSelection => SelectedCartoonSeason != null;

		/// <summary>
		/// Сохранить изменения
		/// </summary>
		public async void SaveChanges()
		{
			using(var ctx = new CVDbContext())
			{
				var temp = ctx.Cartoons.Find(CartoonId);
				if(temp != null)
				{
					temp.CartoonUrls.Find(cu => cu.CartoonWebSiteId == WebSiteId).Url = Url;
					temp.Name = Name;
					temp.Description = Description;
					ctx.Entry(temp).State = EntityState.Modified;
				}

				await ctx.SaveChangesAsync();
			}

			TempUrl = Url;
			TempName = Name;
			TempDescription = Description;

			NotifyOfPropertyChange(() => CanSaveChanges);
			NotifyOfPropertyChange(() => HasChanges);
		}

		public bool CanSaveChanges
		{
			get
			{
				if((string.IsNullOrWhiteSpace(Url) || string.IsNullOrWhiteSpace(Name)) ||
					(TempUrl == Url && TempName == Name && TempDescription == Description))
				{
					return false;
				}

				return true;
			}
		}

		/// <summary>
		/// Действие при изменении текста
		/// </summary>
		public void TextChanged()
		{
			NotifyOfPropertyChange(() => CanSaveChanges);
			NotifyOfPropertyChange(() => HasChanges);
		}

		/// <summary>
		/// Действие при изменении выбора сезона
		/// </summary>
		public void SelectionChanged()
		{
			NotifySeasonList();
		}

		#endregion
	}
}
