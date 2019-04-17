namespace CartoonViewer.Settings.ViewModels
{
	using System.Data.Entity;
	using System.Linq;
	using Caliburn.Micro;
	using CartoonViewer.ViewModels;
	using Database;
	using Models.CartoonModels;
	using static Helpers.Helper;

	public partial class CartoonsControlViewModel : Conductor<Screen>.Collection.OneActive
	{
		#region Public methods

		/// <summary>
		/// Изменить выбранный сезон (проверка на несохраненные изменения)
		/// </summary>
		/// <param name="id"></param>
		public void ChangeSelectedSeason(int id)
		{
			if(((CartoonsEditingViewModel)ActiveItem).HasChanges)
			{
				var result = WinMan.ShowDialog(new DialogViewModel("Сохранить ваши изменения?", DialogState.YES_NO_CANCEL));

				if(result == true)
				{
					((CartoonsEditingViewModel)ActiveItem).SaveChanges();
				}
				else if(result == false)
				{
					var repeatResult = WinMan.ShowDialog(
						new DialogViewModel("Ваши изменения не будут сохранены. Вы точно хотите продолжить?", DialogState.YES_NO));
					if(repeatResult == false || repeatResult == null)
					{
						return;
					}
				}
				else
				{
					return;
				}
			}

			GlobalIdList.SeasonId = id;
			SelectedSeason = Seasons.First(s => s.CartoonSeasonId == id);
		}



		#endregion

		#region Private methods

		/// <summary>
		/// Загрузка из БД списков элементов (Сайты, м/ф, сезоны)
		/// </summary>
		private void LoadListData()
		{
			if(SelectedWebSite == null)
			{
				LoadWebSitesAsync();
				return;
			}

			if(SelectedCartoon == null)
			{
				LoadCartoonsAsync();
				return;
			}


			LoadSeasonsAsync();

		}

		/// <summary>
		/// Загрузка из БД списка сайтов
		/// </summary>
		private async void LoadWebSitesAsync()
		{
			BindableCollection<CartoonWebSite> webSites;

			using(var ctx = new CVDbContext())
			{
				await ctx.CartoonWebSites.LoadAsync();
				webSites = new BindableCollection<CartoonWebSite>(ctx.CartoonWebSites.Local);
			}

			WebSites = new BindableCollection<CartoonWebSite>(webSites);
		}
		/// <summary>
		/// Загрузка из БД списка мультфильмов
		/// </summary>
		private async void LoadCartoonsAsync()
		{
			BindableCollection<Cartoon> cartoons;

			using(var ctx = new CVDbContext())
			{
				await ctx.Cartoons
						 .Where(c => c.CartoonWebSites
									  .Any(cws => cws.CartoonWebSiteId == GlobalIdList.WebSiteId))
						 .LoadAsync();
				cartoons = new BindableCollection<Cartoon>(ctx.Cartoons.Local);
			}

			Cartoons = new BindableCollection<Cartoon>(cartoons) { new Cartoon { Name = NewElementString } };
		}
		/// <summary>
		/// Загрузка из БД списка сезонов
		/// </summary>
		private async void LoadSeasonsAsync()
		{
			BindableCollection<CartoonSeason> seasons;

			using(var ctx = new CVDbContext())
			{
				await ctx.CartoonSeasons
						 .Where(cs => cs.CartoonId == GlobalIdList.CartoonId)
						 .LoadAsync();
				seasons = new BindableCollection<CartoonSeason>(ctx.CartoonSeasons.Local);
			}

			Seasons = new BindableCollection<CartoonSeason>(seasons);
		}
		/// <summary>
		/// Сменить активную VM с проверкой на изменения
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		private bool ChangeActiveItem(Screen viewModel)
		{
			if(((ISettingsViewModel)ActiveItem)?.HasChanges ?? false)
			{
				var result = WinMan.ShowDialog(new DialogViewModel(
												   message: "Сохранить ваши изменения?",
												   currentState: DialogState.YES_NO_CANCEL));

				switch(result)
				{
					case true:
						((ISettingsViewModel)ActiveItem).SaveChanges();
						return true;
					case false:
						ActiveItem.TryClose();
						return true;
					case null:
						return false;
				}
			}
			else
			{
				ActiveItem?.TryClose();

				if(viewModel == null)
					return true;

				ActiveItem = viewModel;
			}

			return true;
		}

		#endregion
	}
}
