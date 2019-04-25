// ReSharper disable CheckNamespace
namespace CartoonViewer.Settings.CartoonEditorFolder.ViewModels
{
	using System.Data.Entity;
	using System.Linq;
	using System.Windows;
	using System.Windows.Threading;
	using Caliburn.Micro;
	using CartoonViewer.ViewModels;
	using Database;
	using Models.CartoonModels;
	using static Helpers.Helper;
	using Action = System.Action;

	public partial class CartoonsEditorViewModel : Conductor<Screen>.Collection.OneActive
	{
		#region Private methods

		/// <summary>
		/// Загрузка из БД списков элементов (Сайты, м/ф, сезоны)
		/// </summary>
		private void LoadList()
		{
			if(SelectedWebSite == null)
			{
				LoadWebSiteList();
				return;
			}

			if(SelectedCartoon == null)
			{
				LoadCartoonList();
				return;
			}

			if(SelectedSeason == null)
			{
				LoadSeasonList();
			}

		}

		private void UnloadData()
		{
			if(_selectedSeason == null)
			{
				ChangeActiveItem(_selectedCartoon != null
									 ? new CartoonsEditingViewModel(SelectedCartoon)
									 : null);
			}

			if(_selectedCartoon == null)
			{
				SelectedSeason = null;
				ChangeActiveItem(null);
				Seasons = new BindableCollection<CartoonSeason>();
			}

			if(_selectedWebSite == null)
			{
				SelectedCartoon = null;
				Cartoons = new BindableCollection<Cartoon>();
			}
		}

		/// <summary>
		/// Загрузка из БД списка сайтов
		/// </summary>
		private async void LoadWebSiteList()
		{
			BindableCollection<CartoonWebSite> webSites;

			using(var ctx = new CVDbContext(AppDataPath))
			{
				await ctx.CartoonWebSites.LoadAsync();
				webSites = new BindableCollection<CartoonWebSite>(ctx.CartoonWebSites.Local);
			}

			WebSites = new BindableCollection<CartoonWebSite>(webSites);
		}
		/// <summary>
		/// Загрузка из БД списка мультсериалов
		/// </summary>
		private async void LoadCartoonList()
		{
			BindableCollection<Cartoon> cartoons;

			using(var ctx = new CVDbContext(AppDataPath))
			{
				await ctx.Cartoons
						 .Where(c => c.CartoonWebSites
									  .Any(cws => cws.CartoonWebSiteId == GlobalIdList.WebSiteId))
						 .LoadAsync();
				cartoons = new BindableCollection<Cartoon>(ctx.Cartoons.Local);
			}

			Cartoons = new BindableCollection<Cartoon>(cartoons)
			{
				new Cartoon
				{
					Name = NewElementString
				}
			};
		}
		/// <summary>
		/// Загрузка из БД списка сезонов
		/// </summary>
		private async void LoadSeasonList()
		{
			BindableCollection<CartoonSeason> seasons;

			using(var ctx = new CVDbContext(AppDataPath))
			{
				await ctx.CartoonSeasons
						 .Where(cs => cs.CartoonId == GlobalIdList.CartoonId)
						 .LoadAsync();
				seasons = new BindableCollection<CartoonSeason>(ctx.CartoonSeasons.Local);
			}

			Seasons = new BindableCollection<CartoonSeason>(seasons);
		}

		#region Change property value

		/// <summary>
		/// Изменение значения свойства с учетом отмены выбора
		/// </summary>
		/// <param name="element">Исходное значение объекта</param>
		/// <param name="value">Конечное значение объекта</param>
		private void ChangePropertyValue(ref object element, object value)
		{
			var (identifier, oldValue, hasChanges) = SetStartingValues(element, value);

			if(hasChanges is false)
				return;

			if(((ISettingsViewModel)ActiveItem)?.HasChanges ?? false)
			{
				var vm = new DialogViewModel(null, DialogType.SAVE_CHANGES);
				_ = WinMan.ShowDialog(vm);

				switch(vm.DialogResult)
				{
					case DialogResult.YES_ACTION:
						((ISettingsViewModel)ActiveItem).SaveChanges();
						break;
					case DialogResult.NO_ACTION:
						break;
					case DialogResult.CANCEL_ACTION:
						Application.Current.Dispatcher.BeginInvoke(
							new Action(() => SetOldValue(identifier, oldValue)),
							DispatcherPriority.ContextIdle, null);
						return;
				}
			}

			NotifyChangedProperties(identifier, value);
		}

