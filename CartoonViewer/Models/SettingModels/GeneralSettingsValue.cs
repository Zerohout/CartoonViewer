// ReSharper disable PossibleInvalidOperationException
namespace CartoonViewer.Models.SettingModels
{
	using System;
	using System.Collections.Generic;
	using System.Windows;
	using Caliburn.Micro;
	using CartoonModels;
	using Newtonsoft.Json;
	using static Helpers.Helper;

	/// <summary>
	/// Общие сохраняемы и загружаемые настройки связанные с приложением
	/// </summary>
	[Serializable]
	[JsonObject(MemberSerialization.OptOut)]
	public class GeneralSettingsValue : PropertyChangedBase, ICloneable
	{
		private int? _defaultEpisodesCount = 10;
		private bool _nightHelperShutdown = true;
		private string _selectedNightHelperShutdownIdleTimeType = "минут";
		private int? _nightHelperShutdownIdleTime = 60;
		private int? _nightHelperShutdownReachedHoursTime = 1;
		private int? _nightHelperShutdownReachedMinutesTime = 0;
		private bool _isNightHelperShutdownRemarkExpand;
		private bool _watchingInRow;
		private bool _randomWatching = true;
		private int? _randomMixCount = 1;
		private string _selectedNoneRepeatTimeType = "дней";
		private int? _noneRepeatTimeCount = 3;

		public GeneralSettingsValue()
		{

		}

		#region Default values

		/// <summary>
		/// Количество эпизодов к просмотру по умолчанию
		/// </summary>
		public int? DefaultEpisodesCount
		{
			get => _defaultEpisodesCount > _availableEpisodesCount
				? _availableEpisodesCount
				: _defaultEpisodesCount;
			set
			{
				if(_defaultEpisodesCount == value)
					return;

				if(value == null || value < 0)
				{
					value = 0;
				}

				if(value > _availableEpisodesCount)
				{
					value = _availableEpisodesCount;
				}

				_defaultEpisodesCount = value;
				NotifyOfPropertyChange(() => DefaultEpisodesCount);
				NotifyOfPropertyChange(() => ApproximateDuration);
			}
		}

		private int _availableEpisodesCount = 10;

		public int AvailableEpisodesCount
		{
			get => _availableEpisodesCount;
			set
			{
				_availableEpisodesCount = value;
				NotifyOfPropertyChange(() => AvailableEpisodesCount);
			}
		}

		private int? _episodesCount = 3;

		public int? EpisodesCount
		{
			get => _episodesCount;
			set
			{
				if(value == null || value < 0)
				{
					value = 0;
				}
				_episodesCount = value;
				NotifyOfPropertyChange(() => EpisodesCount);
				NotifyOfPropertyChange(() => ApproximateDuration);
			}
		}


		/// <summary>
		/// Примерная длительность указанного количества эпизодов
		/// </summary>
		public TimeSpan ApproximateDuration =>
			new TimeSpan(0, 0,
						 (int)Math.Ceiling(ApproximateEpisodeDuration.TotalSeconds * (EpisodesCount ?? 0)));

		/// <summary>
		/// Примерная длительность указанного количества эпизодов
		/// </summary>
		public TimeSpan ApproximateDefaultDuration =>
			new TimeSpan(0, 0,
			             (int)Math.Ceiling(ApproximateEpisodeDuration.TotalSeconds * (DefaultEpisodesCount ?? 0)));

		#endregion

		#region NightHelper

		/// <summary>
		/// Флаг состояния интеллектуального выключения
		/// </summary>
		public bool NightHelperShutdown
		{
			get => _nightHelperShutdown;
			set
			{
				if(_nightHelperShutdown == value)
					return;

				_nightHelperShutdown = value;
				NotifyOfPropertyChange(() => NightHelperShutdown);
				NotifyOfPropertyChange(() => NightHelperSettingsVisibility);
			}
		}

		/// <summary>
		/// Видимость элементов зависимых от состояния включенности интеллектуального выключения
		/// </summary>
		public Visibility NightHelperSettingsVisibility => NightHelperShutdown
			? Visibility.Visible
			: Visibility.Collapsed;

