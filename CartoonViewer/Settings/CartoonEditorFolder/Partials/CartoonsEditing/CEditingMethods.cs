// ReSharper disable CheckNamespace
namespace CartoonViewer.Settings.CartoonEditorFolder.ViewModels
{
	using System.Data.Entity;
	using System.Linq;
	using Caliburn.Micro;
	using Database;
	using Helpers;
	using Models.CartoonModels;
	using static Helpers.Cloner;
	using static Helpers.Helper;

	public partial class CartoonsEditingViewModel : Screen, ISettingsViewModel
	{
		#region Private methods

		private async void LoadData()
		{
			Cartoon result;
			using(var ctx = new CVDbContext(SettingsHelper.AppDataPath))
			{
				result = await ctx.Cartoons
								  .Include(c => c.CartoonUrls)
								  .Include(c => c.CartoonSeasons)
								  .Include(c => c.CartoonVoiceOvers)
								  .SingleAsync(c => c.CartoonId == SettingsHelper.GlobalIdList.CartoonId);
			}

			SelectedCartoon = CloneCartoon(result);
			TempCartoon = CloneCartoon(SelectedCartoon);
			SelectedCartoonUrl = CloneCartoonUrl(result.CartoonUrls
													   .Find(cu => cu.CartoonWebSiteId == SettingsHelper.GlobalIdList.WebSiteId));
			TempCartoonUrl = CloneCartoonUrl(SelectedCartoonUrl);
			Seasons = new BindableCollection<CartoonSeason>(result.CartoonSeasons);
			VoiceOvers = new BindableCollection<CartoonVoiceOver>(result.CartoonVoiceOvers);
			if(Seasons.Count > 0)
			{
				SelectedSeason = Seasons.First();
			}
		}

		public async void UpdateVoiceOverList()
		{
			if (SettingsHelper.GlobalIdList.CartoonId == 0) return;

			Cartoon cartoon;
			using(var ctx = new CVDbContext(SettingsHelper.AppDataPath))
			{
				cartoon = await ctx.Cartoons
				                   .Include(c => c.CartoonVoiceOvers)
				                   .SingleAsync(c => c.CartoonId == SettingsHelper.GlobalIdList.CartoonId);
			}

			VoiceOvers = new BindableCollection<CartoonVoiceOver>(cartoon.CartoonVoiceOvers);
		}

		private void NotifySeasonList()
		{
			NotifyOfPropertyChange(() => CanEditSeason);
			NotifyOfPropertyChange(() => CanRemoveSeason);
			NotifyOfPropertyChange(() => CanCancelSelection);
			NotifyOfPropertyChange(() => CanSelectSeason);
		}


		#endregion


	}
}
