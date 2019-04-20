namespace CartoonViewer.Settings.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.Data.Entity;
	using System.Linq;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Input;
	using System.Windows.Navigation;
	using CartoonViewer.ViewModels;
	using Database;
	using Helpers;
	using Models.CartoonModels;
	using static Helpers.Cloner;
	using static Helpers.Helper;
	using Screen = Caliburn.Micro.Screen;

	public partial class EpisodesEditingViewModel : Screen, ISettingsViewModel
	{
		#region EventsActions

		public void EditVoiceOvers() { WinMan.ShowDialog(new VoiceOversEditingViewModel(GlobalIdList)); }
		public void TextChanged()
		{
			NotifyEditingButtons();
		}

		public void TimeChanged()
		{
			var time = ConvertFromEpisodeTime(EpisodeTime);
			
			EditingEpisode.DelayedSkip = time.DelayedSkip;
			EditingEpisode.SkipCount = time.SkipCount;
			EditingEpisode.CreditsStart = time.CreditsStart;
			EditingEpisode.Duration = CalculatingDuration(EditingEpisode);
			NotifyOfPropertyChange(() => EpisodeDuration);
			NotifyEditingButtons();
		}

		public async void SaveChanges()
		{
			using(var ctx = new CVDbContext())
			{
				var episode = ctx.CartoonEpisodes
								 .Include(c => c.EpisodeVoiceOvers)
								 .Single(c => c.CartoonEpisodeId == GlobalIdList.EpisodeId);

				episode.Name = EditingEpisode.Name;
				episode.Description = EditingEpisode.Description;
				episode.DelayedSkip = EditingEpisode.DelayedSkip;
				episode.SkipCount = EditingEpisode.SkipCount;
				episode.CreditsStart = EditingEpisode.CreditsStart;

				ctx.Entry(episode).State = EntityState.Modified;

				await ctx.SaveChangesAsync();
			}

			TempEpisode = CloneEpisode(EditingEpisode);
			NotifyEditingButtons();
		}


		public bool CanSaveChanges => HasChanges;

		public void CancelEditing()
		{
			IsNotEditing = true;
			EpisodeEditingVisibility = Visibility.Hidden;
			EditingEpisode = null;
			TempEpisode = null;
			NotifyEditingButtons();
		}

		public bool CanCancelEditing => !HasChanges;

		public void CancelChanges()
		{
			var dvm = new DialogViewModel(null, DialogState.CANCEL_CHANGES);
			WinMan.ShowDialog(dvm);

			if(dvm.DialogResult == DialogResult.NO_ACTION)
			{
				return;
			}

			EditingEpisode = CloneEpisode(TempEpisode);
			NotifyEditingButtons();
		}

		public bool CanCancelChanges => HasChanges;

		public async void AddEpisode()
		{
			var count = Episodes.Count + 1;

			var defaultEpisode = new CartoonEpisode
			{
				CartoonSeasonId = GlobalIdList.SeasonId,
				Checked = true,
				DelayedSkip = new TimeSpan(),
				SkipCount = 7,
				CreditsStart = new TimeSpan(0, 21, 30),
				Name = $"Название {count} эпизода",
				Description = $"Описание {count} эпизода",
				Number = count
			};

			defaultEpisode.Duration = CalculatingDuration(defaultEpisode);

			Episodes.Add(defaultEpisode);
			NotifyOfPropertyChange(() => Episodes);

			using(var ctx = new CVDbContext())
			{
				ctx.CartoonEpisodes.Add(defaultEpisode);
				await ctx.SaveChangesAsync();
				Episodes.Last().CartoonEpisodeId = ctx.CartoonEpisodes
				                                      .ToList()
				                                      .Last().CartoonEpisodeId;
			}

			SelectedEpisode = Episodes.Count > 0
				? Episodes.Last()
				: null;
		}

		public bool CanAddEpisode => IsNotEditing;

		public void EditEpisode()
		{
			LoadEpisodeData();
		}

		public bool CanEditEpisode => SelectedEpisode != null && IsNotEditing;

		public async void RemoveEpisode()
		{

			if (!HasChangesValidation())
			{
				return;
			}
			
			using(var ctx = new CVDbContext())
			{
				var episode = ctx.CartoonEpisodes.Find(GlobalIdList.EpisodeId);

				ctx.CartoonEpisodes.Remove(episode);
				await ctx.SaveChangesAsync();
			}

			Episodes.Remove(SelectedEpisode);
			NotifyOfPropertyChange(() => Episodes);
			SelectedEpisode = Episodes.Count > 0
				? Episodes.Last()
				: null;
		}

		public bool CanRemoveEpisode => SelectedEpisode != null && IsNotEditing;


		private bool HasChangesValidation()
		{
			if(HasChanges)
			{
				var dvm = new DialogViewModel(null, DialogState.SAVE_CHANGES);
				WinMan.ShowDialog(dvm);

				switch(dvm.DialogResult)
				{
					case Helper.DialogResult.YES_ACTION:
						SaveChanges();
						return true;
					case Helper.DialogResult.NO_ACTION:
						return true;
					default:
						return false;
				}
			}

			return true;
		}

		public void CancelSelection()
		{
			if (!HasChangesValidation())
			{
				return;
			}

			SelectedEpisode = null;
		}

		public bool CanCancelSelection => SelectedEpisode != null && IsNotEditing;

		public void ListenKey(KeyEventArgs e)
		{
			if(e.Key == Key.Escape)
			{
				if(IsNotEditing is false)
				{
					if(HasChanges is false)
					{
						CancelEditing();
					}
					else
					{
						CancelChanges();
					}
				}
				else
				{
					((CartoonsControlViewModel) Parent).CancelSeasonSelection();
				}
			}
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
