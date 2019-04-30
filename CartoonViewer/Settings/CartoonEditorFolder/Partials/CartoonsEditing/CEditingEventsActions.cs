// ReSharper disable CheckNamespace
namespace CartoonViewer.Settings.CartoonEditorFolder.ViewModels
{
	using System.Collections.Generic;
	using System.Data.Entity;
	using System.Linq;
	using System.Windows.Controls;
	using System.Windows.Input;
	using Caliburn.Micro;
	using CartoonViewer.ViewModels;
	using Database;
	using Helpers;
	using Models.CartoonModels;
	using Newtonsoft.Json;
	using static Helpers.Helper;
	using static Helpers.SettingsHelper;

	public partial class CartoonsEditingViewModel : Screen, ISettingsViewModel
	{
		#region EventsActions

		public void KeyDown(KeyEventArgs e)
		{
			switch(e.KeyboardDevice.Modifiers)
			{
				case ModifierKeys.Control:
					switch(e.Key)
					{
						case Key.S:
							if(CanSaveChanges)
							{
								SaveChanges();
								return;
							}
							break;
					}

					break;
				case ModifierKeys.None:
					switch(e.Key)
					{
						case Key.Escape:
							//	if(IsNotEditing is false)
							//	{
							//		if(CanSaveChanges)
							//		{
							//			CancelChanges();
							//			return;
							//		}
							//		if(CanCancelEditing)
							//		{
							//			CancelEditing();
							//			return;
							//		}

							//		return;
							//	}

							((CartoonsEditorViewModel)Parent).CancelSeasonSelection();
							break;
					}
					break;
			}
		}

		public void TBoxDoubleClick(TextBox source)
		{
			source.SelectAll();
		}

		/// <summary>
		/// Выбор первого в списке сезона
		/// (для обхода бага с неактивным списком при наведении курсора)
		/// P.S. иного решения найти пока что не удалось.
		/// </summary>
		public void SelectSeason()
		{
			if(CanSelectSeason is false) return;

			SelectedSeason = Seasons.First();
		}

		public bool CanSelectSeason
		{
			get
			{
				if(SelectedSeason != null ||
					Seasons.Count == 0)
				{
					return false;
				}

				return true;
			}
		}
		/// <summary>
		/// Открыть редактор озвучек
		/// </summary>
		public void EditVoiceOvers()
		{
			var wm = new WindowsManagerViewModel(new VoiceOversEditingViewModel(
													 websiteId: GlobalIdList.WebSiteId,
													 cartoonId: GlobalIdList.CartoonId));

			WinMan.ShowDialog(wm);

			UpdateVoiceOverList();
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
			((CartoonsEditorViewModel)Parent).Seasons.Add(newSeason);
			NotifyOfPropertyChange(() => ((CartoonsEditorViewModel)Parent).Seasons);

			using(var ctx = new CVDbContext(AppDataPath))
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
			if(CanEditSeason is false) return;

			var parent = ((CartoonsEditorViewModel)Parent);

			parent.SelectedSeason = parent.Seasons
				.First(s => s.CartoonSeasonId == SelectedSeason.CartoonSeasonId);
		}

		public bool CanEditSeason => SelectedSeason != null;

		/// <summary>
		/// Удалить выбранный сезон
		/// </summary>
		public void RemoveSeason()
		{
			if(CanRemoveSeason is false) return;

			using(var ctx = new CVDbContext(AppDataPath))
			{
				var temp = ctx.CartoonSeasons.Find(SelectedSeason.CartoonSeasonId);
				ctx.Entry(temp).State = EntityState.Deleted;
				ctx.SaveChanges();
			}

			var tempList = ((CartoonsEditorViewModel)Parent).Seasons;
			((CartoonsEditorViewModel)Parent)
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
			if(CanCancelSelection is false) return;
			SelectedSeason = null;
		}

		public bool CanCancelSelection => SelectedSeason != null;

		/// <summary>
		/// Сохранить изменения
		/// </summary>
		public async void SaveChanges()
		{
			if(CanSaveChanges is false) return;
			using(var ctx = new CVDbContext(AppDataPath))
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

			TempCartoonSnapshot = JsonConvert.SerializeObject(SelectedCartoon);

			NotifyOfPropertyChange(() => CanSaveChanges);
			NotifyOfPropertyChange(() => HasChanges);
			var parent = (CartoonsEditorViewModel)Parent;

			if(parent == null)
				return;

			parent.SelectedCartoon.Name = SelectedCartoon.Name;
			parent.NotifyOfPropertyChange(() => parent.SelectedCartoon);
			parent.NotifyOfPropertyChange(() => parent.Cartoons);
		}

