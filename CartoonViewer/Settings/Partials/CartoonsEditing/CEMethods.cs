namespace CartoonViewer.Settings.ViewModels
{
	using System.Data.Entity;
	using System.Linq;
	using System.Threading.Tasks;
	using Caliburn.Micro;
	using Database;
	using Models.CartoonModels;
	using static Helpers.Helper;
	using static Helpers.Cloner;

	public partial class CartoonsEditingViewModel : Screen, ISettingsViewModel
	{
		#region Private methods

		private void NotifySeasonList()
		{
			NotifyOfPropertyChange(() => Seasons);
			NotifyOfPropertyChange(() => CanEditSeason);
			NotifyOfPropertyChange(() => CanRemoveSeason);
			NotifyOfPropertyChange(() => CanCancelSelection);
		}

		#endregion
		
		#region Public methods

		private async void LoadData()
		{
			Cartoon result;
			using(var ctx = new CVDbContext())
			{
				result = await ctx.Cartoons
				   .Include(c => c.CartoonUrls)
				   .Include(c => c.CartoonSeasons)
				   .Include(c => c.CartoonVoiceOvers)
				   .SingleAsync(c => c.CartoonId == GlobalIdList.CartoonId);
			}

			SelectedCartoon = CloneCartoon(result);
			TempCartoon = CloneCartoon(SelectedCartoon);
			SelectedCartoonUrl = CloneCartoonUrl(result.CartoonUrls
			                                           .Find(cu => cu.CartoonWebSiteId == GlobalIdList.WebSiteId));
			TempCartoonUrl = CloneCartoonUrl(SelectedCartoonUrl);
			Seasons = new BindableCollection<CartoonSeason>(result.CartoonSeasons);
			VoiceOvers = new BindableCollection<CartoonVoiceOver>(result.CartoonVoiceOvers);


			//Seasons.Clear();
			//Seasons.AddRange(result.CartoonSeasons);
			//Url = result.CartoonUrls.Find(cu => cu.CartoonWebSiteId == WebSiteId).Url;
			//Name = result.Name;
			//Description = result.Description;
			//TempUrl = Url;
			//TempName = Name;
			//TempDescription = Description;
		}

		#endregion
	}
}
