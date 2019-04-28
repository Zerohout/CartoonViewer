// ReSharper disable CheckNamespace
namespace CartoonViewer.Settings.CartoonEditorFolder.ViewModels
{
	using System;
	using System.Data.Entity;
	using System.Linq;
	using System.Windows.Controls;
	using System.Windows.Input;
	using Caliburn.Micro;
	using CartoonViewer.ViewModels;
	using Database;
	using Helpers;
	using Models.CartoonModels;
	using static Helpers.Cloner;

	public partial class VoiceOversEditingViewModel : Screen, ISettingsViewModel
	{
		public void TBoxDoubleClick(TextBox source)
		{
			source.SelectAll();
			
		}


		/// <summary>
		/// Двойной клик по списку глобальных озвучек
		/// </summary>
		public void DoubleClickOnGlobalVoiceOverList()
		{
			if (EditMode)
			{
				EditSelectedGlobalVoiceOver();
			}
			else
			{
				MoveToCartoonVoiceOvers();
			}
		}
		/// <summary>
		/// Двойной клик по списку озвучек м/с
		/// </summary>
		public void DoubleClickOnCartoonVoiceOverList()
		{
			if (EditMode)
			{
				EditSelectedGlobalVoiceOver();
			}
			else
			{
				MoveToEpisodeVoiceOvers();
			}
		}

		/// <summary>
		/// Действие при изменении текста
		/// </summary>
		public void TextChanged()
		{
			NotifyChanges();
		}

		#region Selections actions

		/// <summary>
		/// Снять выделение с текущего м/ф
		/// </summary>
		public void CancelCartoonSelection()
		{
			if(SelectedSeason != null)
			{
				CancelSeasonSelection();
			}
			SelectedCartoon = null;

			if(CartoonVoiceOvers.Count > 0)
			{
				CartoonVoiceOvers = new BindableCollection<CartoonVoiceOver>();
			}
		}

		public bool CanCancelCartoonSelection => SelectedCartoon != null && IsNotEditing;

		/// <summary>
		/// Снять выделение с текущего сезона
		/// </summary>
		public void CancelSeasonSelection()
		{
			if(SelectedEpisode != null)
			{
				CancelEpisodeSelection();
			}
			SelectedSeason = null;
		}

		public bool CanCancelSeasonSelection => SelectedSeason != null && IsNotEditing;

		/// <summary>
		/// Снять выделение с текущего эпизода
		/// </summary>
		public void CancelEpisodeSelection()
		{
			SelectedEpisode = null;
			if(EpisodeVoiceOvers.Count > 0)
			{
				EpisodeVoiceOvers = new BindableCollection<CartoonVoiceOver>();
			}
		}

		public bool CanCancelEpisodeSelection => SelectedEpisode != null && IsNotEditing;

		/// <summary>
		/// Прокрутка списка к выбранной озвучке
		/// </summary>
		/// <param name="lb"></param>
		public void VoiceOverSelectionChanged(ListBox lb)
		{
			lb.ScrollIntoView(lb.SelectedItem);
		}

		#endregion

		#region Shared actions

		public void KeyDown(KeyEventArgs e)
		{
			switch(e.KeyboardDevice.Modifiers)
			{
				case ModifierKeys.Shift:
					switch(e.Key)
					{
						case Key.Delete:
							RemoveGlobalVoiceOverAction();
							break;
						default:
							break;
					}
					break;
				case ModifierKeys.Control:
					switch(e.Key)
					{
						case Key.Delete:
							RemoveSelectedCartoonVoiceOver();
							break;
						case Key.S:
							if(CanSaveChanges) SaveChanges();
							break;
					}
					break;
				case ModifierKeys.None:
					switch(e.Key)
					{
						case Key.Delete:
							RemoveSelectedEpisodeVoiceOver();
							break;
						case Key.Escape:
							if(IsNotEditing is false)
							{
								if (CanSaveChanges)
								{
									CancelChanges();
									return;
								}

								if (CanUnlockEditingInterface)
								{
									UnlockEditingInterface();
									return;
								}
							}

							if(SelectedGlobalVoiceOver != null)
							{
								SelectedGlobalVoiceOver = null;
								return;
							}
							Exit();
							break;
					}
					break;
			}
		}