		/// <summary>
		/// Время ожидания, после которого начнется работа интеллектуального выключения
		/// </summary>
		public int? NightHelperShutdownIdleTime
		{
			get => _nightHelperShutdownIdleTime;
			set
			{
				if(_nightHelperShutdownIdleTime == value)
					return;
				_nightHelperShutdownIdleTime = SetIdleTime(value);
				NotifyOfPropertyChange(() => NightHelperShutdownIdleTime);
			}
		}

		public TimeSpan NightHelperShutdownTimeSpan
		{
			get
			{
				switch (_selectedNightHelperShutdownIdleTimeType)
				{
					case "часов":
						return new TimeSpan(_nightHelperShutdownIdleTime ?? 0,0,0);
					case "минут":
						return new TimeSpan(0,_nightHelperShutdownIdleTime ?? 0,0);
					case "секунд":
						return new TimeSpan(0,0,_nightHelperShutdownIdleTime ?? 0);
					default:
						return new TimeSpan();
				}
			}
		}
		/// <summary>
		/// Список типов времени ожидания интеллектуального выключения
		/// </summary>
		public ICollection<string> HigntHelperShutdownIdleTimeTypeList =>
			new List<string>
			{
				"часов",
				"минут",
				"секунд"
			};
		/// <summary>
		/// Выбранный тип времени ожидания интеллектуально выключения
		/// </summary>
		public string SelectedNightHelperShutdownIdleTimeType
		{
			get => _selectedNightHelperShutdownIdleTimeType;
			set
			{
				if(_selectedNightHelperShutdownIdleTimeType == value)
					return;

				var tempValue = _selectedNightHelperShutdownIdleTimeType;
				_selectedNightHelperShutdownIdleTimeType = value;
				NotifyOfPropertyChange(() => SelectedNightHelperShutdownIdleTimeType);
				NightHelperShutdownIdleTime = ConvertIdleTime(tempValue);
			}
		}

		/// <summary>
		/// Часы времени суток для определения работы интеллектуального выключения
		/// </summary>
		public int? NightHelperShutdownReachedHoursTime
		{
			get => _nightHelperShutdownReachedHoursTime;
			set
			{
				if(_nightHelperShutdownReachedHoursTime == value)
					return;

				if(value == null || value < 0)
				{
					value = 0;

				}
				if(value > 24)
				{
					value = 23;
				}

				_nightHelperShutdownReachedHoursTime = value;
				NotifyOfPropertyChange(() => NightHelperShutdownReachedHoursTime);
				NotifyOfPropertyChange(() => NightHelperShutdownReachedTime);
			}
		}

		/// <summary>
		/// Минуты времени суток для определения работы интеллектуального выключения
		/// </summary>
		public int? NightHelperShutdownReachedMinutesTime
		{
			get => _nightHelperShutdownReachedMinutesTime;
			set
			{
				if(_nightHelperShutdownReachedMinutesTime == value)
					return;

				if(value == null || value < 0)
				{
					value = 0;

				}
				if(value > 59)
				{
					value = 59;
				}

				_nightHelperShutdownReachedMinutesTime = value;
				NotifyOfPropertyChange(() => NightHelperShutdownReachedMinutesTime);
				NotifyOfPropertyChange(() => NightHelperShutdownReachedTime);
			}
		}

		public TimeSpan NightHelperShutdownReachedTime =>
			new TimeSpan(
				NightHelperShutdownReachedHoursTime ?? 0,
				NightHelperShutdownReachedMinutesTime ?? 0,
				0);

		/// <summary>
		/// Флаг определения состояния развернутости примечаний интеллектуального выключения
		/// </summary>
		public bool IsNightHelperShutdownRemarkExpand
		{
			get => _isNightHelperShutdownRemarkExpand;
			set
			{
				if(_isNightHelperShutdownRemarkExpand == value)
					return;

				_isNightHelperShutdownRemarkExpand = value;
				NotifyOfPropertyChange(() => IsNightHelperShutdownRemarkExpand);
				NotifyOfPropertyChange(() => NightHelperShutdownRemarkVisibility);
			}
		}

