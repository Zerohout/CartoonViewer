// ReSharper disable CheckNamespace
namespace CartoonViewer.Settings.CartoonEditorFolder.ViewModels
{
	using System.Windows;
	using Caliburn.Micro;
	using Models.CartoonModels;

	public partial class CartoonsEditorViewModel : Conductor<Screen>.Collection.OneActive
	{
		private object _selectedWebSite;
		private object _selectedCartoon;
		private object _selectedSeason;
		private BindableCollection<CartoonWebSite> _webSites = new BindableCollection<CartoonWebSite>();
		private BindableCollection<Cartoon> _cartoons = new BindableCollection<Cartoon>();
		private BindableCollection<CartoonSeason> _seasons = new BindableCollection<CartoonSeason>();

		
		/// <summary>
		/// Для корректного отображения элементов в конструкторе XAML
		/// Без свойство, конструктор обращается к бд, чем вызывает серьезное зависание
		/// при его открытии и приводит к возникновению ошибки обращения к бд
		/// </summary>
		public bool IsDesignTime { get; set; }

		/// <summary>
		/// Выбранный сайт
		/// </summary>
		public CartoonWebSite SelectedWebSite
		{
			get => _selectedWebSite as CartoonWebSite;
			set
			{
				if(IsDesignTime)
				{
					_selectedWebSite = value;
					return;
				}
				ChangePropertyValue(ref _selectedWebSite, value);
			}
		}

		/// <summary>
		/// Выбранный м/ф
		/// </summary>
		public Cartoon SelectedCartoon
		{
			get => _selectedCartoon as Cartoon;
			set
			{
				if(IsDesignTime)
				{
					_selectedCartoon = value;
					return;
				}
				ChangePropertyValue(ref _selectedCartoon, value);
			}
		}


		/// <summary>
		/// Выбранный сезон
		/// </summary>
		public CartoonSeason SelectedSeason
		{
			get => _selectedSeason as CartoonSeason;
			set
			{
				if(IsDesignTime)
				{
					_selectedSeason = value;
					return;
				}
				ChangePropertyValue(ref _selectedSeason, value);
			}
		}

		/// <summary>
		/// Список доступных сайтов
		/// </summary>
		public BindableCollection<CartoonWebSite> WebSites
		{
			get => _webSites;
			set
			{
				_webSites = value;
				NotifyOfPropertyChange(() => WebSites);
			}
		}
		/// <summary>
		/// Список м/ф выбранного сайта
		/// </summary>
		public BindableCollection<Cartoon> Cartoons
		{
			get => _cartoons;
			set
			{
				_cartoons = value;
				NotifyOfPropertyChange(() => Cartoons);
			}
		}

		/// <summary>
		/// Список сезонов выбранного м/ф
		/// </summary>
		public BindableCollection<CartoonSeason> Seasons
		{
			get => _seasons;
			set
			{
				_seasons = value;
				NotifyOfPropertyChange(() => Seasons);
			}
		}

		/// <summary>
		/// Видимость списка м/ф
		/// </summary>
		public Visibility CartoonsVisibility =>
			SelectedWebSite == null
				? Visibility.Hidden
				: Visibility.Visible;
		/// <summary>
		/// Видимость VM редактирования м/ф и списка сезонов выбраного м/ф
		/// </summary>
		public Visibility CartoonEditingAndSeasonsVisibility =>
			SelectedCartoon == null
				? Visibility.Hidden
				: Visibility.Visible;
		
	}
}
