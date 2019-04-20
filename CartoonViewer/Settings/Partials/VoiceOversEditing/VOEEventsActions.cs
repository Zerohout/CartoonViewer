namespace CartoonViewer.Settings.ViewModels
{
	using System;
	using System.Data.Entity;
	using System.Linq;
	using System.Runtime.InteropServices.ComTypes;
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

				voiceOver.Name = EditedCartoonVoiceOver.Name;
				voiceOver.UrlParameter = EditedCartoonVoiceOver.UrlParameter;
				voiceOver.Description = EditedCartoonVoiceOver.Description;

				ctx.Entry(voiceOver).State = EntityState.Modified;
				await ctx.SaveChangesAsync();
			}

			TempEditedCartoonVoiceOver = CloneVoiceOver(EditedCartoonVoiceOver);

			NotifyChanges();
		}

		public bool CanSaveChanges => HasChanges;
		/// <summary>
		/// Отменить изменения
		/// </summary>
		public void CancelChanges()
		{
			EditedCartoonVoiceOver = CloneVoiceOver(TempEditedCartoonVoiceOver);
			NotifyChanges();
		}

		public bool CanCancelChanges => HasChanges;
		/// <summary>
		/// Выйти из редактора
		/// </summary>
		public void Exit()
		{
			TryClose();
		}
		/// <summary>
		/// Снять выделение с м/ф
		/// </summary>
		public void CancelCartoonSelection() => SelectedCartoon = null;

		public bool CanCancelCartoonSelection => SelectedCartoon != null && IsNotEditing;
		/// <summary>
		/// Добавить новую озвучку в выбранный м/ф
		/// </summary>
		public void AddCartoonVoiceOver()
		{
			CartoonVoiceOver defaultVoiceOver;
			using(var ctx = new CVDbContext())
			{
				var count = ctx.VoiceOvers.Max(vo => vo.CartoonVoiceOverId) + 1;

				defaultVoiceOver = new CartoonVoiceOver
				{
					Name = $"Озвучка_{count}",
					UrlParameter = $"param_{count}",
					Description = $"Описание озвучки_{count}"
				};

				ctx.Cartoons.Find(IdList.CartoonId).CartoonVoiceOvers.Add(defaultVoiceOver);
				ctx.SaveChanges();

				defaultVoiceOver.CartoonVoiceOverId = ctx.VoiceOvers.ToList().Last().CartoonVoiceOverId;
			}

			CartoonVoiceOvers.Add(defaultVoiceOver);
			NotifyOfPropertyChange(() => CartoonVoiceOvers);
			SelectedCartoonVoiceOver = CartoonVoiceOvers.Count > 0
				? CartoonVoiceOvers.Last()
				: null;
		}
		public bool CanAddCartoonVoiceOver => IsNotEditing;
		/// <summary>
		/// Начать редактирование выбранной озвучки м/ф
		/// </summary>
		public void EditCartoonVoiceOver()
		{
			EditedCartoonVoiceOver = CloneVoiceOver(SelectedCartoonVoiceOver);
			TempEditedCartoonVoiceOver = CloneVoiceOver(SelectedCartoonVoiceOver);
			IsNotEditing = false;
		}

		public bool CanEditCartoonVoiceOver => SelectedCartoonVoiceOver != null && IsNotEditing;
		/// <summary>
		/// Удалить выбранную озвучку из всех списков
		/// </summary>
		public async void RemoveCartoonVoiceOver()
		{
			if(SelectedVoiceOverId == 0)
			{
				throw new Exception("Id выбраной озвучки м/ф равен 0");
			}

			using(var ctx = new CVDbContext())
			{
				var voiceOver = ctx.VoiceOvers.Find(SelectedVoiceOverId);

				ctx.VoiceOvers.Remove(voiceOver);
				await ctx.SaveChangesAsync();

				if(SelectedEpisode != null)
				{
					EpisodeVoiceOvers.Remove(voiceOver);
					NotifyOfPropertyChange(() => EpisodeVoiceOvers);
				}
				CartoonVoiceOvers.Remove(voiceOver);
				NotifyOfPropertyChange(() => CartoonVoiceOvers);
				SelectedCartoonVoiceOver = CartoonVoiceOvers.Count > 0
					? CartoonVoiceOvers.Last()
					: null;
			}
		}

		public bool CanRemoveCartoonVoiceOver => SelectedCartoonVoiceOver != null && IsNotEditing;
		/// <summary>
		/// Копировать выбранную озвучку м/ф в выбранный эпизод
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

			var voiceOver = CartoonVoiceOvers.First(vo => vo.CartoonVoiceOverId == SelectedVoiceOverId);

			EpisodeVoiceOvers.Add(voiceOver);
			NotifyOfPropertyChange(() => EpisodeVoiceOvers);
			_selectedEpisodeVoiceOver = voiceOver;
			NotifyOfPropertyChange(() => SelectedEpisodeVoiceOver);
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
		/// <summary>
		/// Снять выделение с выбранной озвучки м/ф
		/// </summary>
		public void CancelCartoonVoiceOverSelection() => SelectedCartoonVoiceOver = null;

		public bool CanCancelCartoonVoiceOverSelection => SelectedCartoonVoiceOver != null && IsNotEditing;
		/// <summary>
		/// Разблокировать интерфейс и скрыть поля редактирования озвучки м/ф
		/// </summary>
		public void UnlockEditingInterface()
		{
			EditedCartoonVoiceOver = null;
			TempEditedCartoonVoiceOver = null;
			IsNotEditing = true;
		}

		public bool CanUnlockEditingInterface => !HasChanges;
		/// <summary>
		/// Снять выделение с выбранного сезона
		/// </summary>
		public void CancelSeasonSelection() => SelectedSeason = null;

		public bool CanCancelSeasonSelection => SelectedSeason != null && IsNotEditing;
		/// <summary>
		/// Снять выделение с выбранного эпизода
		/// </summary>
		public void CancelEpisodeSelection() => SelectedEpisode = null;

		public bool CanCancelEpisodeSelection => SelectedEpisode != null && IsNotEditing;
		/// <summary>
		/// Удалить озвучку из списка выбранного эпизода
		/// </summary>
		public void RemoveFromEpisodeVoiceOvers()
		{


			using(var ctx = new CVDbContext())
			{
				var episode = ctx.CartoonEpisodes
				                 .Include(ce => ce.EpisodeVoiceOvers)
				                 .Single(ce => ce.CartoonEpisodeId == IdList.EpisodeId);

				ctx.VoiceOvers
				   .Include(vo => vo.CartoonEpisodes)
				   .Single(vo => vo.CartoonVoiceOverId == SelectedVoiceOverId)
				   .CartoonEpisodes.Remove(episode);
				ctx.SaveChanges();
			}
			var voiceOver = EpisodeVoiceOvers.First(evo => evo.CartoonVoiceOverId == SelectedVoiceOverId);
			EpisodeVoiceOvers.Remove(voiceOver);
			SelectedCartoonVoiceOver = CartoonVoiceOvers
				.First(cvo => cvo.CartoonVoiceOverId == EpisodeVoiceOvers
				                                        .Last().CartoonVoiceOverId);
		}

		public bool CanRemoveFromEpisodeVoiceOvers => SelectedEpisodeVoiceOver != null && IsNotEditing;
		/// <summary>
		/// Снять выделение с выбранной озвучки эпизода
		/// </summary>
		public void CancelEpisodeVoiceOverSelection() => SelectedEpisodeVoiceOver = null;

		public bool CanCancelEpisodeVoiceOverSelection => SelectedEpisodeVoiceOver != null && IsNotEditing;



		#endregion
	}
}
