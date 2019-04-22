namespace CartoonViewer.Settings.GeneralSettingsFolder.ViewModels
{
	using System;
	using System.Windows;
	using Caliburn.Micro;
	using Models.SettingModels;
	using static Helpers.Helper;

	public partial class GeneralSettingsViewModel : Screen
	{
		//private string _defaultEpisodesStartCountString;
		//private string _intellectualShutdownIdleTimeString;
		//private string _intellectualShutdownReachedTimeString;
		//private string _noneRepeatTimeCountString;

		//private int _defaultEpisodesStartCount;
		//private bool _intellectualShutdown;
		//private int _intellectualShutdownReachedTime;
		//private int _intellectualShutdownIdleTime;
		//private bool _isIntellectualShutdownRemarkExpand;
		//private bool _watchingInRow = false;
		//private bool _randomWatching = true;
		//private string _selectedNoneRepeatTimeType = "Дней";
		//private int _noneRepeatTimeCount;


		private GeneralSettingsValue _generalValue = new GeneralSettingsValue();

		public GeneralSettingsValue GeneralValue
		{
			get => _generalValue;
			set
			{
				_generalValue = value;
				NotifyOfPropertyChange(() => GeneralValue);
			}
		}




		///// <summary>
		///// Количество времени, которое эпизод не будет повторяться
		///// </summary>
		//public int NoneRepeatTimeCount
		//{
		//	get => _noneRepeatTimeCount;
		//	set
		//	{
		//		_noneRepeatTimeCount = value;
		//		NotifyOfPropertyChange(() => NoneRepeatTimeCount);
		//	}
		//}
		
		///// <summary>
		///// Строковая интерпретация количества времени, которое эпизод не будет повторяться
		///// </summary>
		//public string NoneRepeatTimeCountString
		//{
		//	get => _noneRepeatTimeCountString;
		//	set
		//	{
		//		if (int.TryParse(value, out var result))
		//		{
		//			if (result > 999)
		//			{
		//				result = 999;
		//				value = result.ToString();
		//			}

		//			if (result < 1)
		//			{
		//				result = 1;
		//				value = result.ToString();
		//			}

		//			_noneRepeatTimeCount = result;
		//			_noneRepeatTimeCountString = value;
		//		}
		//		else
		//		{
		//			result = 1;
		//			value = result.ToString();
		//		}

		//		NotifyOfPropertyChange(() => NoneRepeatTimeCount);
		//		NotifyOfPropertyChange(() => NoneRepeatTimeCountString);
		//	}
		//}

		///// <summary>
		///// Выбранный тип времени для запрета повторов просмотренных эпизодов
		///// </summary>
		//public string SelectedNoneRepeatTimeType
		//{
		//	get => _selectedNoneRepeatTimeType;
		//	set
		//	{
		//		_selectedNoneRepeatTimeType = value;
		//		NotifyOfPropertyChange(() => SelectedNoneRepeatTimeType);
		//	}
		//}

		///// <summary>
		///// Коллекция типов времени для запрета повторов просмотренных эпизодов
		///// </summary>
		//public BindableCollection<string> NoneRepeatTimeTypeList => new BindableCollection<string>
		//{
		//	"Никогда",
		//	"Лет",
		//	"Месяцев",
		//	"Недель",
		//	"Дней",
		//	"Часов",
		//	"Всегда"
		//};

		///// <summary>
		///// Флаг просмотря эпизодов подряд
		///// </summary>
		//public bool WatchingInRow
		//{
		//	get => _watchingInRow;
		//	set
		//	{
		//		if(_watchingInRow == value) return;
		//		_watchingInRow = value;
		//		RandomWatching = !value;
		//		NotifyOfPropertyChange(() => WatchingInRow);
		//		NotifyOfPropertyChange(() => RandomWatching);
		//	}
		//}

		///// <summary>
		///// Флаг для просмотра эпизодов в случайном порядке
		///// </summary>
		//public bool RandomWatching
		//{
		//	get => _randomWatching;
		//	set
		//	{
		//		if(_randomWatching == value) return;
		//		_randomWatching = value;
		//		WatchingInRow = !value;
		//		NotifyOfPropertyChange(() => RandomWatching);
		//		NotifyOfPropertyChange(() => WatchingInRow);
		//		NotifyOfPropertyChange(() => RandomEnabledVisibility);
		//	}
		//}
		
		///// <summary>
		///// Флаг определения состояния развернутости примечаний интеллектуального выключения
		///// </summary>
		//public bool IsIntellectualShutdownRemarkExpand
		//{
		//	get => _isIntellectualShutdownRemarkExpand;
		//	set
		//	{
		//		_isIntellectualShutdownRemarkExpand = value;
		//		NotifyOfPropertyChange(() => IsIntellectualShutdownRemarkExpand);
		//	}
		//}
		
		

		///// <summary>
		///// Примерная длительность указанного количества эпизодов
		///// </summary>
		//public string ApproximateDuration
		//{
		//	get
		//	{
		//		var time = new TimeSpan(
		//			0,
		//			0,
		//			(int)Math.Ceiling(ApproximateEpisodeDuration.TotalSeconds * DefaultEpisodesStartCount));

		//		return $"~{time.Days}д {time.Hours}ч {time.Minutes}м";
		//	}
		//}

		///// <summary>
		///// Часы суток для определения работы интеллектуального выключения
		///// </summary>
		//public int IntellectualShutdownReachedTime
		//{
		//	get => _intellectualShutdownReachedTime;
		//	set
		//	{
		//		_intellectualShutdownReachedTime = value;
		//		NotifyOfPropertyChange(() => IntellectualShutdownReachedTime);
		//	}
		//}
		
		///// <summary>
		///// Строковая интерпретация часов суток для определения работы интеллектуального выключения
		///// </summary>
		//public string IntellectualShutdownReachedTimeString
		//{
		//	get => _intellectualShutdownReachedTimeString;
		//	set
		//	{
		//		if(int.TryParse(value, out var result))
		//		{
		//			if(result > 24)
		//			{
		//				result = 23;
		//				value = "23";
		//			}

		//			if(result < 0)
		//			{
		//				result = 0;
		//				value = "0";
		//			}

		//			_intellectualShutdownReachedTime = result;
		//			_intellectualShutdownReachedTimeString = value;
		//		}
		//		else
		//		{
		//			_intellectualShutdownReachedTime = 0;
		//			_intellectualShutdownReachedTimeString = "0";
		//		}

		//		NotifyOfPropertyChange(() => IntellectualShutdownReachedTimeString);
		//		NotifyOfPropertyChange(() => IntellectualShutdownReachedTime);
		//	}
		//}
		
		///// <summary>
		///// Строковая интерпретация времени ожидания, после которого начнется работа интеллектуального выключения
		///// </summary>
		//public string IntellectualShutdownIdleTimeString
		//{
		//	get => _intellectualShutdownIdleTimeString;
		//	set
		//	{
		//		if(int.TryParse(value, out var result))
		//		{
		//			if(result > 999)
		//			{
		//				result = 999;
		//				value = "999";
		//			}

		//			if(result < 0)
		//			{
		//				result = 0;
		//				value = "0";
		//			}

		//			_intellectualShutdownIdleTime = result;
		//			_intellectualShutdownIdleTimeString = value;
		//		}
		//		else
		//		{
		//			_intellectualShutdownIdleTime = 0;
		//			_intellectualShutdownIdleTimeString = "0";
		//		}


		//		NotifyOfPropertyChange(() => IntellectualShutdownIdleTimeString);
		//		NotifyOfPropertyChange(() => IntellectualShutdownIdleTime);
		//	}
		//}

		///// <summary>
		///// Время ожидания, после которого начнется работа интеллектуального выключения
		///// </summary>
		//public int IntellectualShutdownIdleTime
		//{
		//	get => _intellectualShutdownIdleTime;
		//	set
		//	{
		//		_intellectualShutdownIdleTime = value;
		//		NotifyOfPropertyChange(() => IntellectualShutdownIdleTime);
		//	}
		//}

		///// <summary>
		///// Флаг состояния интеллектуального выключения
		///// </summary>
		//public bool IntellectualShutdown
		//{
		//	get => _intellectualShutdown;
		//	set
		//	{
		//		_intellectualShutdown = value;
		//		NotifyOfPropertyChange(() => IntellectualShutdown);
		//		NotifyOfPropertyChange(() => IntelectualShutdownSettingsVisibility);
		//	}
		//}

		///// <summary>
		///// Свойство Visibility элементов зависимых от состояния включенности интеллектуального выключения
		///// </summary>
		//public Visibility IntelectualShutdownSettingsVisibility => IntellectualShutdown
		//	? Visibility.Visible
		//	: Visibility.Collapsed;
		///// <summary>
		///// Свойство Visibility элементов зависимых от состояния включенности интелектуального выключения
		///// </summary>
		//public Visibility IntellectualShutdownRemarkVisibility => IsIntellectualShutdownRemarkExpand
		//	? Visibility.Visible
		//	: Visibility.Collapsed;
		///// <summary>
		///// Свойство Visibility элементов зависимых от просмотра эпизодов в случайном порядке
		///// </summary>
		//public Visibility RandomEnabledVisibility => RandomWatching
		//	? Visibility.Visible
		//	: Visibility.Hidden;
		///// <summary>
		///// Количество эпизодов к просмотру по умолчанию
		///// </summary>
		//public int DefaultEpisodesStartCount
		//{
		//	get => _defaultEpisodesStartCount;
		//	set
		//	{
		//		_defaultEpisodesStartCount = value;
		//		NotifyOfPropertyChange(() => DefaultEpisodesStartCount);
		//	}
		//}

		///// <summary>
		///// Строковая интерпретация количества эпизодов к просмотру по умолчанию
		///// </summary>
		//public string DefaultEpisodesStartCountString
		//{
		//	get => _defaultEpisodesStartCountString;
		//	set
		//	{
		//		if(int.TryParse(value, out var result))
		//		{
		//			if(result > 999999)
		//			{
		//				result = 999999;
		//				value = "999999";
		//			}

		//			if(result < 0)
		//			{
		//				result = 0;
		//				value = "0";
		//			}

		//			_defaultEpisodesStartCount = result;
		//			_defaultEpisodesStartCountString = value;
		//		}
		//		else
		//		{
		//			_defaultEpisodesStartCount = 0;
		//			_defaultEpisodesStartCountString = "0";
		//		}

		//		NotifyOfPropertyChange(() => DefaultEpisodesStartCountString);
		//		NotifyOfPropertyChange(() => DefaultEpisodesStartCount);

		//	}
		//}

	}
}