		/// <summary>
		/// Разблокировать интерфейс и скрыть поля редактирования выбранной озвучки
		/// </summary>
		public void UnlockEditingInterface()
		{
			EditedVoiceOver = null;
			TempEditedVoiceOver = null;
			IsNotEditing = true;
		}

		public bool CanUnlockEditingInterface
		{
			get
			{
				if(EditedVoiceOver == null)
				{
					return false;
				}

				if(HasChanges)
				{
					return false;
				}

				if(string.IsNullOrEmpty(EditedVoiceOver.Name))
				{
					return false;
				}

				return true;

			}
		}

		#endregion

		#region Global Voice overs actions

		/// <summary>
		/// Добавить новую глобальную озвучку
		/// </summary>
		public async void AddGlobalVoiceOver()
		{
			var newVoiceOver = await CreateNewVoiceOver();

			GlobalVoiceOvers.Add(newVoiceOver);
			NotifyOfPropertyChange(() => GlobalVoiceOvers);
			SelectedGlobalVoiceOver = GlobalVoiceOvers.Count > 0
				? GlobalVoiceOvers.Last()
				: null;
		}
		public bool CanAddGlobalVoiceOver => IsNotEditing;

		/// <summary>
		/// Редактировать выбранную глобальную озвучку
		/// </summary>
		public void EditSelectedGlobalVoiceOver()
		{
			if(SelectedGlobalVoiceOver == null)
				return;
			EditedVoiceOver = CloneVoiceOver(SelectedGlobalVoiceOver);
			TempEditedVoiceOver = CloneVoiceOver(SelectedGlobalVoiceOver);
			IsNotEditing = false;
		}
		public bool CanEditSelectedGlobalVoiceOver => SelectedGlobalVoiceOver != null && IsNotEditing;

		/// <summary>
		/// Удалить выбранную озвучку из всех списков
		/// </summary>
		public void RemoveGlobalVoiceOverAction()
		{
			if(SelectedGlobalVoiceOver == null)
				return;

			using(var ctx = new CVDbContext(SettingsHelper.AppDataPath))
			{
				var voiceOver = ctx.VoiceOvers
								   .Include(vo => vo.Cartoons)
								   .Include(vo => vo.CartoonEpisodes)
								   .Include(vo => vo.CheckedEpisodes)
								   .First(vo => vo.CartoonVoiceOverId == SelectedVoiceOverId);

				// Коллекция всех эпизодов, где встречается выбранная озвучка
				var episodes = ctx.CartoonEpisodes
								  .Include(ce => ce.CartoonVoiceOver)
								  .Include(ce => ce.EpisodeVoiceOvers)
								  .Where(ce => ce.EpisodeVoiceOvers
												 .Any(evo => evo.CartoonVoiceOverId == SelectedVoiceOverId));


				foreach(var episode in episodes.ToList())
				{
					voiceOver.CartoonEpisodes.Remove(episode);

					if(voiceOver.CartoonEpisodes
								 .Any(ce => ce.CartoonEpisodeId == episode.CartoonEpisodeId))
					{
						voiceOver.CheckedEpisodes.Remove(episode);
						episode.EpisodeVoiceOvers.Remove(voiceOver);

						// Удаление озвучки из списка выбранных озвучек эпизодов
						// для полной очистки БД от объекта
						if(episode.EpisodeVoiceOvers.Count > 0)
						{
							var checkedVoiceOver = ctx.VoiceOvers
													  .First(vo => vo.CartoonVoiceOverId ==
																   episode.EpisodeVoiceOvers
																		  .First().CartoonVoiceOverId);

							checkedVoiceOver.CheckedEpisodes.Add(episode);
							episode.CartoonVoiceOver = checkedVoiceOver;
						}
					}
				}

				var cartoons = ctx.Cartoons
								  .Where(c => c.CartoonVoiceOvers
											   .Any(cvo => cvo.CartoonVoiceOverId == SelectedVoiceOverId));

				foreach(var cartoon in cartoons.ToList())
				{
					voiceOver.Cartoons.Remove(cartoon);
				}


				ctx.VoiceOvers.Remove(voiceOver);
				ctx.SaveChanges();
			}


			UpdateVoiceOverList();
		}

		public bool CanRemoveGlobalVoiceOverAction => SelectedGlobalVoiceOver != null && IsNotEditing;

