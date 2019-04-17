namespace CartoonViewer.Settings.ViewModels
{
	using System.Data.Entity;
	using System.Linq;
	using Caliburn.Micro;
	using Database;
	using Models.CartoonModels;

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

		public void LoadData()
		{
			Cartoon result;
			using(var ctx = new CVDbContext())
			{
				ctx.Cartoons
				   .Where(c => c.CartoonId == CartoonId)
				   .Include(c => c.CartoonUrls)
				   .Include(c => c.CartoonSeasons)
				   .Include(c => c.CartoonVoiceOvers)
				   .Load();

				result = ctx.Cartoons.Local.First();
				VoiceOvers.Clear();
				VoiceOvers.AddRange(result.CartoonVoiceOvers);
			}

			Seasons.Clear();
			Seasons.AddRange(result.CartoonSeasons);
			Url = result.CartoonUrls.Find(cu => cu.CartoonWebSiteId == WebSiteId).Url;
			Name = result.Name;
			Description = result.Description;
			TempUrl = Url;
			TempName = Name;
			TempDescription = Description;
		}

		#endregion
	}
}
