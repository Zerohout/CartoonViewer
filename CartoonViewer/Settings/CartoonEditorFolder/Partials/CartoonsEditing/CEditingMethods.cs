// ReSharper disable CheckNamespace
namespace CartoonViewer.Settings.CartoonEditorFolder.ViewModels
{
	using System.Data.Entity;
	using System.Linq;
	using Caliburn.Micro;
	using Database;
	using Helpers;
	using Models.CartoonModels;
	using Newtonsoft.Json;
	using static Helpers.Helper;

	public partial class CartoonsEditingViewModel : Screen, ISettingsViewModel
	{
		#region Private methods

		private  void LoadData()
		{
			Cartoon result;
			using(var ctx = new CVDbContext(SettingsHelper.AppDataPath))
			{
				result = ctx.Cartoons
								  .Include(c => c.CartoonUrls)
								  .Include(c => c.CartoonSeasons)
								  .Include(c => c.CartoonVoiceOvers)
								  .Single(c => c.CartoonId == SettingsHelper.GlobalIdList.CartoonId);
			}

			SelectedCartoon = CloneObject<Cartoon>(result);
			SelectedCartoonUrl = SelectedCartoon.CartoonUrls.First();
			TempCartoonSnapshot = JsonConvert.SerializeObject(SelectedCartoon);

			Seasons = new BindableCollection<CartoonSeason>(result.CartoonSeasons);
			VoiceOvers = new BindableCollection<CartoonVoiceOver>(result.CartoonVoiceOvers);
			SelectedSeason = Seasons.FirstOrDefault();
		}

		public void UpdateVoiceOverList()
		{
			if(SettingsHelper.GlobalIdList.CartoonId == 0)
				return;

			Cartoon cartoon;
			using(var ctx = new CVDbContext(SettingsHelper.AppDataPath))
			{
				cartoon = ctx.Cartoons
								   .Include(c => c.CartoonVoiceOvers)
								   .Single(c => c.CartoonId == SettingsHelper.GlobalIdList.CartoonId);
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