		/// <summary>
		/// Снять выделение с выбранной глобальной озвучки
		/// </summary>
		public void CancelGlobalVoiceOverSelection() => SelectedGlobalVoiceOver = null;

		public bool CanCancelGlobalVoiceOverSelection => SelectedGlobalVoiceOver != null && IsNotEditing;

		/// <summary>
		/// Копировать выбранную глобальную озвучку в текущий м/ф
		/// </summary>
		public void MoveToCartoonVoiceOvers()
		{
			if(SelectedVoiceOverId == 0)
			{
				throw new Exception("Id выбраной озвучки м/ф равен 0");
			}

			if(CartoonVoiceOvers.Any(cvo => cvo.CartoonVoiceOverId == SelectedVoiceOverId))
			{
				SelectedCartoonVoiceOver = CartoonVoiceOvers
					.First(cvo => cvo.CartoonVoiceOverId == SelectedVoiceOverId);
				return;
			}

			using(var ctx = new CVDbContext(Helpers.SettingsHelper.AppDataPath))
			{
				var cartoon = ctx.Cartoons
								 .Include(c => c.CartoonVoiceOvers)
								 .Single(c => c.CartoonId == IdList.CartoonId);

				ctx.VoiceOvers
				   .Include(vo => vo.Cartoons)
				   .Single(vo => vo.CartoonVoiceOverId == SelectedVoiceOverId)
				   .Cartoons.Add(cartoon);
				ctx.SaveChanges();
			}

			var voiceOver = GlobalVoiceOvers.First(vo => vo.CartoonVoiceOverId == SelectedVoiceOverId);

			CartoonVoiceOvers.Add(voiceOver);
			NotifyOfPropertyChange(() => EpisodeVoiceOvers);

			SelectedCartoonVoiceOver = voiceOver;
			NotifyOfPropertyChange(() => SelectedCartoonVoiceOver);
		}

		public bool CanMoveToCartoonVoiceOvers
		{
			get
			{
				if(SelectedGlobalVoiceOver == null ||
				   SelectedCartoon == null ||
				   IsNotEditing is false)
				{
					return false;
				}

				if(CartoonVoiceOvers.Any(evo => evo.CartoonVoiceOverId == SelectedVoiceOverId))
				{
					return false;
				}

				return true;
			}
		}

		/// <summary>
		/// Копировать выбранную глобальную озвучку сразу в текущий эпизод
		/// </summary>
		public void MoveFromGlobalToEpisodeVoiceOvers()
		{
			MoveToCartoonVoiceOvers();
			MoveToEpisodeVoiceOvers();
		}

		public bool CanMoveFromGlobalToEpisodeVoiceOvers
		{
			get
			{
				if(SelectedGlobalVoiceOver == null ||
				   SelectedCartoon == null ||
				   SelectedEpisode == null ||
				   IsNotEditing is false)
				{
					return false;
				}

				if(EpisodeVoiceOvers.Any(evo => evo.CartoonVoiceOverId == SelectedVoiceOverId))
				{
					return false;
				}

				return true;
			}
		}

		#endregion

		#region Cartoon Voice overs actions

		/// <summary>
		/// Добавить новую озвучку в текущий м/ф
		/// </summary>
		public void AddCartoonVoiceOver()
		{
			AddGlobalVoiceOver();
			MoveToCartoonVoiceOvers();

		}
		public bool CanAddCartoonVoiceOver => IsNotEditing;

		/// <summary>
		/// Редактировать выбранную озвучку текущего м/ф
		/// </summary>
		public void EditSelectedCartoonVoiceOver()
		{
			EditSelectedGlobalVoiceOver();
		}
		public bool CanEditSelectedCartoonVoiceOver => SelectedCartoonVoiceOver != null && IsNotEditing;

