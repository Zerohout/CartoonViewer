// ReSharper disable CheckNamespace
namespace CartoonViewer.Settings.ViewingsSettingsFolder.ViewModels
{
	using System.Data.Entity;
	using System.Linq;
	using System.Threading.Tasks;
	using Caliburn.Micro;
	using Database;
	using Models.CartoonModels;
	using static Helpers.Helper;

	public partial class ViewingsSettingsViewModel : Screen
	{

		/// <summary>
		/// Загрузка данных (начальная и при смене значений объектов)
		/// </summary>
		private async void LoadData()
		{
			if(_selectedCartoon == null)
			{
				await LoadCartoonData();
				return;
			}

			if(_selectedSeason == null)
			{
				await LoadSeasonData();
				return;
			}

			if(_selectedEpisode == null)
			{
				await LoadEpisodeData();
				return;
			}

			if(_selectedVoiceOver != null)
				return;

			await LoadVoiceOverData();

		}

		/// <summary>
		/// Загрузка м/с и связанных с ним данных
		/// </summary>
		private Task LoadCartoonData()
		{
			if(IdList.CartoonId == 0)
			{
				// Начальная загрузка списка м/с при загрузке VM
				LoadCartoonList();
				return Task.CompletedTask;
			}

			// Установка значения выбранного м/с с null на notNull
			_selectedCartoon = _cartoons.First(c => c.CartoonId == IdList.CartoonId);
			NotifyCartoonData();

			// Загрузка списка сезонов выбранного м/с
			LoadSeasonList();
			return Task.CompletedTask;
		}

		/// <summary>
		/// Загрузка сезона и связанных с нима данных
		/// </summary>
		private Task LoadSeasonData()
		{
			if(_selectedCartoon.CartoonId != IdList.CartoonId)
			{
				// Смена значения м/с с notNull на notNull
				// при условии, что оно не равно текущему
				_selectedCartoon = _cartoons.First(c => c.CartoonId == IdList.CartoonId);
				NotifyCartoonData();
			}

			if(IdList.SeasonId == 0)
			{
				// Загрузка списка сезонов выбранного м/с
				LoadSeasonList();
				return Task.CompletedTask;
			}

			// Установка значения выбранного сезона с null на notNull

			_selectedSeason = _seasons.First(s => s.CartoonSeasonId == IdList.SeasonId);

			NotifySeasonData();

			LoadEpisodeList();

			return Task.CompletedTask;
		}

		/// <summary>
		/// Загрузка эпизода и связанных с ним данных
		/// </summary>
		private Task LoadEpisodeData()
		{
			if(_selectedSeason.CartoonSeasonId != IdList.SeasonId)
			{
				// Смена значения сезона с notNull на notNull
				// при условии, что оно не равно текущему
				_selectedSeason = _seasons.First(s => s.CartoonSeasonId == IdList.SeasonId);

				NotifySeasonData();
			}

			if(IdList.EpisodeId == 0)
			{
				// Загрузка списка эпизодов выбранного сезона
				LoadEpisodeList();
				return Task.CompletedTask;
			}

			// Установка значения выбранного эпизода с null на notNull
			_selectedEpisode = _episodes.First(e => e.CartoonEpisodeId == IdList.EpisodeId);
			NotifyEpisodeData();

			LoadEpisodeVoiceOverList();
			return Task.CompletedTask;
		}

		private Task LoadVoiceOverData()
		{
			if(_selectedEpisode.CartoonEpisodeId != IdList.EpisodeId)
			{
				// Смена значения эпизода с notNull на notNull
				// при условии, что оно не равно текущему
				_selectedEpisode = _episodes.First(e => e.CartoonEpisodeId == IdList.EpisodeId);
				NotifyEpisodeData();
			}

			if(IdList.VoiceOverId == 0)
			{
				// Загрузка списка озвучек выбранного эпизода
				LoadEpisodeVoiceOverList();
				return Task.CompletedTask;
			}

			// Установка значения выбранной озвучки с null на notNull
			_selectedVoiceOver = _voiceOvers.First(vo => vo.CartoonVoiceOverId == IdList.VoiceOverId);
			NotifyVoiceOverData();
			return Task.CompletedTask;
		}

		/// <summary>
		/// Загрузка списка м/с из БД
		/// </summary>
		public async void LoadCartoonList()
		{
			//using(var ctx = new CVDbContext(Helpers.Helper.AppDataPath))
			//{
			//	Cartoons = new BindableCollection<Cartoon>(await ctx.Cartoons.ToListAsync());
			//}

			Cartoons = new BindableCollection<Cartoon>(await CvDbContext.Cartoons.ToListAsync());
		}

		/// <summary>
		/// Загрузка списка сезонов выбранного м/с из БД
		/// </summary>
		public async void LoadSeasonList()
		{
			//using(var ctx = new CVDbContext(Helpers.Helper.AppDataPath))
			//{
			//	Seasons = new BindableCollection<CartoonSeason>(
			//		await ctx.CartoonSeasons
			//				 .Where(cs => cs.CartoonId == IdList.CartoonId)
			//				 .ToListAsync());
			//}

			Seasons = new BindableCollection<CartoonSeason>(
				await CvDbContext.CartoonSeasons
				                 .Where(cs => cs.CartoonId == IdList.CartoonId)
							 .ToListAsync());
		}

