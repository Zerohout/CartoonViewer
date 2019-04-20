namespace CartoonViewer.Settings.ViewModels
{
	using System.Collections.Generic;
	using System.Data.Entity;
	using System.Linq;
	using System.Windows;
	using System.Windows.Controls;
	using Caliburn.Micro;
	using CartoonViewer.ViewModels;
	using Database;
	using Models.CartoonModels;
	using static Helpers.Helper;
	using static Helpers.Cloner;

	public partial class CartoonsEditingViewModel : Screen, ISettingsViewModel
	{
		#region EventsActions

		public void SelectSeason() { SelectedSeason = Seasons.First(); }

		public bool CanSelectSeason
		{
			get
			{
				if (SelectedSeason == null ||
				    Seasons.Count == 0)
				{
					return false;
				}

				return true;
			}
		}

		public void EditVoiceOvers()
		{
			WinMan.ShowDialog(
				new VoiceOversEditingViewModel(
					websiteId: GlobalIdList.WebSiteId,
					cartoonId: GlobalIdList.CartoonId));
		}

		/// <summary>
		/// Добавить Сезон в список
		/// </summary>
		public async void AddSeason()
		{
			var count = Seasons.Count + 1;

			var newSeason = new CartoonSeason
			{
				CartoonId = GlobalIdList.CartoonId,
				Number = count,
				Checked = true,
				CartoonEpisodes = new List<CartoonEpisode>()
			};

			Seasons.Add(newSeason);
			NotifyOfPropertyChange(() => Seasons);
			((CartoonsControlViewModel)Parent).Seasons.Add(newSeason);
			NotifyOfPropertyChange(() => ((CartoonsControlViewModel)Parent).Seasons);

			using(var ctx = new CVDbContext())
			{
				ctx.CartoonSeasons.Add(Seasons.Last());
				await ctx.SaveChangesAsync();
				Seasons.Last().CartoonSeasonId = ctx.CartoonSeasons.ToList().Last().CartoonSeasonId;
			}

			SelectedSeason = Seasons.Count > 0
				? Seasons.Last()
				: null;
		}

		/// <summary>
		/// Редактировать выбранный сезон
		/// </summary>
		public void EditSeason()
		{
			var parent = ((CartoonsControlViewModel) Parent);

			parent.SelectedSeason = parent.Seasons
				.First(s => s.CartoonSeasonId == SelectedSeason.CartoonSeasonId);
		}

		public bool CanEditSeason => SelectedSeason != null;

		/// <summary>
		/// Удалить выбранный сезон
		/// </summary>
		public void RemoveSeason()
		{
			using(var ctx = new CVDbContext())
			{
				var temp = ctx.CartoonSeasons.Find(SelectedSeason.CartoonSeasonId);
				ctx.Entry(temp).State = EntityState.Deleted;
				ctx.SaveChanges();
			}
			
			var tempList = ((CartoonsControlViewModel) Parent).Seasons;
			((CartoonsControlViewModel) Parent)
				.Seasons.Remove(tempList.First(s => s.CartoonSeasonId == SelectedSeason.CartoonSeasonId));
			Seasons.Remove(SelectedSeason);
			NotifyOfPropertyChange(() => Seasons);
			SelectedSeason = Seasons.Count > 0
				? Seasons.Last()
				: null;
		}

		public bool CanRemoveSeason => SelectedSeason != null;

		/// <summary>
		/// Отменить выделение сезона
		/// </summary>
		public void CancelSelection()
		{
			SelectedSeason = null;
		}

		public bool CanCancelSelection => SelectedSeason != null;

		/// <summary>
		/// Сохранить изменения
		/// </summary>
		public async void SaveChanges()
		{
			using(var ctx = new CVDbContext())
			{
				var cartoon = ctx.Cartoons
				                 .Include(c => c.CartoonUrls)
				                 .Single(c => c.CartoonId == GlobalIdList.CartoonId);

				cartoon.Name = SelectedCartoon.Name;
				cartoon.Description = SelectedCartoon.Description;
				cartoon.CartoonUrls
				       .Single(cu => cu.CartoonUrlId == SelectedCartoonUrl.CartoonUrlId).Url = SelectedCartoonUrl.Url;
				
				ctx.Entry(cartoon).State = EntityState.Modified;
				

				await ctx.SaveChangesAsync();
			}

			TempCartoon = CloneCartoon(SelectedCartoon);
			TempCartoonUrl = CloneCartoonUrl(SelectedCartoonUrl);
			
			NotifyOfPropertyChange(() => CanSaveChanges);
			NotifyOfPropertyChange(() => HasChanges);
			var parent = (CartoonsControlViewModel)Parent;

			parent.SelectedCartoon.Name = SelectedCartoon.Name;
			parent.NotifyOfPropertyChange(() => parent.SelectedCartoon);
			parent.NotifyOfPropertyChange(() => parent.Cartoons);

		}

		public bool CanSaveChanges
		{
			get
			{
				if (SelectedCartoon == null ||
				    SelectedCartoonUrl == null)
				{
					return false;
				}
				
				if (string.IsNullOrEmpty(SelectedCartoonUrl.Url) ||
				    string.IsNullOrEmpty(SelectedCartoon.Name))
				{
					return false;
				}

				if(((CartoonsControlViewModel)Parent).Cartoons.Any(c => c.Name == SelectedCartoon.Name))
				{
					return false;
				}

				if (SelectedCartoonUrl.Url == TempCartoonUrl.Url &&
				    SelectedCartoon.Name == TempCartoon.Name &&
				    SelectedCartoon.Description == TempCartoon.Description)
				{
					return false;
				}

				return true;
			}
		}

		public void CreateNewCartoon()
		{
			SelectedCartoon.CartoonUrls.Add(SelectedCartoonUrl);
			var parent = ((CartoonsControlViewModel)Parent);
			using (var ctx = new CVDbContext())
			{
				ctx.Cartoons.Add(SelectedCartoon);
				ctx.SaveChanges();

				var newCartoon = ctx.Cartoons.Single(c => c.Name == SelectedCartoon.Name);
				ctx.CartoonWebSites.Find(GlobalIdList.WebSiteId).Cartoons.Add(newCartoon);
				ctx.SaveChanges();
				
				GlobalIdList.CartoonId = newCartoon.CartoonId;
				parent.Cartoons.Add(newCartoon);
				parent.NotifyOfPropertyChange(() => parent.Cartoons);
				parent.Cartoons.Remove(parent.Cartoons.First(c => c.Name == NewElementString));
				parent.Cartoons.Add(new Cartoon { Name = NewElementString });
				parent.SelectedCartoon = newCartoon;
			}
		}

		public bool CanCreateNewCartoon
		{
			get
			{
				if (SelectedCartoon == null ||
				    SelectedCartoonUrl == null)
				{
					return false;
				}

				if (string.IsNullOrEmpty(SelectedCartoon.Name) ||
				    string.IsNullOrEmpty(SelectedCartoonUrl.Url) ||
				    SelectedCartoon.Name == NewElementString)
				{
					return false;
				}

				if (((CartoonsControlViewModel)Parent).Cartoons.Any(c => c.Name == SelectedCartoon.Name))
				{
					return false;
				}

				return true;
			}
		}

		public void RemoveCartoon()
		{

			var dvm = new DialogViewModel("мультфильм", DialogState.REMOVE);
			WinMan.ShowDialog(dvm);

			switch (dvm.DialogResult)
			{
				case DialogResult.YES_ACTION:
					break;
				case DialogResult.NO_ACTION:
					return;
				default:
					return;
			}


			var parent = ((CartoonsControlViewModel) Parent);
			using (var ctx = new CVDbContext())
			{
				var cartoon = ctx.Cartoons.Find(GlobalIdList.CartoonId);

				if(cartoon != null) ctx.Cartoons.Remove(cartoon);

				ctx.SaveChanges();
			}

			parent.Cartoons.Remove(parent.SelectedCartoon);
		}

		/// <summary>
		/// Действие при изменении текста
		/// </summary>
		public void TextChanged()
		{
			NotifyOfPropertyChange(() => CanSaveChanges);
			NotifyOfPropertyChange(() => CanCreateNewCartoon);
			NotifyOfPropertyChange(() => HasChanges);
		}

		/// <summary>
		/// Действие при изменении выбора сезона
		/// </summary>
		public void SelectionChanged(ListBox lb)
		{
			lb.ScrollIntoView(lb.SelectedItem);
		}

		#endregion
	}
}
