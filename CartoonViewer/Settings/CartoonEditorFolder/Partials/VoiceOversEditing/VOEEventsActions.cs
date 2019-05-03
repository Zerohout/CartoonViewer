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
	using static Helpers.Helper;
	using static Helpers.SettingsHelper;

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
			if(EditMode)
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
			if(EditMode)
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
			if(CanCancelCartoonSelection is false)
				return;

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
			if(CanCancelSeasonSelection is false)
				return;

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
			if(CanCancelEpisodeSelection is false)
				return;

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
							if(CanSaveChanges)
								SaveChanges();
							break;
						case Key.OemPlus:
							if(CanSelectNextEpisode)
							{
								SelectNextEpisode();
								return;
							}

							break;
						case Key.OemMinus:
							if(CanSelectPreviousEpisode)
							{
								SelectPreviousEpisode();
								return;
							}

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
								if(CanSaveChanges)
								{
									CancelChanges();
									return;
								}

								if(CanUnlockEditingInterface)
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
			if(CanUnlockEditingInterface is false)
				return;

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
		public void AddGlobalVoiceOver()
		{
			if(CanAddGlobalVoiceOver is false)
				return;

			var newVoiceOver = CreateNewVoiceOver();

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
			if(CanEditSelectedGlobalVoiceOver is false)
				return;
			if(SelectedGlobalVoiceOver == null)
				return;
			EditedVoiceOver = CloneObject<CartoonVoiceOver>(SelectedGlobalVoiceOver);
			TempEditedVoiceOver = CloneObject<CartoonVoiceOver>(SelectedGlobalVoiceOver);
			IsNotEditing = false;
		}
		public bool CanEditSelectedGlobalVoiceOver => SelectedGlobalVoiceOver != null && IsNotEditing;

		/// <summary>
		/// Удалить выбранную озвучку из всех списков
		/// </summary>
		public void RemoveGlobalVoiceOverAction()
		{
			if(CanRemoveGlobalVoiceOverAction is false)
				return;

			if(SelectedGlobalVoiceOver == null)
				return;

			using(var ctx = new CVDbContext(AppDataPath))
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

				var options = ctx.EpisodeOptions.Where(eo => eo.CartoonVoiceOverId == voiceOver.CartoonVoiceOverId);

				if(options.Any())
				{
					ctx.EpisodeOptions.RemoveRange(options);
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
		public void CancelGlobalVoiceOverSelection()
		{
			if(CanCancelGlobalVoiceOverSelection is false)
				return;

			SelectedGlobalVoiceOver = null;
		}

		public bool CanCancelGlobalVoiceOverSelection => SelectedGlobalVoiceOver != null && IsNotEditing;

		/// <summary>
		/// Копировать выбранную глобальную озвучку в текущий м/ф
		/// </summary>
		public void MoveToCartoonVoiceOvers()
		{
			if(CanMoveToCartoonVoiceOvers is false)
				return;

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

			using(var ctx = new CVDbContext(AppDataPath))
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
			if(CanMoveFromGlobalToEpisodeVoiceOvers is false)
				return;

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
			if(CanAddCartoonVoiceOver is false)
				return;

			AddGlobalVoiceOver();
			MoveToCartoonVoiceOvers();

		}
		public bool CanAddCartoonVoiceOver => IsNotEditing;

		/// <summary>
		/// Редактировать выбранную озвучку текущего м/ф
		/// </summary>
		public void EditSelectedCartoonVoiceOver()
		{
			if(CanEditSelectedCartoonVoiceOver is false)
				return;
			EditSelectedGlobalVoiceOver();
		}
		public bool CanEditSelectedCartoonVoiceOver => SelectedCartoonVoiceOver != null && IsNotEditing;

		/// <summary>
		/// Удалить выбранную озвучку из текущего м/ф
		/// </summary>
		public void RemoveSelectedCartoonVoiceOver()
		{
			if(CanRemoveSelectedCartoonVoiceOver is false)
				return;

			if(SelectedCartoonVoiceOver == null)
				return;

			using(var ctx = new CVDbContext(AppDataPath))
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

				var options = ctx.EpisodeOptions
								 .Where(eo => eo.CartoonVoiceOverId == voiceOver.CartoonVoiceOverId &&
											  eo.CartoonEpisode.CartoonId == IdList.CartoonId);

				if(options.Any())
				{
					ctx.EpisodeOptions.RemoveRange(options);
				}

				voiceOver.Cartoons.Remove(cartoon);

				ctx.SaveChanges();
			}

			UpdateVoiceOverList();
		}

		public bool CanRemoveSelectedCartoonVoiceOver => SelectedCartoonVoiceOver != null && IsNotEditing;

		/// <summary>
		/// Снять выделение с выбранной озвучки текущего м/ф
		/// </summary>
		public void CancelCartoonVoiceOverSelection()
		{
			if(CanCancelCartoonVoiceOverSelection is false)
				return;

			SelectedGlobalVoiceOver = null;
		}

		public bool CanCancelCartoonVoiceOverSelection => SelectedCartoonVoiceOver != null && IsNotEditing;

		/// <summary>
		/// Копировать выбранную озвучку м/ф в текущий эпизод
		/// </summary>
		public void MoveToEpisodeVoiceOvers()
		{
			if(CanMoveToEpisodeVoiceOvers is false)
				return;

			if(SelectedVoiceOverId == 0)
			{
				throw new Exception("Id выбраной озвучки м/ф равен 0");
			}

			using(var ctx = new CVDbContext(AppDataPath))
			{
				var episode = ctx.CartoonEpisodes
								 .Include(ce => ce.EpisodeVoiceOvers)
								 .Single(ce => ce.CartoonEpisodeId == IdList.EpisodeId);

				var episodeOption = new EpisodeOption
				{
					CartoonEpisodeId = episode.CartoonEpisodeId,
					CartoonVoiceOverId = SelectedVoiceOverId,
					Name = $"{SelectedGlobalVoiceOver.Name}_Option"
				};

				ctx.EpisodeOptions.Add(episodeOption);
				ctx.SaveChanges();
				episodeOption = ctx.EpisodeOptions.ToList().Last();
				// создание нового джампера
				var jumper = new Jumper
				{
					EpisodeOptionId = episodeOption.EpisodeOptionId,
					Number = 1
				};

				ctx.Jumpers.Add(jumper);
				ctx.SaveChanges();

				episodeOption = ctx.EpisodeOptions
								   .Include(eo => eo.Jumpers)
								   .ToList().Last();

				var totalSkipCount = episodeOption.Jumpers.Sum(j => j.SkipCount);

				var duration = episodeOption.CreditsStart - new TimeSpan(0, 0, totalSkipCount * 5);

				episodeOption.Duration = duration;
				ctx.SaveChanges();


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
			if(CanAddEpisodeVoiceOverAction is false)
				return;
			AddGlobalVoiceOver();
			MoveFromGlobalToEpisodeVoiceOvers();
		}

		public bool CanAddEpisodeVoiceOverAction => IsNotEditing;

		/// <summary>
		/// Редактировать выбранную озвучку текущего эпизода
		/// </summary>
		public void EditSelectedEpisodeVoiceOver()
		{
			if(CanEditSelectedEpisodeVoiceOver is false)
				return;
			EditSelectedGlobalVoiceOver();
		}
		public bool CanEditSelectedEpisodeVoiceOver => SelectedEpisodeVoiceOver != null && IsNotEditing;

		/// <summary>
		/// Удалить выбранную озвучку из текущего эпизода
		/// </summary>
		public void RemoveSelectedEpisodeVoiceOver()
		{
			if(CanRemoveSelectedEpisodeVoiceOver is false)
				return;

			if(SelectedEpisodeVoiceOver == null)
				return;

			using(var ctx = new CVDbContext(AppDataPath))
			{
				var episode = ctx.CartoonEpisodes
								 .Include(ce => ce.CartoonVoiceOver)
								 .Include(ce => ce.EpisodeVoiceOvers)
								 .First(ce => ce.CartoonEpisodeId == IdList.EpisodeId);

				var options = ctx.EpisodeOptions
									   .Where(eo => eo.CartoonVoiceOverId == SelectedVoiceOverId &&
													eo.CartoonEpisodeId == IdList.EpisodeId);
				if(options.Any())
				{
					ctx.EpisodeOptions.RemoveRange(options);
				}


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

					// присваивание активной озвучки новой активной озвучки
					// если таковая была удалена
					if(episode.EpisodeVoiceOvers.Count > 0)
					{
						var tempId = episode.EpisodeVoiceOvers
											.First().CartoonVoiceOverId;

						var checkedVoiceOver = ctx.VoiceOvers
												  .First(vo => vo.CartoonVoiceOverId == tempId);

						checkedVoiceOver.CheckedEpisodes.Add(episode);
						episode.CartoonVoiceOver = checkedVoiceOver;
					}
				}

				ctx.SaveChanges();

			}

			UpdateVoiceOverList();
		}

		public bool CanRemoveSelectedEpisodeVoiceOver => SelectedEpisodeVoiceOver != null && IsNotEditing;

		/// <summary>
		/// Снять выделение с выбранной озвучки текущего эпизода
		/// </summary>
		public void CancelEpisodeVoiceOverSelection()
		{
			if(CanCancelEpisodeVoiceOverSelection is false)
				return;

			SelectedGlobalVoiceOver = null;
		}

		public bool CanCancelEpisodeVoiceOverSelection => SelectedEpisodeVoiceOver != null && IsNotEditing;

		#endregion

		#region Changes buttons

		/// <summary>
		/// Сохранить изменения
		/// </summary>
		public void SaveChanges()
		{
			if(CanSaveChanges is false)
				return;

			if(SelectedVoiceOverId == 0)
			{
				throw new Exception("Id выбраной озвучки м/ф равен 0");
			}

			using(var ctx = new CVDbContext(AppDataPath))
			{
				var voiceOver = ctx.VoiceOvers.Find(SelectedVoiceOverId);

				var options =
					ctx.EpisodeOptions.Where(eo => eo.CartoonVoiceOverId == EditedVoiceOver.CartoonVoiceOverId);

				if(voiceOver.Name != EditedVoiceOver.Name)
				{
					foreach(var option in options)
					{
						option.Name = $"{EditedVoiceOver.Name}_Option";
						ctx.Entry(option).State = EntityState.Modified;
					}
				}

				CopyChanges(ref voiceOver, EditedVoiceOver);

				ctx.Entry(voiceOver).State = EntityState.Modified;
				ctx.SaveChanges();
			}

			TempEditedVoiceOver = CloneObject<CartoonVoiceOver>(EditedVoiceOver);

			NotifyChanges();
			IsNotEditing = true;

			UpdateVoiceOverList();

			NotifyChanges();
			
		}



		public bool CanSaveChanges => HasChanges;

		/// <summary>
		/// Отменить изменения
		/// </summary>
		public void CancelChanges()
		{
			if(CanCancelChanges is false)
				return;

			EditedVoiceOver = CloneObject<CartoonVoiceOver>(TempEditedVoiceOver);
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
		/// <summary>
		/// Индексы м/с
		/// </summary>
		private (int CurrentIndex, int EndIndex) CartoonIndexes = (-1,0);
		/// <summary>
		/// Индексы сезонов
		/// </summary>
		private (int CurrentIndex, int EndIndex) SeasonIndexes = (-1, 0);
		/// <summary>
		/// Индексы эпизодов
		/// </summary>
		private (int CurrentIndex, int EndIndex) EpisodeIndexes = (-1, 0);

		/// <summary>
		/// Выбрать предыдущий м/с
		/// </summary>
		public void SelectPreviousCartoon()
		{
			if (CanSelectPreviousCartoon is false) return;

			SelectedCartoon = Cartoons[CartoonIndexes.CurrentIndex - 1];
			SelectedSeason = Seasons.FirstOrDefault();
		}

		public bool CanSelectPreviousCartoon => Cartoons.Count > 0 &&
		                                         CartoonIndexes.CurrentIndex > 0;

		/// <summary>
		/// Выбрать следующий м/с
		/// </summary>
		public void SelectNextCartoon()
		{
			if(CanSelectNextCartoon is false)
				return;
			SelectedCartoon = Cartoons[CartoonIndexes.CurrentIndex + 1];
			SelectedSeason = Seasons.FirstOrDefault();
		}

		public bool CanSelectNextCartoon => Cartoons.Count > 0 &&
											CartoonIndexes.CurrentIndex < CartoonIndexes.EndIndex;
		/// <summary>
		/// Выбрать предыдущий сезон
		/// </summary>
		public void SelectPreviousSeason()
		{
			if (CanSelectPreviousSeason is false) return;

			SelectedSeason = Seasons[SeasonIndexes.CurrentIndex - 1];

			SelectedEpisode = Episodes.FirstOrDefault();
		}

		public bool CanSelectPreviousSeason => Seasons.Count > 0 &&
		                                       SeasonIndexes.CurrentIndex > 0;

		/// <summary>
		/// Выбрать следующий сезон
		/// </summary>
		public void SelectNextSeason()
		{
			if(CanSelectNextSeason is false)
				return;

			SelectedSeason = Seasons[SeasonIndexes.CurrentIndex + 1];

			SelectedEpisode = Episodes.FirstOrDefault();
		}

		public bool CanSelectNextSeason => Seasons.Count > 0 &&
										   SeasonIndexes.CurrentIndex < SeasonIndexes.EndIndex;

		public void SelectPreviousEpisode()
		{
			if (CanSelectPreviousEpisode is false) return;

			SelectedEpisode = Episodes[EpisodeIndexes.CurrentIndex - 1];
		}

		public bool CanSelectPreviousEpisode => Episodes.Count > 0 &&
		                                        EpisodeIndexes.CurrentIndex > 0;

		/// <summary>
		/// Выбрать следующий эпизод
		/// </summary>
		public void SelectNextEpisode()
		{
			if(CanSelectNextEpisode is false)
				return;
			SelectedEpisode = Episodes[EpisodeIndexes.CurrentIndex + 1];
		}

		public bool CanSelectNextEpisode => Episodes.Count > 0 &&
											EpisodeIndexes.CurrentIndex < EpisodeIndexes.EndIndex;


	}
}