		/// <summary>
		/// Загрузка списка эпизодов выбранного сезона из БД
		/// </summary>
		public async void LoadEpisodeList()
		{
			//using(var ctx = new CVDbContext(Helpers.Helper.AppDataPath))
			//{
			//	Episodes = new BindableCollection<CartoonEpisode>(
			//		await ctx.CartoonEpisodes
			//				 .Where(ce => ce.CartoonSeasonId == IdList.SeasonId)
			//				 .ToListAsync());
			//}

			Episodes = new BindableCollection<CartoonEpisode>(
				await CvDbContext.CartoonEpisodes
				         .Where(ce => ce.CartoonSeasonId == IdList.SeasonId)
				         .ToListAsync());
		}

		/// <summary>
		/// Загрузка списка озвучек выбранного эпизода из БД
		/// </summary>
		public async void LoadEpisodeVoiceOverList()
		{
			BindableCollection<CartoonVoiceOver> voiceOvers;

			//using(var ctx = new CVDbContext(Helpers.Helper.AppDataPath))
			//{
			//	voiceOvers = new BindableCollection<CartoonVoiceOver>(
			//		await ctx.VoiceOvers
			//		         .Include(vo => vo.CartoonEpisodes)
			//				 .Include(vo => vo.CheckedEpisodes)
			//				 .Where(vo => vo.CartoonEpisodes
			//								.Any(ce => ce.CartoonEpisodeId == IdList.EpisodeId))
			//				 .ToListAsync());
			//}

			voiceOvers = new BindableCollection<CartoonVoiceOver>(
				await CvDbContext.VoiceOvers
				         .Include(vo => vo.CartoonEpisodes)
				         .Include(vo => vo.CheckedEpisodes)
				         .Where(vo => vo.CartoonEpisodes
				                        .Any(ce => ce.CartoonEpisodeId == IdList.EpisodeId))
				         .ToListAsync());

			var totalCount = voiceOvers.Count;
			var count = 0;

			while(count < totalCount)
			{
				voiceOvers[count++].SelectedEpisodeId = IdList.EpisodeId;
			}

			VoiceOvers = new BindableCollection<CartoonVoiceOver>(voiceOvers);
		}

		/// <summary>
		/// Изменить выбранный м/ф и все связанные данные
		/// </summary>
		/// <param name="value">Конечное значение м/ф</param>
		private void ChangeSelectedCartoon(Cartoon value)
		{
			if(IsDesignTime)
			{
				_selectedCartoon = value;
				NotifyOfPropertyChange(() => SelectedCartoon);
				return;
			}

			if(_selectedCartoon == value)
				return;

			IdList.CartoonId = value?.CartoonId ?? 0;
			ChangeSelectedSeason(null);


			if(value == null)
			{
				_selectedCartoon = null;
				NotifyCartoonData();
			}
			else
			{

				LoadData();

			}
		}

		/// <summary>
		/// Изменить выбранный сезон и все связные данные
		/// </summary>
		/// <param name="value">Конечное значение сезона</param>
		private void ChangeSelectedSeason(CartoonSeason value)
		{
			if(IsDesignTime)
			{
				_selectedSeason = value;
				NotifyOfPropertyChange(() => SelectedSeason);
				return;
			}

			if(_selectedSeason == value)
				return;

			IdList.SeasonId = value?.CartoonSeasonId ?? 0;
			ChangeSelectedEpisode(null);

			if(value == null)
			{
				_selectedSeason = null;
				NotifySeasonData();
			}
			else
			{
				LoadData();
			}
		}

		/// <summary>
		/// Изменить выбранный эпизод и все связанные данные
		/// </summary>
		/// <param name="value">Конечное значение эпизода</param>
		private void ChangeSelectedEpisode(CartoonEpisode value)
		{
			if(IsDesignTime)
			{
				_selectedEpisode = value;
				NotifyOfPropertyChange(() => SelectedEpisode);
				return;
			}

			if(_selectedEpisode == value)
				return;

			IdList.EpisodeId = value?.CartoonEpisodeId ?? 0;
			ChangeSelectedVoiceOver(null);

			if(value == null)
			{
				_selectedEpisode = null;
				NotifyEpisodeData();
			}
			else
			{
				LoadData();
			}
		}

		private void ChangeSelectedVoiceOver(CartoonVoiceOver value)
		{
			if(IsDesignTime)
			{
				_selectedVoiceOver = value;
				NotifyOfPropertyChange(() => SelectedEpisode);
				return;
			}

			if(_selectedVoiceOver == value)
				return;

			IdList.VoiceOverId = value?.CartoonVoiceOverId ?? 0;

			if(value == null)
			{
				_selectedVoiceOver = null;
				NotifyVoiceOverData();
			}
			else
			{
				LoadData();
			}
		}

		private void NotifyCartoonData()
		{
			NotifyOfPropertyChange(() => SelectedCartoon);
			NotifyOfPropertyChange(() => SelectedCartoonVisibility);
			NotifyOfPropertyChange(() => CanCancelCartoonSelection);
		}

		private void NotifySeasonData()
		{
			NotifyOfPropertyChange(() => SelectedSeason);
			NotifyOfPropertyChange(() => SelectedSeasonVisibility);
			NotifyOfPropertyChange(() => CanCancelSeasonSelection);
		}

		private void NotifyEpisodeData()
		{
			NotifyOfPropertyChange(() => SelectedEpisode);
			NotifyOfPropertyChange(() => SelectedEpisodeVisibility);
			NotifyOfPropertyChange(() => CanCancelEpisodeSelection);
		}

		private void NotifyVoiceOverData()
		{
			NotifyOfPropertyChange(() => SelectedVoiceOver);
			NotifyOfPropertyChange(() => CanCancelVoiceOverSelection);
		}
	}
}
