// ReSharper disable CheckNamespace
namespace CartoonViewer.Settings.CartoonEditorFolder.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.Data.Entity;
	using System.Linq;
	using System.Runtime.InteropServices.ComTypes;
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
				var voiceOver = ctx.Cartoons
									.Include(ce => ce.CartoonVoiceOvers)
									.First(c => c.CartoonId == GlobalIdList.CartoonId).CartoonVoiceOvers.First();

				var count = _episodes.Count + 1;

				// создание эпизода с активной озвучкой
				var newEpisode = new CartoonEpisode
				{
					CartoonSeasonId = GlobalIdList.SeasonId,
					CartoonId = GlobalIdList.CartoonId,
					CartoonVoiceOver = voiceOver,
					Name = $"{count}-й эпизод",
					Description = $"{count}-й эпизод",
					Number = count
				};

				ctx.CartoonEpisodes.Add(newEpisode);
				ctx.SaveChanges();
				// загрузка эпизода с ID
				newEpisode = ctx.CartoonEpisodes.ToList().Last();
				// добавление эпизода в список выбранных эпизодов озвучки
				voiceOver.CheckedEpisodes.Add(newEpisode);
				newEpisode.EpisodeVoiceOvers.Add(voiceOver);
				ctx.SaveChanges();
				var t = ctx.VoiceOvers.First(vo => vo.CartoonVoiceOverId == voiceOver.CartoonVoiceOverId);
				// создание новой опции
				var episodeOption = new EpisodeOption
				{
					CartoonEpisodeId = newEpisode.CartoonEpisodeId,
					CartoonVoiceOverId = voiceOver.CartoonVoiceOverId,
					Name = $"{voiceOver.Name}_Option"
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

				episodeOption = ctx.EpisodeOptions.ToList().Last();

				episodeOption.Duration = CalculatingDuration(episodeOption);
				ctx.SaveChanges();

				newEpisode = ctx.CartoonEpisodes.ToList().Last();

				Episodes.Add(newEpisode);
				NotifyOfPropertyChange(() => Episodes);
			}

			SelectedEpisode = Episodes.LastOrDefault();
		}

		public bool CanAddEpisode => IsNotEditing;

		/// <summary>
		/// Редактировать выбранный эпизод
		/// </summary>
		public void EditEpisode()
		{
			if(CanEditEpisode is false)
				return;
			using(var ctx = new CVDbContext(AppDataPath))
			{
				var episode = ctx.CartoonEpisodes
								 .Include(ce => ce.EpisodeVoiceOvers)
							 .First(ce => ce.CartoonEpisodeId == GlobalIdList.EpisodeId);

				if(episode.EpisodeVoiceOvers.Count == 0)
				{
					WinMan.ShowDialog(new DialogViewModel(
										  "У выбранного эпизода отсутствуют озвучки, добавьте одну или более для продолжения редактирования",
										  DialogType.INFO));
					return;
				}

				var voiceOvers = ctx.VoiceOvers
									.Include(vo => vo.CartoonEpisodes)
									.Include(vo => vo.CheckedEpisodes)
									.Where(vo => vo.CartoonEpisodes
												   .Any(ce => ce.CartoonEpisodeId == GlobalIdList.EpisodeId));

				foreach(var vo in voiceOvers)
				{
					vo.SelectedEpisodeId = episode.CartoonEpisodeId;
				}

				var options = ctx.EpisodeOptions
								 .Include(eo => eo.Jumpers)
								 .Where(eo => eo.CartoonEpisodeId == GlobalIdList.EpisodeId);

				EditableEpisode = CloneObject<CartoonEpisode>(episode);
				VoiceOvers = new BindableCollection<CartoonVoiceOver>(voiceOvers);
				EpisodeOptions = new BindableCollection<EpisodeOption>(options);
			}

			//VoiceOvers = new BindableCollection<CartoonVoiceOver>(EditableEpisode.EpisodeVoiceOvers);

			SelectedEpisodeOption = EpisodeOptions
				.First(eo => eo.CartoonVoiceOverId == EditableEpisode.CartoonVoiceOver.CartoonVoiceOverId);

			Jumpers = new BindableCollection<Jumper>(SelectedEpisodeOption.Jumpers);
			SelectedJumper = Jumpers.First();

			EditableEpisodeTime = ConvertToEpisodeTime(SelectedEpisodeOption, SelectedJumper);


			TempEpisodeSnapshot = JsonConvert.SerializeObject(EditableEpisode);
			TempEpisodeOptionSnapshot = JsonConvert.SerializeObject(SelectedEpisodeOption);

			//NotifyOfPropertyChange(() => EpisodeDuration);
			IsNotEditing = false;
		}
		public bool CanEditEpisode => SelectedEpisode != null && IsNotEditing;

		public void EpisodeOptionSelectionChanged()
		{
			if(SelectedEpisodeOption == null)
				return;
			Jumpers = new BindableCollection<Jumper>(SelectedEpisodeOption.Jumpers);
			SelectedJumper = Jumpers.First();
			EditableEpisodeTime = ConvertToEpisodeTime(SelectedEpisodeOption, SelectedJumper);
			TempEpisodeOptionSnapshot = JsonConvert.SerializeObject(SelectedEpisodeOption);
			//NotifyOfPropertyChange(() => EpisodeDuration);
			NotifyTimeProperties();
			NotifyEditingButtons();
			NotifyEditingProperties();
		}


		/// <summary>
		/// Удалить выбранный эпизод
		/// </summary>
		public async void RemoveEpisode()
		{
			if(CanRemoveEpisode is false)
				return;

			using(var ctx = new CVDbContext(AppDataPath))
			{
				var episode = ctx.CartoonEpisodes
								 .FirstOrDefault(ce => ce.CartoonEpisodeId == GlobalIdList.EpisodeId);

				if(episode == null)
				{
					throw new Exception("Удаляемый эпизод не найден");
				}

				ctx.CartoonEpisodes.Remove(episode);
				await ctx.SaveChangesAsync();
			}

			Episodes.Remove(SelectedEpisode);
			NotifyOfPropertyChange(() => Episodes);
			SelectedEpisode = Episodes.LastOrDefault();
		}

		public bool CanRemoveEpisode => SelectedEpisode != null && IsNotEditing;

		/// <summary>
		/// Снять выделение с выбранного эпизода
		/// </summary>
		public void CancelSelection()
		{
			if(CanCancelSelection is false)
				return;

			if(HasChangesValidation() is false)
				return;

			SelectedEpisode = null;
		}

		public bool CanCancelSelection => IsNotEditing;

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

		#endregion

		#region Editing buttons

		public void JumperSelectionChanged()
		{
			EditableEpisodeTime = ConvertToEpisodeTime(SelectedEpisodeOption, SelectedJumper);
			NotifyEditingButtons();
		}

		public void AddJumper()
		{
			if(CanAddJumper is false)
				return;

			if(HasChanges)
			{
				var dvm = new DialogViewModel(null, DialogType.SAVE_CHANGES);
				WinMan.ShowDialog(dvm);
				switch(dvm.DialogResult)
				{
					case DialogResult.YES_ACTION:
						SaveChanges();
						break;
					case DialogResult.NO_ACTION:
						CancelChanges();
						break;
					case DialogResult.CANCEL_ACTION:
						return;
				}
			}


			var count = Jumpers.Count + 1;
			//var episodeId = EditableEpisode.CartoonEpisodeId;
			var optionId = SelectedEpisodeOption.EpisodeOptionId;


			var jumper = new Jumper { EpisodeOptionId = optionId, Number = count };

			Jumpers.Add(jumper);
			SelectedEpisodeOption.Jumpers.Add(Jumpers.Last());

			using(var ctx = new CVDbContext(AppDataPath))
			{
				ctx.EpisodeOptions
				   .First(eo => eo.EpisodeOptionId == SelectedEpisodeOption
									.EpisodeOptionId).Duration = CalculatingDuration();
				ctx.Jumpers.Add(jumper);
				ctx.SaveChanges();

				Jumpers.Last().JumperId = ctx.Jumpers.ToList().Last().JumperId;
				NotifyOfPropertyChange(() => Jumpers);
			}


			SelectedJumper = Jumpers.Last();
			EditableEpisodeTime = ConvertToEpisodeTime(SelectedEpisodeOption, SelectedJumper);
			TempEpisodeOptionSnapshot = JsonConvert.SerializeObject(SelectedEpisodeOption);

			NotifyEditingButtons();
			NotifyOfPropertyChange(() => SelectedEpisodeOption);
			NotifyOfPropertyChange(() => CanAddJumper);
			NotifyOfPropertyChange(() => CanRemoveJumper);
			NotifyOfPropertyChange(() => EpisodeDuration);
		}

		public bool CanAddJumper => IsNotEditing is false;

		public void RemoveJumper()
		{
			if(CanRemoveJumper is false)
				return;

			if(HasChanges)
			{
				var dvm = new DialogViewModel(null, DialogType.SAVE_CHANGES);
				WinMan.ShowDialog(dvm);
				switch(dvm.DialogResult)
				{
					case DialogResult.YES_ACTION:
						SaveChanges();
						break;
					case DialogResult.NO_ACTION:
						CancelChanges();
						break;
					case DialogResult.CANCEL_ACTION:
						return;
				}
			}

			using(var ctx = new CVDbContext(AppDataPath))
			{
				var jumper = ctx.Jumpers.First(j => j.JumperId == SelectedJumper.JumperId);

				ctx.Jumpers.Remove(jumper);
				ctx.SaveChanges();

				SelectedEpisodeOption.Jumpers.Remove(SelectedJumper);
				Jumpers.Remove(SelectedJumper);

				ctx.EpisodeOptions
				   .First(eo => eo.EpisodeOptionId == SelectedEpisodeOption
									.EpisodeOptionId).Duration = CalculatingDuration();
				ctx.SaveChanges();
			}



			NotifyOfPropertyChange(() => Jumpers);

			SelectedJumper = Jumpers.Last();
			EditableEpisodeTime = ConvertToEpisodeTime(SelectedEpisodeOption, SelectedJumper);

			TempEpisodeSnapshot = JsonConvert.SerializeObject(EditableEpisode);
			TempEpisodeOptionSnapshot = JsonConvert.SerializeObject(SelectedEpisodeOption);

			NotifyOfPropertyChange(() => SelectedEpisodeOption);
			NotifyEditingButtons();
			NotifyOfPropertyChange(() => CanAddJumper);
			NotifyOfPropertyChange(() => CanRemoveJumper);
			NotifyOfPropertyChange(() => EpisodeDuration);
		}

		public bool CanRemoveJumper => Jumpers.Count > 1 &&
			IsNotEditing is false;

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
			NotifyOfPropertyChange(() => EditableEpisode);
			NotifyOfPropertyChange(() => SelectedEpisodeOption);
			NotifyEditingButtons();
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
			NotifyOfPropertyChange(() => EditableEpisode);
			NotifyOfPropertyChange(() => SelectedEpisodeOption);
			NotifyEditingButtons();
		}

		public bool CanResetLastDateViewed => IsNotEditing is false &&
											  SelectedEpisodeOption.LastDateViewed != ResetTime;

		/// <summary>
		/// Кнопка Редактор озвучек
		/// </summary>
		public void EditVoiceOvers()
		{
			var wm = new WindowsManagerViewModel(new VoiceOversEditingViewModel(GlobalIdList));

			WinMan.ShowDialog(wm);

			UpdateVoiceOverList();
		}

		#endregion

		#region Changes button

		/// <summary>
		/// Сохранение данных
		/// </summary>
		public async void SaveChanges()
		{
			if(CanSaveChanges is false)
				return;

			using(var ctx = new CVDbContext(AppDataPath))
			{
				var tempOption = JsonConvert.DeserializeObject<EpisodeOption>(TempEpisodeOptionSnapshot);
				var jumpers = ctx.Jumpers.Where(j => j.EpisodeOptionId == tempOption.EpisodeOptionId).ToList();

				for(var i = 0; i < Jumpers.Count; i++)
				{
					if(IsEquals(jumpers[i], Jumpers[i]) is false)
					{
						jumpers[i].SkipCount = Jumpers[i].SkipCount;
						jumpers[i].StartTime = Jumpers[i].StartTime;

						SelectedEpisodeOption.Jumpers[i] = Jumpers[i];

						ctx.Entry(jumpers[i]).State = EntityState.Modified;
						ctx.SaveChanges();
					}
				}

				ctx.SaveChanges();

				var episode = ctx.CartoonEpisodes
								 .Include(ce => ce.EpisodeOptions)
								 .Include(ce => ce.EpisodeOptions)
								 .Include(c => c.EpisodeVoiceOvers)
								 .First(c => c.CartoonEpisodeId == GlobalIdList.EpisodeId);

				episode.Name = EditableEpisode.Name;
				episode.Description = EditableEpisode.Description;
				await ctx.SaveChangesAsync();

				var option = ctx.EpisodeOptions
				   .Include(eo => eo.Jumpers)
				   .First(eo => eo.EpisodeOptionId == SelectedEpisodeOption.EpisodeOptionId);
				SelectedEpisodeOption.Duration = EpisodeDuration;


				if(IsEquals(option, SelectedEpisodeOption) is false)
				{
					option.Duration = SelectedEpisodeOption.Duration;
					option.CreditsStart = SelectedEpisodeOption.CreditsStart;

					//option = SelectedEpisodeOption;
					ctx.Entry(option).State = EntityState.Modified;
					ctx.SaveChanges();
				}
			}

			TempEpisodeSnapshot = JsonConvert.SerializeObject(EditableEpisode);
			TempEpisodeOptionSnapshot = JsonConvert.SerializeObject(SelectedEpisodeOption);

			NotifyEditingButtons();
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
			SelectedJumper = SelectedEpisodeOption.Jumpers.First(j => j.JumperId == SelectedJumper.JumperId);
			EditableEpisodeTime = ConvertToEpisodeTime(SelectedEpisodeOption, SelectedJumper);
			NotifyEditingButtons();
		}

		public bool CanCancelChanges => HasChanges;

		#endregion


		#region Other actions

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

		private (int CurrentIndex, int EndIndex) EpisodeIndexes;

		public void EditPreviousEpisode()
		{
			if(CanEditPreviousEpisode is false)
				return;

			CancelEditing();

			SelectedEpisode = Episodes[EpisodeIndexes.CurrentIndex - 1];
			EditEpisode();
		}

		public bool CanEditPreviousEpisode => HasChanges is false &&
											  EpisodeIndexes.CurrentIndex > 0;

		/// <summary>
		/// Выбрать следующий эпизод
		/// </summary>
		public void EditNextEpisode()
		{
			if(CanEditNextEpisode is false)
				return;

			CancelEditing();

			SelectedEpisode = Episodes[EpisodeIndexes.CurrentIndex + 1];
			EditEpisode();
		}

		public bool CanEditNextEpisode => HasChanges is false &&
										EpisodeIndexes.CurrentIndex < EpisodeIndexes.EndIndex;

		/// <summary>
		/// Действие при изменении выбора сезона
		/// </summary>
		public void SelectionChanged(ListBox lb)
		{
			EpisodeIndexes.CurrentIndex = Episodes.IndexOf(SelectedEpisode);
			lb.ScrollIntoView(lb.SelectedItem);
		}

		#endregion
	}
}