		/// <summary>
		/// Видимость элементов зависимых от состояния включенности интелектуального выключения
		/// </summary>
		public Visibility NightHelperShutdownRemarkVisibility => IsNightHelperShutdownRemarkExpand
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
				NotifyOfPropertyChange(() => WatchingInRowVisibility);
				NotifyOfPropertyChange(() => RandomEnabledVisibility);
			}
		}

		private int? _lastWatchedEpisodeInRowFullNumber = 101;

		public int? LastWatchedEpisodeInRowFullNumber
		{
			get => _lastWatchedEpisodeInRowFullNumber;
			set
			{
				if (value == null ||
				    value <= 100)
				{
					value = 101;
				}

				_lastWatchedEpisodeInRowFullNumber = value;
				NotifyOfPropertyChange(() => LastWatchedEpisodeInRowFullNumber);
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
				NotifyOfPropertyChange(() => WatchingInRowVisibility);
				NotifyOfPropertyChange(() => RandomEnabledVisibility);
			}
		}
		/// <summary>
		/// Видимость элементов зависимых от просмотра эпизодов в случайном порядке
		/// </summary>
		public Visibility RandomEnabledVisibility => RandomWatching
			? Visibility.Visible
			: Visibility.Hidden;

		public Visibility WatchingInRowVisibility => WatchingInRow
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
				if(_randomMixCount == value)
					return;

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

		public TimeSpan NonRepeatTime
		{
			get
			{
				var result = new TimeSpan();

				switch(SelectedNoneRepeatTimeType)
				{
					case "никогда":
						result = TimeSpan.MaxValue;
						break;
					case "лет":
						result = new TimeSpan(
							(int)((double)NoneRepeatTimeCount * 365.25),
							0, 0, 0);
						break;
					case "месяцев":
						result = new TimeSpan(
							(int)((double)NoneRepeatTimeCount * 30.47916),
							0, 0, 0);
						break;
					case "недель":
						result = new TimeSpan(
							(int)NoneRepeatTimeCount * 7,
							0, 0, 0);
						break;
					case "дней":
						result = new TimeSpan(
							(int)NoneRepeatTimeCount,
							0, 0, 0);
						break;
					case "часов":
						result = new TimeSpan(
							(int)NoneRepeatTimeCount,
							0, 0);
						break;
					case "всегда":
						break;
				}

				return result;
			}
		}

		/// <summary>
		/// Коллекция типов времени для запрета повторов просмотренных эпизодов
		/// </summary>
		public ICollection<string> NoneRepeatTimeTypeList => new List<string>
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
				if(_selectedNoneRepeatTimeType == value)
					return;

				var tempValue = _selectedNoneRepeatTimeType;
				_selectedNoneRepeatTimeType = value;
				NotifyOfPropertyChange(() => SelectedNoneRepeatTimeType);

				var temp = ConvertNonRepeatTime(tempValue);

				if(temp == null)
					return;

				NoneRepeatTimeCount = temp;
			}
		}

		/// <summary>
		/// Значение времени, в течение которого просмотренный эпизод не будет повторяться
		/// </summary>
		public int? NoneRepeatTimeCount
		{
			get => _noneRepeatTimeCount;
			set
			{
				if(_noneRepeatTimeCount == value)
					return;

				_noneRepeatTimeCount = SetNonRepeatTime(value);
				NotifyOfPropertyChange(() => NoneRepeatTimeCount);
			}
		}



		#endregion

		#region Methods

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

			switch(_selectedNightHelperShutdownIdleTimeType)
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
						$"Некорректное принятое значение SelectedNightHelperShutdownIdleTimeType\n" +
						$"Значение равно {SelectedNightHelperShutdownIdleTimeType}\n");
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
			var value = _nightHelperShutdownIdleTime;

			if(tempValue == null)
				return null;

			switch(_selectedNightHelperShutdownIdleTimeType)
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
					if(value > 36_525)
					{
						SelectedNoneRepeatTimeType = "никогда";
						return 36_525;
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
			var timeValue = _noneRepeatTimeCount;
			var typeValue = _selectedNoneRepeatTimeType;

			if(tempTypeValue == null)
				return null;

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
							// каждый 4-й год весокосный, значит в году 365,25 дней
							timeValue = (int)Math.Ceiling((double)timeValue / 365.25);
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
							timeValue = (int)((double)timeValue * 365.25);
							break;
						case "месяцев":
							timeValue = (int)((double)timeValue * 30.47916);
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
							timeValue = (int)((double)timeValue * 731.5);
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

		#endregion


		/// <summary>
		/// Клонирование данного класса
		/// </summary>
		/// <returns></returns>
		public object Clone() => MemberwiseClone();
	}
}
