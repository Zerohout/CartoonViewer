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
	using static Helpers.Cloner;
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
			if(EditableEpisode == null)
				return;

			var time = ConvertFromEpisodeTime(EditableEpisodeTime);

			if(SelectedJumper != null)
			{
				SelectedJumper.JumperStartTime = time.JumperTime;
				SelectedJumper.SkipCount = time.SkipCount;
			}

			EditableEpisode.CreditsStart = time.CreditsStart;

			EditableEpisodeSnapshot = JsonConvert.SerializeObject(EditableEpisode);
			SelectedJumperSnapshot = JsonConvert.SerializeObject(SelectedJumper);

			NotifyEditingProperties();
			NotifyOfPropertyChange(() => EpisodeDuration);
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
		public async void AddEpisode()
		{
			var count = _episodes.Count + 1;

			var defaultEpisode = GetDefaultEpisode(count);

			Episodes.Add(defaultEpisode);
			NotifyOfPropertyChange(() => Episodes);

			using(var ctx = new CVDbContext(AppDataPath))
			{
				ctx.CartoonEpisodes.Add(defaultEpisode);
				await ctx.SaveChangesAsync();
				Episodes.Last().CartoonEpisodeId = ctx.CartoonEpisodes
													  .ToList()
													  .Last().CartoonEpisodeId;
			}

			SelectedEpisode = Episodes.LastOrDefault();
		}

		public bool CanAddEpisode => IsNotEditing;

		/// <summary>
		/// Редактировать выбранный эпизод
		/// </summary>
		public void EditEpisode()
		{
			CartoonEpisode episode;

			using(var ctx = new CVDbContext(AppDataPath))
			{
				episode = ctx.CartoonEpisodes
							 .Include(ce => ce.Jumpers)
							 .Include(ce => ce.EpisodeVoiceOvers)
							 .Single(ce => ce.CartoonEpisodeId == GlobalIdList.EpisodeId);
			}

			EditableEpisode = CloneEpisode(episode);
			Jumpers = new BindableCollection<Jumper>(EditableEpisode.Jumpers);
			SelectedJumper = Jumpers.FirstOrDefault();
			VoiceOvers = new BindableCollection<CartoonVoiceOver>(CloneVoiceOverList(episode.EpisodeVoiceOvers));

			EditableEpisodeTime = ConvertToEpisodeTime(SelectedJumper, EditableEpisode);

			EditableEpisodeSnapshot = JsonConvert.SerializeObject(EditableEpisode);
			TempEpisodeSnapshot = JsonConvert.SerializeObject(EditableEpisode);
			SelectedJumperSnapshot = JsonConvert.SerializeObject(SelectedJumper);
			TempJumperSnapshot = JsonConvert.SerializeObject(SelectedJumper);

			NotifyOfPropertyChange(() => EpisodeDuration);



			IsNotEditing = false;
		}

		public bool CanEditEpisode => SelectedEpisode != null && IsNotEditing;

		/// <summary>
		/// Удалить выбранный эпизод
		/// </summary>
		public async void RemoveEpisode()
		{
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
			if(HasChangesValidation() is false)
				return;

			SelectedEpisode = null;
		}

		public bool CanCancelSelection => SelectedEpisode != null && IsNotEditing;

		/// <summary>
		/// Отмена редактирования (разблокировка интрефейса)
		/// </summary>
		public void CancelEditing()
		{
			EditableEpisode = null;
			TempEpisodeSnapshot = "";
			SelectedJumper = null;
			Jumpers = new BindableCollection<Jumper>();
			IsNotEditing = true;
		}

		public bool CanCancelEditing => HasChanges is false;

		#endregion

		#region Editing buttons

		public void JumperSelectionChanged()
		{
			EditableEpisodeTime = ConvertToEpisodeTime(SelectedJumper, EditableEpisode);
			SelectedJumperSnapshot = JsonConvert.SerializeObject(SelectedJumper);
			TempJumperSnapshot = JsonConvert.SerializeObject(SelectedJumper);
			NotifyEditingButtons();

		}

		public void AddJumper()
		{
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
			var episodeId = EditableEpisode.CartoonEpisodeId;
			var jumper = new Jumper { CartoonEpisodeId = episodeId, Number = count };

			Jumpers.Add(jumper);


			using(var ctx = new CVDbContext(AppDataPath))
			{
				ctx.CartoonEpisodes
				   .First(ce => ce.CartoonEpisodeId == episodeId)
				   .Jumpers.Add(jumper);
				ctx.SaveChanges();

				Jumpers.Last().JumperId = ctx.Jumpers.ToList().Last().JumperId;
				NotifyOfPropertyChange(() => Jumpers);

			}

			EditableEpisode.Jumpers.Add(Jumpers.Last());
			SelectedJumper = Jumpers.LastOrDefault();
			EditableEpisodeTime = ConvertToEpisodeTime(SelectedJumper, EditableEpisode);

			EditableEpisodeSnapshot = JsonConvert.SerializeObject(EditableEpisode);
			TempEpisodeSnapshot = JsonConvert.SerializeObject(EditableEpisode);
			//SelectedJumperSnapshot = JsonConvert.SerializeObject(SelectedJumper);
			//TempJumperSnapshot = JsonConvert.SerializeObject(SelectedJumper);
			NotifyEditingButtons();
		}

		public bool CanAddJumper => IsNotEditing is false;

		public void RemoveJumper()
		{
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
			}

			var temp = EditableEpisode.Jumpers.First(j => j.JumperId == SelectedJumper.JumperId);
			EditableEpisode.Jumpers.Remove(temp);
			Jumpers.Remove(SelectedJumper);

			NotifyOfPropertyChange(() => Jumpers);
			SelectedJumper = Jumpers.LastOrDefault();
			EditableEpisodeTime = ConvertToEpisodeTime(SelectedJumper, EditableEpisode);

			EditableEpisodeSnapshot = JsonConvert.SerializeObject(EditableEpisode);
			TempEpisodeSnapshot = JsonConvert.SerializeObject(EditableEpisode);
			//SelectedJumperSnapshot = JsonConvert.SerializeObject(SelectedJumper);
			//TempJumperSnapshot = JsonConvert.SerializeObject(SelectedJumper);
			NotifyEditingButtons();
		}

		public bool CanRemoveJumper => SelectedJumper != null &&
			IsNotEditing is false;

		/// <summary>
		/// Установить последнюю дату просмотра эпизода на сегодняшний день
		/// </summary>
		public void SetOnTodayLastDateViewed()
		{
			var newDate = DateTime.Now;

			using(var ctx = new CVDbContext(AppDataPath))
			{
				ctx.CartoonEpisodes
				   .First(ce => ce.CartoonEpisodeId ==
								EditableEpisode.CartoonEpisodeId)
				   .LastDateViewed = newDate;
				ctx.SaveChanges();
			}

			EditableEpisode.LastDateViewed = newDate;
			var editableEpisode = JsonConvert.DeserializeObject<CartoonEpisode>(EditableEpisodeSnapshot);
			var tempEpisode = JsonConvert.DeserializeObject<CartoonEpisode>(TempEpisodeSnapshot);
			editableEpisode.LastDateViewed = newDate;
			tempEpisode.LastDateViewed = newDate;
			EditableEpisodeSnapshot = JsonConvert.SerializeObject(editableEpisode);
			TempEpisodeSnapshot = JsonConvert.SerializeObject(tempEpisode);
			NotifyOfPropertyChange(() => CanResetLastDateViewed);
			NotifyOfPropertyChange(() => EditableEpisode);
		}

		public bool CanSetOnTodayLastDateViewed => IsNotEditing is false;

		/// <summary>
		/// Сброс даты последнего просмотра эпизода
		/// </summary>
		public void ResetLastDateViewed()
		{
			var newDate = ResetTime;
			using(var ctx = new CVDbContext(AppDataPath))
			{
				ctx.CartoonEpisodes
				   .First(ce => ce.CartoonEpisodeId ==
								EditableEpisode.CartoonEpisodeId)
				   .LastDateViewed = newDate;
				ctx.SaveChanges();
			}

			EditableEpisode.LastDateViewed = newDate;
			var editableEpisode = JsonConvert.DeserializeObject<CartoonEpisode>(EditableEpisodeSnapshot);
			var tempEpisode = JsonConvert.DeserializeObject<CartoonEpisode>(TempEpisodeSnapshot);
			editableEpisode.LastDateViewed = newDate;
			tempEpisode.LastDateViewed = newDate;
			EditableEpisodeSnapshot = JsonConvert.SerializeObject(editableEpisode);
			TempEpisodeSnapshot = JsonConvert.SerializeObject(tempEpisode);
			NotifyOfPropertyChange(() => CanResetLastDateViewed);
			NotifyOfPropertyChange(() => EditableEpisode);
		}

		public bool CanResetLastDateViewed => IsNotEditing is false &&
											  EditableEpisode.LastDateViewed != ResetTime;

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
			using(var ctx = new CVDbContext(AppDataPath))
			{
				var tempEpisode = JsonConvert.DeserializeObject<CartoonEpisode>(TempEpisodeSnapshot);
				var tempJumpers = tempEpisode.Jumpers;

				for(var i = 0; i < Jumpers.Count; i++)
				{
					if(IsEquals(tempJumpers[i], Jumpers[i]) is false)
					{
						ctx.Entry(Jumpers[i]).State = EntityState.Modified;
					}
				}

				ctx.SaveChanges();


				var episode = ctx.CartoonEpisodes
								 .Include(ce => ce.Jumpers)
								 .Include(c => c.EpisodeVoiceOvers)
								 .Single(c => c.CartoonEpisodeId == GlobalIdList.EpisodeId);

				episode.Name = EditableEpisode.Name;
				episode.Description = EditableEpisode.Description;
				episode.CreditsStart = EditableEpisode.CreditsStart;

				await ctx.SaveChangesAsync();
			}

			TempEpisodeSnapshot = JsonConvert.SerializeObject(EditableEpisode);
			TempJumperSnapshot = JsonConvert.SerializeObject(SelectedJumper);

			NotifyEditingButtons();
		}


		public bool CanSaveChanges => HasChanges;

		/// <summary>
		/// Отмена измененных данных
		/// </summary>
		public void CancelChanges()
		{
			var dvm = new DialogViewModel(null, DialogType.CANCEL_CHANGES);
			WinMan.ShowDialog(dvm);

			if(dvm.DialogResult == DialogResult.NO_ACTION)
			{
				return;
			}

			EditableEpisode = JsonConvert.DeserializeObject<CartoonEpisode>(TempEpisodeSnapshot);
			SelectedJumper = JsonConvert.DeserializeObject<Jumper>(TempJumperSnapshot);
			EditableEpisodeTime = ConvertToEpisodeTime(SelectedJumper, EditableEpisode);
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
