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
	using Models.CartoonModels;
	using Newtonsoft.Json;
	using static Helpers.Helper;
	using static Helpers.SettingsHelper;
	using Screen = Caliburn.Micro.Screen;

	public partial class EpisodesEditingViewModel : Screen, ISettingsViewModel
	{
		#region TextBox actions

		/// <summary>
		/// Действие при двойном клике по текстовому полю
		/// </summary>
		/// <param name="source"></param>
		public void TBoxDoubleClick(TextBox source)
		{
			source.SelectAll();
		}


		/// <summary>
		/// Действие при изменении текста в полях с текстовыми значениями
		/// </summary>
		public void TextChanged()
		{
			NotifyEditingButtons();
			NotifyOfPropertyChange(() => EditableEpisode);
		}



		/// <summary>
		/// Действие при изменение текста в полях со значениями времени
		/// </summary>
		public void TimeChanged()
		{
			if(SelectedEpisodeOption == null)
				return;

			var time = ConvertFromEpisodeTime(EditableEpisodeTime);

			if(SelectedJumper != null)
			{
				var jumperIndex = Jumpers.IndexOf(SelectedJumper);
				if(jumperIndex > 0)
				{
					var previousJumper = Jumpers[jumperIndex - 1];
					if(time.JumperTime < previousJumper.EndTime)
					{
						SelectedJumper.StartTime = previousJumper.EndTime;
						EditableEpisodeTime = ConvertToEpisodeTime(SelectedEpisodeOption, SelectedJumper);
						TimeChanged();
					}
				}

				SelectedJumper.StartTime = time.JumperTime;
				SelectedJumper.SkipCount = time.SkipCount;
				SelectedEpisodeOption.CreditsStart = time.CreditsStart;
			}

			NotifyEditingProperties();
			NotifyOfPropertyChange(() => EpisodeDuration);
			NotifyOfPropertyChange(() => EditableEpisodeTime);
			NotifyOfPropertyChange(() => SelectedJumper);
		}

		/// <summary>
		/// Проверка на ввод корректных данных в текстовое поле с числовыми значениями
		/// </summary>
		/// <param name="e"></param>
		public void NumericValidation(KeyEventArgs e)
		{
			e.Handled = (e.Key.GetHashCode() >= 34 && e.Key.GetHashCode() <= 43 ||
						 e.Key.GetHashCode() >= 74 && e.Key.GetHashCode() <= 83) is false;
		}

		#endregion

		#region Episode list actions

		/// <summary>
		/// Добавить новый эпизод
		/// </summary>
		public void AddEpisode()
		{
			if(CanAddEpisode is false)
				return;
			using(var ctx = new CVDbContext(AppDataPath))
			{
				var cartoon = ctx.Cartoons
								 .Include(c => c.CartoonVoiceOvers)
								 .First(c => c.CartoonId == GlobalIdList.CartoonId);

				if(cartoon.CartoonVoiceOvers.Count == 0)
				{
					WinMan.ShowDialog(new DialogViewModel(
										  "У выбранного мультсериала отсутствуют озвучки, добавьте одну или более для создания нового эпизода",
										  DialogType.INFO));
					return;
				}

				// загрузка первой озвучки выбранного м/с
				CartoonVoiceOver voiceOver;

				if(DefaultVoiceOver == null)
				{
					voiceOver = ctx.Cartoons
					                   .Include(ce => ce.CartoonVoiceOvers)
					                   .First(c => c.CartoonId == GlobalIdList.CartoonId).CartoonVoiceOvers.First();
				}
				else
				{
					voiceOver = ctx.VoiceOvers
					               .First(vo => vo.CartoonVoiceOverId == 
					                            DefaultVoiceOver.CartoonVoiceOverId);
				}

				var episode = CreateNewEpisode(ctx, voiceOver);

				CreateNewEpisodeOption(ctx, episode, voiceOver);

				episode = ctx.CartoonEpisodes.ToList().Last();

				Episodes.Add(episode);
				NotifyOfPropertyChange(() => Episodes);

			}

			SelectedEpisode = Episodes.LastOrDefault();
			EpisodeIndexes.CurrentIndex = Episodes.IndexOf(SelectedEpisode);
			EpisodeIndexes.EndIndex = Episodes.IndexOf(SelectedEpisode);
			NotifyOfPropertyChange(() => CanEditNextEpisode);
			NotifyOfPropertyChange(() => CanEditPreviousEpisode);
		}

		public bool CanAddEpisode => IsNotEditing;



		/// <summary>
		/// Редактировать выбранный эпизод
		/// </summary>
		public void EditEpisode()
		{
			if(CanEditEpisode is false)
				return;

			if(LoadSelectedEpisodeData() is false)
				return;

			SelectedEpisodeOption = EpisodeOptions
				.First(eo => eo.CartoonVoiceOverId == EditableEpisode.CartoonVoiceOver.CartoonVoiceOverId);
			ImportingEpisodeOption = EpisodeOptions.First();

			Jumpers = new BindableCollection<Jumper>(SelectedEpisodeOption.Jumpers);
			SelectedJumper = Jumpers.First();

			EditableEpisodeTime = ConvertToEpisodeTime(SelectedEpisodeOption, SelectedJumper);

			TempEpisodeSnapshot = JsonConvert.SerializeObject(EditableEpisode);
			TempEpisodeOptionSnapshot = JsonConvert.SerializeObject(SelectedEpisodeOption);

			//NotifyOfPropertyChange(() => EpisodeDuration);
			IsNotEditing = false;
			NotifyOfPropertyChange(() => CanCancelEpisodeSelection);
		}
		public bool CanEditEpisode => SelectedEpisode != null && IsNotEditing;

		/// <summary>
		/// Удалить выбранный эпизод
		/// </summary>
		public void RemoveEpisode()
		{
			if(CanRemoveEpisode is false)
				return;

			RemoveEpisodeFromDb();

			Episodes.Remove(SelectedEpisode);
			NotifyOfPropertyChange(() => Episodes);
			SelectedEpisode = Episodes.LastOrDefault();
		}



		public bool CanRemoveEpisode => SelectedEpisode != null && IsNotEditing;

		/// <summary>
		/// Снять выделение с выбранного эпизода
		/// </summary>
		public void CancelEpisodeSelection()
		{
			if(CanCancelEpisodeSelection is false)
				return;

			if(HasChangesValidation() is false)
				return;

			SelectedEpisode = null;
		}

		public bool CanCancelEpisodeSelection => IsNotEditing;

		/// <summary>
		/// Отмена редактирования (разблокировка интрефейса)
		/// </summary>
		public void CancelEditing()
		{
			if(CanCancelEditing is false)
				return;

			EditableEpisode = null;
			EpisodeOptions = new BindableCollection<EpisodeOption>();
			Jumpers = new BindableCollection<Jumper>();
			SelectedJumper = null;

			TempEpisodeSnapshot = "";
			TempEpisodeOptionSnapshot = "";

			IsNotEditing = true;
			var tempId = SelectedEpisode.CartoonEpisodeId;
			using(var ctx = new CVDbContext(AppDataPath))
			{
				var episodes = ctx.CartoonEpisodes
								  .Include(ce => ce.EpisodeVoiceOvers)
								  .Where(ce => ce.CartoonSeasonId == GlobalIdList.SeasonId);
				Episodes = new BindableCollection<CartoonEpisode>(episodes);
			}

			SelectedEpisode = Episodes.First(e => e.CartoonEpisodeId == tempId);
		}

		public bool CanCancelEditing => HasChanges is false;

		/// <summary>
		/// Переключиться на редактирование предыдущего в списке эпизода
		/// </summary>
		public void EditPreviousEpisode()
		{
			if(CanEditPreviousEpisode is false)
				return;

			CancelEditing();

			SelectedEpisode = Episodes[EpisodeIndexes.CurrentIndex - 1];
			EditEpisode();
			NotifyOfPropertyChange(() => CanCancelEpisodeSelection);
		}

		public bool CanEditPreviousEpisode => HasChanges is false &&
											  EpisodeIndexes.CurrentIndex > 0;

		/// <summary>
		/// Переключиться на редактирование следующего в списке эпизода
		/// </summary>
		public void EditNextEpisode()
		{
			if(CanEditNextEpisode is false)
				return;

			CancelEditing();

			SelectedEpisode = Episodes[EpisodeIndexes.CurrentIndex + 1];
			EditEpisode();
			NotifyOfPropertyChange(() => CanCancelEpisodeSelection);
		}

		public bool CanEditNextEpisode => HasChanges is false &&
										  EpisodeIndexes.CurrentIndex < EpisodeIndexes.EndIndex;

		#endregion

		#region Episode option editing actions

		/// <summary>
		/// Изменен выбор опции эпизода
		/// </summary>
		public void EpisodeOptionSelectionChanged()
		{
			if(SelectedEpisodeOption == null)
				return;
			Jumpers = new BindableCollection<Jumper>(SelectedEpisodeOption.Jumpers);
			SelectedJumper = Jumpers.First();
			EditableEpisodeTime = ConvertToEpisodeTime(SelectedEpisodeOption, SelectedJumper);
			TempEpisodeOptionSnapshot = JsonConvert.SerializeObject(SelectedEpisodeOption);

			NotifyOfPropertyChange(() => CanRemoveJumper);
			NotifyOfPropertyChange(() => CanResetLastDateViewed);
			NotifyOfPropertyChange(() => EpisodeDuration);
			NotifyOfPropertyChange(() => CanImportOptionData);

			NotifyEditingButtons();
		}

		public void ImportingSelectionChanged()
		{
			NotifyOfPropertyChange(() => CanImportOptionData);
		}


		/// <summary>
		/// Установить последнюю дату просмотра эпизода на сегодняшний день
		/// </summary>
		public void SetOnTodayLastDateViewed()
		{
			if(CanSetOnTodayLastDateViewed is false)
				return;

			var newDate = DateTime.Now;

			using(var ctx = new CVDbContext(AppDataPath))
			{
				ctx.EpisodeOptions
				   .First(eo => eo.EpisodeOptionId == SelectedEpisodeOption.EpisodeOptionId)
				   .LastDateViewed = newDate;
				ctx.SaveChanges();
			}

			SelectedEpisodeOption.LastDateViewed = newDate;

			var tempOption = JsonConvert.DeserializeObject<EpisodeOption>(TempEpisodeOptionSnapshot);
			tempOption.LastDateViewed = newDate;
			TempEpisodeOptionSnapshot = JsonConvert.SerializeObject(tempOption);

			NotifyOfPropertyChange(() => CanResetLastDateViewed);
			NotifyOfPropertyChange(() => SelectedEpisodeOption);
		}

		public bool CanSetOnTodayLastDateViewed => IsNotEditing is false;

		/// <summary>
		/// Сброс даты последнего просмотра эпизода
		/// </summary>
		public void ResetLastDateViewed()
		{
			if(CanResetLastDateViewed is false)
				return;

			var newDate = ResetTime;
			using(var ctx = new CVDbContext(AppDataPath))
			{
				ctx.EpisodeOptions
				   .First(eo => eo.EpisodeOptionId == SelectedEpisodeOption.EpisodeOptionId)
				   .LastDateViewed = newDate;
				ctx.SaveChanges();
			}

			SelectedEpisodeOption.LastDateViewed = newDate;

			var tempOption = JsonConvert.DeserializeObject<EpisodeOption>(TempEpisodeOptionSnapshot);
			tempOption.LastDateViewed = newDate;
			TempEpisodeOptionSnapshot = JsonConvert.SerializeObject(tempOption);

			NotifyOfPropertyChange(() => CanResetLastDateViewed);
			NotifyOfPropertyChange(() => SelectedEpisodeOption);
		}

		public bool CanResetLastDateViewed => IsNotEditing is false &&
											  SelectedEpisodeOption.LastDateViewed != ResetTime;

		/// <summary>
		/// Импорт настроек опции
		/// </summary>
		public void ImportOptionData()
		{
			if(CanImportOptionData is false)
				return;

			using(var ctx = new CVDbContext(AppDataPath))
			{
				var option = ctx.EpisodeOptions
								.Include(eo => eo.Jumpers)
								.First(eo => eo.EpisodeOptionId == SelectedEpisodeOption.EpisodeOptionId);
				if(option == null)
				{
					throw new Exception("Опция не существует");
				}

				foreach(var jumper in option.Jumpers.ToList())
				{
					ctx.Jumpers.Remove(jumper);
					ctx.SaveChanges();
				}


				Jumpers.Clear();
				SelectedEpisodeOption.Jumpers.Clear();

				var count = 1;
				foreach(var j in ImportingEpisodeOption.Jumpers)
				{
					var jumper = new Jumper
					{
						EpisodeOptionId = SelectedEpisodeOption.EpisodeOptionId,
						StartTime = j.StartTime,
						SkipCount = j.SkipCount,
						Number = count++
					};

					option.Jumpers.Add(jumper);
					ctx.SaveChanges();
					jumper = ctx.Jumpers.ToList().Last();
					Jumpers.Add(jumper);
					SelectedEpisodeOption.Jumpers.Add(jumper);
				}

				option.CreditsStart = ImportingEpisodeOption.CreditsStart;
				SelectedEpisodeOption.CreditsStart = ImportingEpisodeOption.CreditsStart;

				option.Duration = CalculatingDuration(option);
				SelectedEpisodeOption.Duration = CalculatingDuration(option);
				SelectedJumper = Jumpers.First();
				EditableEpisodeTime = ConvertToEpisodeTime(SelectedEpisodeOption, SelectedJumper);
				ctx.SaveChanges();

				TempEpisodeOptionSnapshot = JsonConvert.SerializeObject(SelectedEpisodeOption);
				NotifyOfPropertyChange(() => SelectedEpisodeOption);
				NotifyOfPropertyChange(() => Jumpers);
				NotifyOfPropertyChange(() => CanImportOptionData);
				NotifyEditingButtons();
				NotifyTimeProperties();
			}

		}

		public bool CanImportOptionData
		{
			get
			{
				if(SelectedEpisodeOption == null ||
					ImportingEpisodeOption == null ||
					SelectedEpisodeOption.EpisodeOptionId == ImportingEpisodeOption.EpisodeOptionId)
				{
					return false;
				}

				return true;
			}
		}


		#endregion

		#region Jumpers actions

		/// <summary>
		/// Изменен выбор джампера
		/// </summary>
		public void JumperSelectionChanged()
		{
			EditableEpisodeTime = ConvertToEpisodeTime(SelectedEpisodeOption, SelectedJumper);
			NotifyEditingButtons();
			//NotifyOfPropertyChange(() => CanSwapJumpers);
		}

		/// <summary>
		/// Добавить новый джампер
		/// </summary>
		public void AddJumper()
		{
			if(CanAddJumper is false)
				return;

			if(HasChangesValidation() is false)
				return;

			var count = Jumpers.Count + 1;
			var optionId = SelectedEpisodeOption.EpisodeOptionId;
			var jumper = new Jumper { EpisodeOptionId = optionId, Number = count };

			Jumpers.Add(jumper);
			SelectedEpisodeOption.Jumpers.Add(Jumpers.Last());

			AddJumperToDb(jumper);

			SelectedJumper = Jumpers.Last();
			EditableEpisodeTime = ConvertToEpisodeTime(SelectedEpisodeOption, SelectedJumper);
			TempEpisodeOptionSnapshot = JsonConvert.SerializeObject(SelectedEpisodeOption);
			NotifyOfPropertyChange(() => Jumpers);
			NotifyOfPropertyChange(() => SelectedEpisodeOption);
			NotifyOfPropertyChange(() => CanRemoveJumper);
			NotifyOfPropertyChange(() => EpisodeDuration);
			NotifyEditingButtons();
		}

		public bool CanAddJumper => IsNotEditing is false;

		/// <summary>
		/// Удалить выбранный джампер
		/// </summary>
		public void RemoveJumper()
		{
			if(CanRemoveJumper is false)
				return;

			if(HasChangesValidation() is false)
				return;

			RemoveJumperFromDb();

			SelectedJumper = Jumpers.Last();
			EditableEpisodeTime = ConvertToEpisodeTime(SelectedEpisodeOption, SelectedJumper);

			TempEpisodeSnapshot = JsonConvert.SerializeObject(EditableEpisode);
			TempEpisodeOptionSnapshot = JsonConvert.SerializeObject(SelectedEpisodeOption);

			NotifyOfPropertyChange(() => Jumpers);
			NotifyOfPropertyChange(() => SelectedEpisodeOption);
			NotifyOfPropertyChange(() => CanRemoveJumper);
			NotifyOfPropertyChange(() => EpisodeDuration);
			NotifyEditingButtons();
		}


		public bool CanRemoveJumper => Jumpers.Count > 1 &&
			IsNotEditing is false;

		///// <summary>
		///// Измене выбор обмениваемого джампера
		///// </summary>
		//public void SwappingJumperSelectionChanged()
		//{
		//	NotifyOfPropertyChange(() => CanDisableSwapping);
		//	NotifyOfPropertyChange(() => CanSwapJumpers);
		//}


		///// <summary>
		///// Включить режим обмена местами джамперов
		///// </summary>
		//public void EnableSwapping()
		//{
		//	IsJumperSwapping = true;
		//	NotifyOfPropertyChange(() => CanEnableSwapping);
		//	NotifyOfPropertyChange(() => CanDisableSwapping);

		//}

		//public bool CanEnableSwapping => IsJumperSwapping is false;

		///// <summary>
		///// Поменять местами джампер
		///// </summary>
		//public void SwapJumpers()
		//{
		//	if (CanSwapJumpers is false) return;

		//	Jumper temp;

		//	using (var ctx = new CVDbContext(AppDataPath))
		//	{
		//		var selJumper = ctx.Jumpers.Find(SelectedJumper.JumperId);
		//		var swapJumper = ctx.Jumpers.Find(SwappingJumper.JumperId);

		//		if (selJumper == null ||
		//		    swapJumper == null)
		//		{
		//			throw new Exception("Джампер не найден");
		//		}

		//		temp = CloneObject<Jumper>(selJumper);
		//		selJumper.StartTime = swapJumper.StartTime;
		//		selJumper.SkipCount = swapJumper.SkipCount;
		//		swapJumper.StartTime = temp.StartTime;
		//		swapJumper.SkipCount = temp.SkipCount;

		//		ctx.SaveChanges();
		//	}

		//	temp = CloneObject<Jumper>(SelectedJumper);
		//	SelectedJumper = CloneObject<Jumper>(SwappingJumper);
		//	SwappingJumper = CloneObject<Jumper>(temp);
		//	NotifyOfPropertyChange(() => CanSwapJumpers);
		//	NotifyOfPropertyChange(() => Jumpers);

		//}

		//public bool CanSwapJumpers
		//{
		//	get
		//	{
		//		if (SelectedJumper == null ||
		//		    SwappingJumper == null)
		//		{
		//			return false;
		//		}

		//		if (SelectedJumper.JumperId == SwappingJumper.JumperId ||
		//		    SelectedJumper.EndTime >= SwappingJumper.StartTime)
		//		{
		//			return false;
		//		}

		//		return true;
		//	}
		//}

		///// <summary>
		///// Выключить режим обмена местами джамперов
		///// </summary>
		//public void DisableSwapping()
		//{
		//	IsJumperSwapping = false;
		//	NotifyOfPropertyChange(() => CanEnableSwapping);
		//	NotifyOfPropertyChange(() => CanDisableSwapping);
		//}

		//public bool CanDisableSwapping => IsJumperSwapping;

		#endregion

		#region Changes actions

		/// <summary>
		/// Сохранение данных
		/// </summary>
		public void SaveChanges()
		{
			if(CanSaveChanges is false)
				return;

			using(var ctx = new CVDbContext(AppDataPath))
			{
				SaveEpisodeChanges(ctx);
				SaveJumpersChanges(ctx);
				SaveEpisodeOptionChanges(ctx);
			}

			TempEpisodeSnapshot = JsonConvert.SerializeObject(EditableEpisode);
			TempEpisodeOptionSnapshot = JsonConvert.SerializeObject(SelectedEpisodeOption);

			NotifyEditingProperties();
		}
		public bool CanSaveChanges => HasChanges;



		/// <summary>
		/// Отмена измененных данных
		/// </summary>
		public void CancelChanges()
		{
			if(CanCancelChanges is false)
				return;

			var dvm = new DialogViewModel(null, DialogType.CANCEL_CHANGES);
			WinMan.ShowDialog(dvm);

			if(dvm.DialogResult == DialogResult.NO_ACTION)
			{
				return;
			}

			EditableEpisode = JsonConvert.DeserializeObject<CartoonEpisode>(TempEpisodeSnapshot);
			SelectedEpisodeOption = JsonConvert.DeserializeObject<EpisodeOption>(TempEpisodeOptionSnapshot);
			var tempId = SelectedJumper.JumperId;
			Jumpers = new BindableCollection<Jumper>(SelectedEpisodeOption.Jumpers);
			SelectedJumper = Jumpers.First(j => j.JumperId == tempId);
			EditableEpisodeTime = ConvertToEpisodeTime(SelectedEpisodeOption, SelectedJumper);
			NotifyEditingProperties();
			NotifyOfPropertyChange(() => Jumpers);
			NotifyOfPropertyChange(() => SelectedJumper);
		}

		public bool CanCancelChanges => HasChanges;

		#endregion

		#region Other actions

		/// <summary>
		/// Кнопка Редактор озвучек
		/// </summary>
		public void EditVoiceOvers()
		{
			var wm = new WindowsManagerViewModel(new VoiceOversEditingViewModel(GlobalIdList));

			WinMan.ShowDialog(wm);

			UpdateVoiceOverList();
		}

		/// <summary>
		/// Проверка нажатой горячей клавиши
		/// </summary>
		/// <param name="e"></param>
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
						case Key.OemPlus:
							if(CanEditNextEpisode)
							{
								EditNextEpisode();
								return;
							}

							break;
						case Key.OemMinus:
							if(CanEditPreviousEpisode)
							{
								EditPreviousEpisode();
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
			EpisodeIndexes.CurrentIndex = Episodes.IndexOf(SelectedEpisode);
			lb.ScrollIntoView(lb.SelectedItem);
			NotifyOfPropertyChange(() => CanEditNextEpisode);
			NotifyOfPropertyChange(() => CanEditPreviousEpisode);
		}

		#endregion
	}
}
