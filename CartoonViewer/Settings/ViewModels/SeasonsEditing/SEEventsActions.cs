namespace CartoonViewer.Settings.ViewModels
{
	using System;
	using System.Data.Entity;
	using System.Windows;
	using Caliburn.Micro;
	using Database;
	using Models.CartoonModels;
	using static Helpers.Cloner;

	public partial class SeasonsEditingViewModel : Screen, ISettingsViewModel
	{
		#region EventsActions

		public async void SaveChanges()
		{
			if(SeasonHaveChanges)
			{
				using(var ctx = new CVDbContext())
				{
					ctx.Entry(_cartoonSeason).State = EntityState.Modified;
					await ctx.SaveChangesAsync();
				}

				_tempCartoonSeason = CloneSeason(_cartoonSeason);
			}
		}

		public bool CanSaveChanges
		{
			get
			{
				if(EpisodeEditingVisibility == Visibility.Hidden)
				{
					if(_cartoonSeason.Name == null &&
					   _cartoonSeason.Description == null)
					{
						SeasonHaveChanges = false;
						return false;
					}

					if(_cartoonSeason.Name == _tempCartoonEpisode.Name &&
					   _cartoonSeason.Description == _tempCartoonSeason.Description)
					{
						SeasonHaveChanges = false;
						return false;
					}

					SeasonHaveChanges = true;
					return true;
				}


				return false;
			}
		}

		public void AddEpisode()
		{
			var count = Episodes.Count + 1;

			var defaultEpisode = new CartoonEpisode
			{
				CartoonSeasonId = SeasonId,
				Checked = true,
				DelayedSkip = new TimeSpan(),
				SkipCount = 7,
				CreditsStart = new TimeSpan(0, 21, 30),
				Name = $"Название {count} эпизода",
				Description = $"Описание {count} эпизода",
				Number = count
			};

			using(var ctx = new CVDbContext())
			{

			}

			//CartoonEpisodes.Add();
		}

		public void EditEpisode()
		{

		}

		public bool CanEditEpisode => SelectedCartoonEpisode != null;

		public void RemoveEpisode()
		{

		}

		public bool CanRemoveEpisode => SelectedCartoonEpisode != null;

		public void CancelSelection()
		{
			SelectedCartoonEpisode = null;
			NotifySeasonList();
		}

		public bool CanCancelSelection => SelectedCartoonEpisode != null;

		public void SelectionChanged()
		{
			ChangeEpisodeEditingFrame();
		}

		#endregion
	}
}