		public bool CanSaveChanges => HasChanges;

		/// <summary>
		/// Создать новый м/с
		/// </summary>
		public void CreateNewCartoon()
		{
			if(CanCreateNewCartoon is false) return;
			using(var ctx = new CVDbContext(AppDataPath))
			{
				var newCartoon = new Cartoon
				{
					Name = SelectedCartoon.Name,
					Checked = true,
					Description = SelectedCartoon.Description
				};

				ctx.Cartoons.Add(newCartoon);
				ctx.SaveChanges();
				ctx.CartoonWebSites
				   .First(cws => cws.CartoonWebSiteId == GlobalIdList.WebSiteId)
				   .Cartoons.Add(ctx.Cartoons.ToList().Last());
				ctx.SaveChanges();

				newCartoon = ctx.Cartoons.ToList().Last();
				GlobalIdList.CartoonId = newCartoon.CartoonId;

				CartoonVoiceOver voiceOver;
				if(VoiceOvers.Any() is false)
				{
					ctx.VoiceOvers.Add(new CartoonVoiceOver
					{
						Name = $"{SelectedCartoon.Name}_VO"
					});
					ctx.SaveChanges();
					voiceOver = ctx.VoiceOvers.ToList().Last();
				}
				else
				{
					voiceOver = ctx.VoiceOvers
								   .First(vo => vo.Cartoons
												  .Any(c => c.CartoonId == newCartoon.CartoonId));
				}

				voiceOver.Cartoons.Add(newCartoon);
				newCartoon.CartoonVoiceOvers.Add(voiceOver);

				var parent = ((CartoonsEditorViewModel)Parent);

				var cartoonUrl = new CartoonUrl
				{
					CartoonWebSiteId = parent.SelectedWebSite.CartoonWebSiteId,
					WebSiteUrl = parent.SelectedWebSite.Url,
					CartoonId = newCartoon.CartoonId,
					Checked = true,
					Url = SelectedCartoonUrl.Url
				};
				
				newCartoon.CartoonUrls.Add(cartoonUrl);
				ctx.SaveChanges();

				TempCartoonSnapshot = JsonConvert.SerializeObject(SelectedCartoon);
				NotifyOfPropertyChange(() => CanSaveChanges);

				parent.Cartoons.Add(newCartoon);
				parent.NotifyOfPropertyChange(() => parent.Cartoons);
				parent.Cartoons.Remove(parent.Cartoons.First(c => c.Name == NewElementString));
				parent.Cartoons.Add(new Cartoon { Name = NewElementString });
				parent.SelectedCartoon = parent.Cartoons.First(c => c.CartoonId == GlobalIdList.CartoonId);
			}
		}

		public bool CanCreateNewCartoon
		{
			get
			{
				if(SelectedCartoon == null ||
					SelectedCartoonUrl == null)
				{
					return false;
				}

				if(string.IsNullOrEmpty(SelectedCartoon.Name) ||
					string.IsNullOrEmpty(SelectedCartoonUrl.Url) ||
					SelectedCartoon.Name == NewElementString)
				{
					return false;
				}

				if(((CartoonsEditorViewModel)Parent).Cartoons.Any(c => c.Name == SelectedCartoon.Name))
				{
					return false;
				}

				return true;
			}
		}
		/// <summary>
		/// Удалить выбраный м/с
		/// </summary>
		public void RemoveCartoon()
		{
			var dvm = new DialogViewModel(null, DialogType.REMOVE_OBJECT);
			WinMan.ShowDialog(dvm);

			switch(dvm.DialogResult)
			{
				case DialogResult.YES_ACTION:
					break;
				case DialogResult.NO_ACTION:
					return;
				default:
					return;
			}

			using(var ctx = new CVDbContext(AppDataPath))
			{
				var cartoon = ctx.Cartoons.Find(GlobalIdList.CartoonId);

				if(cartoon != null)
					ctx.Cartoons.Remove(cartoon);

				var voiceOverForRemove = ctx.VoiceOvers
											.Where(vo => vo.Cartoons.Count == 1)
											.Where(vo => vo.Cartoons
														   .Any(c => c.CartoonId == GlobalIdList.CartoonId));
				ctx.VoiceOvers.RemoveRange(voiceOverForRemove);
				
				ctx.SaveChanges();
			}

			var parent = ((CartoonsEditorViewModel)Parent);
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
