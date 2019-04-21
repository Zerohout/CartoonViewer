namespace CartoonViewer.Settings.ViewModels
{
	using System;
	using System.Data.Entity;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Windows.Controls;
	using Caliburn.Micro;
	using Database;
	using Models.CartoonModels;
	using static Helpers.Cloner;

	public partial class VoiceOversEditingViewModel : Screen
	{
		#region EventsActions
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
		public void CancelCartoonSelection() => SelectedCartoon = null;

		public bool CanCancelCartoonSelection => SelectedCartoon != null && IsNotEditing;

		/// <summary>
		/// Снять выделение с текущего сезона
		/// </summary>
		public void CancelSeasonSelection() => SelectedSeason = null;

		public bool CanCancelSeasonSelection => SelectedSeason != null && IsNotEditing;

		/// <summary>
		/// Снять выделение с текущего эпизода
		/// </summary>
		public void CancelEpisodeSelection() => SelectedEpisode = null;

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
				if (EditedVoiceOver == null)
				{
					return false;
				}

				if(HasChanges)
				{
					return false;
				}

				if(string.IsNullOrEmpty(EditedVoiceOver.Name) ||
				   string.IsNullOrEmpty(EditedVoiceOver.UrlParameter) ||
				   string.IsNullOrEmpty(EditedVoiceOver.Description))
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
			if(SelectedVoiceOverId == 0)
			{
				throw new Exception("Id выбраной озвучки м/ф равен 0");
			}

			RemoveSelectedGlobalVoiceOverFromDb();

			//RemoveVoiceOverFromLists();
			var tempId = SelectedVoiceOverId;
			if(SelectedEpisode != null)
			{
				if(EpisodeVoiceOvers.Any(evo => evo.CartoonVoiceOverId == SelectedVoiceOverId))
				{
					RemoveVoiceOverFromEpisodeList();
				}
			}

			SelectedVoiceOverId = tempId;

			if(SelectedCartoon != null)
			{
				if(CartoonVoiceOvers.Any(cvo => cvo.CartoonVoiceOverId == SelectedVoiceOverId))
				{
					RemoveVoiceOverFromCartoonList();
				}
			}

			SelectedVoiceOverId = tempId;

			RemoveVoiceOverFromGlobalList();
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

			using(var ctx = new CVDbContext())
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
			if(SelectedVoiceOverId == 0)
			{
				throw new Exception("Id выбраной озвучки м/ф равен 0");
			}

			RemoveSelectedEpisodeVoiceOverFromDb();
			RemoveSelectedCartoonVoiceOverFromDb();
			var tempId = SelectedVoiceOverId;

			if(SelectedEpisode != null)
			{
				if(EpisodeVoiceOvers.Any(evo => evo.CartoonVoiceOverId == SelectedVoiceOverId))
				{
					RemoveVoiceOverFromEpisodeList();
				}
			}

			SelectedVoiceOverId = tempId;
			RemoveVoiceOverFromCartoonList();

			SelectedGlobalVoiceOver = GlobalVoiceOvers
				.First(gvo => gvo.CartoonVoiceOverId == CartoonVoiceOvers
														.Last().CartoonVoiceOverId);
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

			using(var ctx = new CVDbContext())
			{
				var episode = ctx.CartoonEpisodes
								 .Include(ce => ce.EpisodeVoiceOvers)
								 .Single(ce => ce.CartoonEpisodeId == IdList.EpisodeId);

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
			RemoveSelectedEpisodeVoiceOverFromDb();
			RemoveVoiceOverFromEpisodeList();

			SelectedGlobalVoiceOver = GlobalVoiceOvers
				.First(gvo => gvo.CartoonVoiceOverId == EpisodeVoiceOvers
														.Last().CartoonVoiceOverId);
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

			using(var ctx = new CVDbContext())
			{
				var voiceOver = ctx.VoiceOvers.Find(SelectedVoiceOverId);

				CopyChanges(ref voiceOver, EditedVoiceOver);

				ctx.Entry(voiceOver).State = EntityState.Modified;
				await ctx.SaveChangesAsync();
			}

			TempEditedVoiceOver = CloneVoiceOver(EditedVoiceOver);
			
			NotifyChanges();
			IsNotEditing = true;
			var tempId = SelectedVoiceOverId;

			var tempValues = IdList;

			CancelCartoonSelection();
			IdList = tempValues;
			LoadGlobalVoiceOverList();
			LoadData();
			SelectedGlobalVoiceOver = GlobalVoiceOvers
				.First(gvo => gvo.CartoonVoiceOverId == tempId);
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
			TryClose();
		}
		
		#endregion
	}
}
