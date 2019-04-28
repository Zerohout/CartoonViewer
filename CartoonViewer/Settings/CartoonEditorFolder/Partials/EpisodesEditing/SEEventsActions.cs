// ReSharper disable CheckNamespace
namespace CartoonViewer.Settings.CartoonEditorFolder.ViewModels
{
	using System;
	using System.Data.Entity;
	using System.Linq;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Input;
	using CartoonViewer.ViewModels;
	using Database;
	using Helpers;
	using Models.CartoonModels;
	using static Helpers.Cloner;
	using static Helpers.Helper;
	using Screen = Caliburn.Micro.Screen;

	public partial class EpisodesEditingViewModel : Screen, ISettingsViewModel
	{
		public void TBoxDoubleClick(TextBox source)
		{
			source.SelectAll();
		}

		/// <summary>
		/// Сброс даты последнего просмотра эпизода
		/// </summary>
		public void ResetLastDateViewed()
		{
			var newDate = new DateTime(2019, 01, 01);
			using(var ctx = new CVDbContext(SettingsHelper.AppDataPath))
			{
				ctx.CartoonEpisodes
				   .First(ce => ce.CartoonEpisodeId ==
								EditingEpisode.CartoonEpisodeId)
				   .LastDateViewed = newDate;
				ctx.SaveChanges();
			}

			EditingEpisode.LastDateViewed = newDate;
			TempEpisode.LastDateViewed = newDate;
			NotifyOfPropertyChange(() => CanResetLastDateViewed);
			NotifyOfPropertyChange(() => EditingEpisode);
		}

		public bool CanResetLastDateViewed =>
			EditingEpisode?.LastDateViewed != new DateTime(2019, 01, 01);

		/// <summary>
		/// Кнопка Редактор озвучек
		/// </summary>
		public void EditVoiceOvers()
		{
			var wm = new WindowsManagerViewModel(new VoiceOversEditingViewModel(SettingsHelper.GlobalIdList));

			WinMan.ShowDialog(wm);

			UpdateVoiceOverList();
		}


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
			using(var ctx = new CVDbContext(SettingsHelper.AppDataPath))
			{
				var episode = ctx.CartoonEpisodes
								 .Include(c => c.EpisodeVoiceOvers)
								 .Single(c => c.CartoonEpisodeId == SettingsHelper.GlobalIdList.EpisodeId);

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
			var dvm = new DialogViewModel(null, DialogType.CANCEL_CHANGES);
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
				CartoonSeasonId = SettingsHelper.GlobalIdList.SeasonId,
				CartoonId = SettingsHelper.GlobalIdList.CartoonId,
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

			using(var ctx = new CVDbContext(SettingsHelper.AppDataPath))
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
			NotifyOfPropertyChange(() => CanResetLastDateViewed);
		}

		public bool CanEditEpisode => SelectedEpisode != null && IsNotEditing;

		public async void RemoveEpisode()
		{

			if(!HasChangesValidation())
			{
				return;
			}

			using(var ctx = new CVDbContext(SettingsHelper.AppDataPath))
			{
				var episode = ctx.CartoonEpisodes.Find(SettingsHelper.GlobalIdList.EpisodeId);

				if(episode == null)
				{
					throw new Exception("Удаляемый эпизод не найден");
				}

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
				var dvm = new DialogViewModel(null, DialogType.SAVE_CHANGES);
				WinMan.ShowDialog(dvm);

				switch(dvm.DialogResult)
				{
					case DialogResult.YES_ACTION:
						SaveChanges();
						return true;
					case DialogResult.NO_ACTION:
						return true;
					default:
						return false;
				}
			}

			return true;
		}

		public void CancelSelection()
		{
			if(!HasChangesValidation())
			{
				return;
			}

			SelectedEpisode = null;
		}

		public bool CanCancelSelection => SelectedEpisode != null && IsNotEditing;

		public void ListenKey(KeyEventArgs e)
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
							if(IsNotEditing is false)
							{
								if(CanSaveChanges)
								{
									CancelChanges();
									return;
								}
								if(CanCancelEditing)
								{
									CancelEditing();
									return;
								}

								return;
							}

							((CartoonsEditorViewModel)Parent).CancelSeasonSelection();
							break;
					}
					break;
			}
		}

		/// <summary>
		/// Действие при изменении выбора сезона
		/// </summary>
		public void SelectionChanged(ListBox lb)
		{
			lb.ScrollIntoView(lb.SelectedItem);
		}


	}
}