		/// <summary>
		/// Сменить активную VM с проверкой на изменения
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		private void ChangeActiveItem(Screen viewModel)
		{
			ActiveItem?.TryClose();

			if(viewModel == null)
				return;

			ActiveItem = viewModel;
		}

		/// <summary>
		/// Установить начальные значения или отменить, если значение не изменилось
		/// </summary>
		/// <param name="element">Исходное значение объекта</param>
		/// <param name="value">Конечное значение объекта</param>
		/// <returns></returns>
		private (object Identifier, object OldValue, bool HasChanges) SetStartingValues(object element, object value)
		{
			var hasChanges = value != element;

			if(hasChanges is false)
				return (null, null, false);

			var identifier = element ?? value;

			var oldValue = element;

			switch(identifier)
			{
				case CartoonWebSite _:
					_selectedWebSite = value as CartoonWebSite;
					break;
				case Cartoon _:
					_selectedCartoon = value as Cartoon;
					break;
				case CartoonSeason _:
					_selectedSeason = value as CartoonSeason;
					break;
			}

			return (identifier, oldValue, hasChanges);
		}

		/// <summary>
		/// Установить старое значение (при отмене действия)
		/// </summary>
		/// <param name="identifier">Идентификатор класса объекта</param>
		/// <param name="oldValue">Старое значение объекта</param>
		private void SetOldValue(object identifier, object oldValue)
		{
			switch(identifier)
			{
				case CartoonWebSite _:
					Set(
						ref _selectedWebSite,
						oldValue as CartoonWebSite,
						nameof(SelectedWebSite));
					NotifyOfPropertyChange(() => CanCancelWebSiteSelection);
					return;
				case Cartoon _:
					Set(
						ref _selectedCartoon,
						oldValue as Cartoon,
						nameof(SelectedCartoon));
					NotifyOfPropertyChange(() => CanCancelCartoonSelection);
					return;
				case CartoonSeason _:
					Set(
						ref _selectedSeason,
						oldValue as CartoonSeason,
						nameof(SelectedSeason));
					NotifyOfPropertyChange(() => CanCancelSeasonSelection);
					return;
			}
		}
		/// <summary>
		/// Уведомить свойства об изменении их значения
		/// </summary>
		/// <param name="identifier">Идентификатор класса объекта</param>
		/// <param name="value">Конечное значение свойства</param>
		private void NotifyChangedProperties(object identifier, object value)
		{
			switch(identifier)
			{
				case CartoonWebSite _:
					if(value != null)
					{
						GlobalIdList.WebSiteId = ((CartoonWebSite)value).CartoonWebSiteId;
						if(_selectedCartoon != null)
						{
							SelectedCartoon = null;
						}
						LoadList();
					}
					else
					{
						UnloadData();

					}
					NotifyOfPropertyChange(() => CanCancelWebSiteSelection);
					NotifyOfPropertyChange(() => SelectedWebSite);
					NotifyOfPropertyChange(() => CartoonsVisibility);
					break;
				case Cartoon _:
					if(value != null)
					{
						GlobalIdList.CartoonId = ((Cartoon)value).CartoonId;
						if(_selectedSeason != null)
						{
							SelectedSeason = null;
						}
						LoadList();
						ChangeActiveItem(
							new CartoonsEditingViewModel(SelectedCartoon)
							{
								DisplayName = "Редактирование мультсериала"
							});

					}
					else
					{
						UnloadData();

					}
					NotifyOfPropertyChange(() => CanCancelCartoonSelection);
					NotifyOfPropertyChange(() => SelectedCartoon);
					NotifyOfPropertyChange(() => CartoonEditingAndSeasonsVisibility);

					break;
				case CartoonSeason _:
					if(value != null)
					{
						GlobalIdList.SeasonId = ((CartoonSeason)value).CartoonSeasonId;
						ChangeActiveItem(new EpisodesEditingViewModel()
						{
							DisplayName = "Редактирование сезона"
						});
					}
					else
					{
						UnloadData();

					}
					NotifyOfPropertyChange(() => CanCancelSeasonSelection);
					NotifyOfPropertyChange(() => SelectedSeason);
					break;
			}
		}

		#endregion

		#endregion
	}
}
