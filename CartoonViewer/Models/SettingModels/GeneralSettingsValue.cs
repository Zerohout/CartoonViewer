namespace CartoonViewer.Models.SettingModels
{
	using System;
	using System.Windows;
	using Caliburn.Micro;
	using static Helpers.Helper;

	public class GeneralSettingsValue : PropertyChangedBase
	{
		private int? _defaultStartEpisodesCount = 68;
		private bool _intellectualShutdown = true;
		private int? _intellectualShutdownIdleTime = 60;
		private string _selectedIntellectualShutdownIdleTimeType = "минут";
		private int? _intellectualShutdownReachedHoursTime = 1;
		private int? _intellectualShutdownReachedMinutesTime = 0;
		private bool _isIntellectualShutdownRemarkExpand = false;
		private bool _watchingInRow;
		private bool _randomWatching = true;
		private int? _randomMixCount = 1;
		private string _selectedNoneRepeatTimeType = "дней";
		private int? _noneRepeatTime = 3;


		#region Default values

		/// <summary>
		/// Количество эпизодов к просмотру по умолчанию
		/// </summary>
		public int? DefaultStartEpisodesCount
		{
			get => _defaultStartEpisodesCount;
			set
			{
				if(value == null || value < 0)
				{
					value = 0;
				}

				if(value > 999_999)
				{
					value = 999_999;
				}

				_defaultStartEpisodesCount = value;
				NotifyOfPropertyChange(() => DefaultStartEpisodesCount);
				NotifyOfPropertyChange(() => ApproximateDuration);
			}
		}

		/// <summary>
		/// Примерная длительность указанного количества эпизодов
		/// </summary>
		public TimeSpan ApproximateDuration =>
			new TimeSpan(0, 0,
						 (int)Math.Ceiling(ApproximateEpisodeDuration.TotalSeconds * (DefaultStartEpisodesCount ?? 0)));

		#endregion

		#region Intellectual shutdown

		/// <summary>
		/// Флаг состояния интеллектуального выключения
		/// </summary>
		public bool IntellectualShutdown
		{
			get => _intellectualShutdown;
			set
			{
				_intellectualShutdown = value;
				NotifyOfPropertyChange(() => IntellectualShutdown);
				NotifyOfPropertyChange(() => IntelectualShutdownSettingsVisibility);
			}
		}

		/// <summary>
		/// Свойство Visibility элементов зависимых от состояния включенности интеллектуального выключения
		/// </summary>
		public Visibility IntelectualShutdownSettingsVisibility => IntellectualShutdown
			? Visibility.Visible
			: Visibility.Collapsed;

		/// <summary>
		/// Время ожидания, после которого начнется работа интеллектуального выключения
		/// </summary>
		public int? IntellectualShutdownIdleTime
		{
			get => _intellectualShutdownIdleTime;
			set
			{
				_intellectualShutdownIdleTime = SetIdleTime(value);
				NotifyOfPropertyChange(() => IntellectualShutdownIdleTime);
			}
		}
		/// <summary>
		/// Список типов времени ожидания интеллектуального выключения
		/// </summary>
		public BindableCollection<string> IntellectualShutdownIdleTimeTypeList =>
			new BindableCollection<string>
			{
				"часов",
				"минут",
				"секунд"
			};
		/// <summary>
		/// Выбранный тип времени ожидания интеллектуально выключения
		/// </summary>
		public string SelectedIntellectualShutdownIdleTimeType
		{
			get => _selectedIntellectualShutdownIdleTimeType;
			set
			{

				var tempValue = _selectedIntellectualShutdownIdleTimeType;
				_selectedIntellectualShutdownIdleTimeType = value;
				NotifyOfPropertyChange(() => SelectedIntellectualShutdownIdleTimeType);
				IntellectualShutdownIdleTime = ConvertIdleTime(tempValue);
			}
		}

		/// <summary>
		/// Часы времени суток для определения работы интеллектуального выключения
		/// </summary>
		public int? IntellectualShutdownReachedHoursTime
		{
			get => _intellectualShutdownReachedHoursTime;
			set
			{
				if(value == null || value < 0)
				{
					value = 0;

				}
				if(value > 24)
				{
					value = 23;
				}

				_intellectualShutdownReachedHoursTime = value;
				NotifyOfPropertyChange(() => IntellectualShutdownReachedHoursTime);
				NotifyOfPropertyChange(() => IntellectualShutdownReachedTime);
			}
		}

		/// <summary>
		/// Минуты времени суток для определения работы интеллектуального выключения
		/// </summary>
		public int? IntellectualShutdownReachedMinutesTime
		{
			get => _intellectualShutdownReachedMinutesTime;
			set
			{
				if(value == null || value < 0)
				{
					value = 0;

				}
				if(value > 59)
				{
					value = 59;
				}

				_intellectualShutdownReachedMinutesTime = value;
				NotifyOfPropertyChange(() => IntellectualShutdownReachedMinutesTime);
				NotifyOfPropertyChange(() => IntellectualShutdownReachedTime);
			}
		}

		public TimeSpan IntellectualShutdownReachedTime =>
			new TimeSpan(
				IntellectualShutdownReachedHoursTime ?? 0,
				IntellectualShutdownReachedMinutesTime ?? 0,
				0);

		/// <summary>
		/// Флаг определения состояния развернутости примечаний интеллектуального выключения
		/// </summary>
		public bool IsIntellectualShutdownRemarkExpand
		{
			get => _isIntellectualShutdownRemarkExpand;
			set
			{
				_isIntellectualShutdownRemarkExpand = value;
				NotifyOfPropertyChange(() => IsIntellectualShutdownRemarkExpand);
				NotifyOfPropertyChange(() => IntellectualShutdownRemarkVisibility);
			}
		}

		/// <summary>
		/// Свойство Visibility элементов зависимых от состояния включенности интелектуального выключения
		/// </summary>
		public Visibility IntellectualShutdownRemarkVisibility => IsIntellectualShutdownRemarkExpand
			? Visibility.Visible
			: Visibility.Collapsed;

		#endregion

		#region Watching mode

		/// <summary>
		/// Флаг просмотра эпизодов подряд
		/// </summary>
		public bool WatchingInRow
		{
			get => _watchingInRow;
			set
			{
				if(_watchingInRow == value)
					return;
				_watchingInRow = value;
				RandomWatching = !value;
				NotifyOfPropertyChange(() => WatchingInRow);
				NotifyOfPropertyChange(() => RandomWatching);
			}
		}
		/// <summary>
		/// Флаг просмотра эпизодов в случайном порядке
		/// </summary>
		public bool RandomWatching
		{
			get => _randomWatching;
			set
			{
				if(_randomWatching == value)
					return;
				_randomWatching = value;
				WatchingInRow = !value;
				NotifyOfPropertyChange(() => RandomWatching);
				NotifyOfPropertyChange(() => WatchingInRow);
				NotifyOfPropertyChange(() => RandomEnabledVisibility);
			}
		}
		/// <summary>
		/// Свойство Visibility элементов зависимых от просмотра эпизодов в случайном порядке
		/// </summary>
		public Visibility RandomEnabledVisibility => RandomWatching
			? Visibility.Visible
			: Visibility.Hidden;
		/// <summary>
		/// Количество смешиваний выбранных эпизодов (экспериментальная функция)
		/// </summary>
		public int? RandomMixCount
		{
			get => _randomMixCount;
			set
			{
				if(value == null ||
					value < 1)
				{
					value = 1;
				}

				if(value > 10)
				{
					value = 10;
				}


				_randomMixCount = value;
				NotifyOfPropertyChange(() => RandomMixCount);
			}
		}

		/// <summary>
		/// Коллекция типов времени для запрета повторов просмотренных эпизодов
		/// </summary>
		public BindableCollection<string> NoneRepeatTimeTypeList => new BindableCollection<string>
		{
			"никогда",
			"лет",
			"месяцев",
			"недель",
			"дней",
			"часов",
			"всегда"
		};

		/// <summary>
		/// Выбранный тип времени для запрета повторов просмотренных эпизодов
		/// </summary>
		public string SelectedNoneRepeatTimeType
		{
			get => _selectedNoneRepeatTimeType;
			set
			{
				var tempValue = _selectedNoneRepeatTimeType;
				_selectedNoneRepeatTimeType = value;
				NotifyOfPropertyChange(() => SelectedNoneRepeatTimeType);

				var temp = ConvertNonRepeatTime(tempValue);

				if (temp == null) return;

				NoneRepeatTime = temp;
			}
		}

		/// <summary>
		/// Значение времени, в течение которого просмотренный эпизод не будет повторяться
		/// </summary>
		public int? NoneRepeatTime
		{
			get => _noneRepeatTime;
			set
			{
				_noneRepeatTime = SetNonRepeatTime(value);
				NotifyOfPropertyChange(() => NoneRepeatTime);
			}
		}



		#endregion

		/// <summary>
		/// Установить время ожидания для интеллектуального выключения 
		/// </summary>
		/// <param name="value">входящее значение</param>
		/// <returns></returns>
		private int? SetIdleTime(int? value)
		{
			if(value == null ||
			   value < 0)
			{
				return 0;
			}

			switch(_selectedIntellectualShutdownIdleTimeType)
			{
				case "секунд":
					if(value > 86_400)
					{
						return 86_400;
					}
					break;
				case "минут":
					if(value > 1_440)
					{
						return 1_440;
					}
					break;
				case "часов":
					if(value > 24)
					{
						return 24;
					}
					break;
				default:
					throw new Exception(
						$"Некорректное принятое значение SelectedIntellectualShutdownIdleTimeType\n" +
						$"Значение равно {SelectedIntellectualShutdownIdleTimeType}\n");
			}

			return value;

		}

		/// <summary>
		/// Конвертировать время ожидания интеллектуального выключения исходя из выбранного типа времени
		/// </summary>
		/// <param name="tempValue">Значение типа времени перед его изменением</param>
		/// <returns></returns>
		private int? ConvertIdleTime(string tempValue)
		{
			var value = _intellectualShutdownIdleTime;
			switch(_selectedIntellectualShutdownIdleTimeType)
			{
				case "секунд":
					if(tempValue == "часов")
					{
						value *= 3600;
					}
					else
					{
						value *= 60;
					}
					break;
				case "минут":
					if(tempValue == "часов")
					{
						value *= 60;
					}
					else
					{
						value = (int)Math.Ceiling((double)value / 60);
					}
					break;
				case "часов":
					if(tempValue == "минут")
					{
						value = (int)Math.Ceiling((double)value / 60);
					}
					else
					{
						value = (int)Math.Ceiling((double)value / 3600);

					}
					break;
			}

			return value;

		}

		/// <summary>
		/// Установить значение времени, в течение которого просмотренный эпизод не будет повторяться
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		private int? SetNonRepeatTime(int? value)
		{
			if(value == null ||
			   value <= 0)
			{
				SelectedNoneRepeatTimeType = "всегда";

				return 1;
			}

			switch(_selectedNoneRepeatTimeType)
			{
				case "лет":
					if(value > 100)
					{
						SelectedNoneRepeatTimeType = "никогда";
						return 100;
					}
					break;
				case "месяцев":
					if(value > 1_200)
					{
						SelectedNoneRepeatTimeType = "никогда";
						return 1_200;
					}
					break;
				case "недель":
					if(value > 5_225)
					{
						SelectedNoneRepeatTimeType = "никогда";
						return 5_225;
					}
					break;
				case "дней":
					if(value > 36_575)
					{
						SelectedNoneRepeatTimeType = "никогда";
						return 36_575;
					}
					break;
				case "часов":
					if(value > 877_800)
					{
						SelectedNoneRepeatTimeType = "никогда";
						return 877_800;
					}
					break;
			}

			return value;
		}
		/// <summary>
		/// Конвертация значений NonRepeatTime в зависимости от выбранного типа времени
		/// </summary>
		/// <param name="tempTypeValue"></param>
		/// <returns></returns>
		private int? ConvertNonRepeatTime(string tempTypeValue)
		{
			var timeValue = _noneRepeatTime;
			var typeValue = _selectedNoneRepeatTimeType;
			
			switch(typeValue)
			{
				case "никогда":
					return null;
				case "лет":
					switch(tempTypeValue)
					{
						case "никогда":
							timeValue = 99;
							break;
						case "месяцев":
							timeValue = (int)Math.Ceiling((double)timeValue / 12);

							break;
						case "недель":
							// в году примерно 52.25 недели
							timeValue = (int)Math.Ceiling((double)timeValue / 52.25);
							break;
						case "дней":
							// каждый 4-й год весокосный, значит в году 365,75 дней
							timeValue = (int)Math.Ceiling((double)timeValue / 365.75);
							break;
						case "часов":
							// 365.75 дней умноженные на 24 == 8778 часов в году
							timeValue = (int)Math.Ceiling((double)timeValue / 8778);
							break;
						case "всегда":
							return null;
					}
					break;
				case "месяцев":
					switch(tempTypeValue)
					{
						case "никогда":
							break;
						case "лет":
							timeValue *= 12;
							break;
						case "недель":
							// 52.25 недель в году / на 12 месяцев == ~ 4.35 недель в месяце
							timeValue = (int)Math.Ceiling((double)timeValue / 4.35);
							break;
						case "дней":
							// 365.75 дней в году / на 12 месяцев == ~30.47916 дней в месяце 
							timeValue = (int)Math.Ceiling((double)timeValue / 30.47916);
							break;
						case "часов":
							// 30.47916 дней * 24 == 731.5
							timeValue = (int)Math.Ceiling((double)timeValue / 731.5);
							break;
						case "всегда":
							return null;
					}
					break;
				case "недель":
					switch(tempTypeValue)
					{
						case "никогда":
							break;
						case "лет":
							timeValue = (int)((double)timeValue * 52.25);
							break;
						case "месяцев":
							timeValue *= 12;
							break;
						case "дней":
							timeValue = (int)Math.Ceiling((double)timeValue / 7);
							break;
						case "часов":
							timeValue = (int)Math.Ceiling((double)timeValue / 24);
							break;
						case "всегда":
							return null;
					}
					break;
				case "дней":
					switch(tempTypeValue)
					{
						case "никогда":
							break;
						case "лет":
							timeValue = (int) ((double) timeValue * 365.75);
							break;
						case "месяцев":
							timeValue = (int) ((double) timeValue * 30.47916);
							break;
						case "недель":
							timeValue *= 7;
							break;
						case "часов":
							timeValue = (int)Math.Ceiling((double)timeValue / 24);
							break;
						case "всегда":
							return null;
					}
					break;
				case "часов":
					switch(tempTypeValue)
					{
						case "никогда":
							break;
						case "лет":
							timeValue *= 8778;
							break;
						case "месяцев":
							timeValue = (int) ((double) timeValue * 731.5);
							break;
						case "недель":
							// 24 * 7 = 168
							timeValue *= 168;
							break;
						case "дней":
							timeValue *= 24;
							break;
						case "всегда":
							return null;
					}
					break;
				case "всегда":
					return null;
			}

			return timeValue;
		}

	}
}