		/// <summary>
		/// Удалить выбранную озвучку из текущего м/ф
		/// </summary>
		public void RemoveSelectedCartoonVoiceOver()
		{
			if(SelectedCartoonVoiceOver == null)
				return;

			using(var ctx = new CVDbContext(SettingsHelper.AppDataPath))
			{
				var voiceOver = ctx.VoiceOvers
								   .Include(vo => vo.Cartoons)
								   .Include(vo => vo.CheckedEpisodes)
								   .Include(vo => vo.CartoonEpisodes)
								   .First(vo => vo.CartoonVoiceOverId == SelectedVoiceOverId);

				// Коллекция эпизодов только выбранного м/с
				// и только с присутствующей в нем выбранной озвучки
				var episodes = ctx.CartoonEpisodes
								  .Where(ce => ce.CartoonId == IdList.CartoonId &&
											   ce.EpisodeVoiceOvers
												 .Any(evo => evo.CartoonVoiceOverId == SelectedVoiceOverId));

				foreach(var episode in episodes.ToList())
				{
					voiceOver.CartoonEpisodes.Remove(episode);

					if(voiceOver.CheckedEpisodes
								.Any(ce => ce.CartoonEpisodeId == IdList.EpisodeId))
					{
						voiceOver.CheckedEpisodes.Remove(episode);
						episode.EpisodeVoiceOvers.Remove(voiceOver);

						if(episode.EpisodeVoiceOvers.Count > 0)
						{
							var checkedVoiceOver = ctx.VoiceOvers
													  .First(vo => vo.CartoonVoiceOverId ==
																   episode.EpisodeVoiceOvers
																		  .First().CartoonVoiceOverId);

							checkedVoiceOver.CheckedEpisodes.Add(episode);
							episode.CartoonVoiceOver = checkedVoiceOver;
						}
					}
				}

				var cartoon = ctx.Cartoons.Find(IdList.CartoonId);

				voiceOver.Cartoons.Remove(cartoon);

				ctx.SaveChanges();
			}

			UpdateVoiceOverList();
		}

		public bool CanRemoveSelectedCartoonVoiceOver => SelectedCartoonVoiceOver != null && IsNotEditing;

		/// <summary>
		/// Снять выделение с выбранной озвучки текущего м/ф
		/// </summary>
		public void CancelCartoonVoiceOverSelection() => SelectedGlobalVoiceOver = null;

		public bool CanCancelCartoonVoiceOverSelection => SelectedCartoonVoiceOver != null && IsNotEditing;

		/// <summary>
		/// Копировать выбранную озвучку м/ф в текущий эпизод
		/// </summary>
		public void MoveToEpisodeVoiceOvers()
		{
			if(SelectedVoiceOverId == 0)
			{
				throw new Exception("Id выбраной озвучки м/ф равен 0");
			}

			using(var ctx = new CVDbContext(Helpers.SettingsHelper.AppDataPath))
			{
				var episode = ctx.CartoonEpisodes
								 .Include(ce => ce.EpisodeVoiceOvers)
								 .Single(ce => ce.CartoonEpisodeId == IdList.EpisodeId);

				if(episode.EpisodeVoiceOvers.Count == 0)
				{
					ctx.VoiceOvers
					   .Include(vo => vo.CheckedEpisodes)
					   .Single(vo => vo.CartoonVoiceOverId == SelectedVoiceOverId)
					   .CheckedEpisodes.Add(episode);
				}

				ctx.VoiceOvers
				   .Include(vo => vo.CartoonEpisodes)
				   .Single(vo => vo.CartoonVoiceOverId == SelectedVoiceOverId)
				   .CartoonEpisodes.Add(episode);

				ctx.SaveChanges();
			}

			var voiceOver = GlobalVoiceOvers.First(vo => vo.CartoonVoiceOverId == SelectedVoiceOverId);

			EpisodeVoiceOvers.Add(voiceOver);
			NotifyOfPropertyChange(() => EpisodeVoiceOvers);
			SelectedEpisodeVoiceOver = voiceOver;
		}

		public bool CanMoveToEpisodeVoiceOvers
		{
			get
			{
				if(SelectedCartoonVoiceOver == null ||
				   SelectedEpisode == null ||
				   IsNotEditing is false)
				{
					return false;
				}

				if(EpisodeVoiceOvers.Any(evo => evo.CartoonVoiceOverId == SelectedVoiceOverId))
				{
					return false;
				}

				return true;
			}
		}

		#endregion

		#region Episode Voice overs actions

		/// <summary>
		/// Добавить озвучку в текущий эпизод
		/// </summary>
		public void AddEpisodeVoiceOverAction()
		{
			AddGlobalVoiceOver();
			MoveFromGlobalToEpisodeVoiceOvers();
		}

		public bool CanAddEpisodeVoiceOverAction => IsNotEditing;

		/// <summary>
		/// Редактировать выбранную озвучку текущего эпизода
		/// </summary>
		public void EditSelectedEpisodeVoiceOver()
		{
			EditSelectedGlobalVoiceOver();
		}
		public bool CanEditSelectedEpisodeVoiceOver => SelectedEpisodeVoiceOver != null && IsNotEditing;

		/// <summary>
		/// Удалить выбранную озвучку из текущего эпизода
		/// </summary>
		public void RemoveSelectedEpisodeVoiceOver()
		{
			if(SelectedEpisodeVoiceOver == null)
				return;

			using(var ctx = new CVDbContext(SettingsHelper.AppDataPath))
			{
				var episode = ctx.CartoonEpisodes
								 .Include(ce => ce.CartoonVoiceOver)
								 .Include(ce => ce.EpisodeVoiceOvers)
								 .First(ce => ce.CartoonEpisodeId == IdList.EpisodeId);

				var voiceOver = ctx.VoiceOvers
								   .Include(vo => vo.CartoonEpisodes)
								   .Include(vo => vo.CheckedEpisodes)
								   .First(vo => vo.CartoonVoiceOverId == SelectedVoiceOverId);


				voiceOver.CartoonEpisodes.Remove(episode);

				if(voiceOver.CheckedEpisodes
							.Any(ce => ce.CartoonEpisodeId == IdList.EpisodeId))
				{
					voiceOver.CheckedEpisodes.Remove(episode);
					episode.EpisodeVoiceOvers.Remove(voiceOver);

					if(episode.EpisodeVoiceOvers.Count > 0)
					{
						var checkedVoiceOver = ctx.VoiceOvers
												  .First(vo => vo.CartoonVoiceOverId ==
															   episode.EpisodeVoiceOvers
																	  .First().CartoonVoiceOverId);

						checkedVoiceOver.CheckedEpisodes.Add(episode);
						episode.CartoonVoiceOver = checkedVoiceOver;
					}
				}

				ctx.SaveChanges();

			}

			UpdateVoiceOverList();

			//SelectedGlobalVoiceOver = GlobalVoiceOvers
			//	.First(gvo => gvo.CartoonVoiceOverId == EpisodeVoiceOvers
			//											.Last().CartoonVoiceOverId);
		}

		public bool CanRemoveSelectedEpisodeVoiceOver => SelectedEpisodeVoiceOver != null && IsNotEditing;

		/// <summary>
		/// Снять выделение с выбранной озвучки текущего эпизода
		/// </summary>
		public void CancelEpisodeVoiceOverSelection() => SelectedGlobalVoiceOver = null;

		public bool CanCancelEpisodeVoiceOverSelection => SelectedEpisodeVoiceOver != null && IsNotEditing;

		#endregion

		#region Changes buttons

		/// <summary>
		/// Сохранить изменения
		/// </summary>
		public async void SaveChanges()
		{
			if(SelectedVoiceOverId == 0)
			{
				throw new Exception("Id выбраной озвучки м/ф равен 0");
			}

			using(var ctx = new CVDbContext(Helpers.SettingsHelper.AppDataPath))
			{
				var voiceOver = ctx.VoiceOvers.Find(SelectedVoiceOverId);

				CopyChanges(ref voiceOver, EditedVoiceOver);

				ctx.Entry(voiceOver).State = EntityState.Modified;
				await ctx.SaveChangesAsync();
			}

			TempEditedVoiceOver = CloneVoiceOver(EditedVoiceOver);

			NotifyChanges();
			IsNotEditing = true;

			UpdateVoiceOverList();


		}



		public bool CanSaveChanges => HasChanges;

		/// <summary>
		/// Отменить изменения
		/// </summary>
		public void CancelChanges()
		{
			EditedVoiceOver = CloneVoiceOver(TempEditedVoiceOver);
			NotifyChanges();
		}

		public bool CanCancelChanges => HasChanges;

		#endregion

		/// <summary>
		/// Выйти из редактора
		/// </summary>
		public void Exit()
		{
			if(Parent is WindowsManagerViewModel wm)
			{
				wm.TryClose();
			}

			TryClose();
		}
	}
}
