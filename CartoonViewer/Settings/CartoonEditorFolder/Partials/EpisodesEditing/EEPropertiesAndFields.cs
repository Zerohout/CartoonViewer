// ReSharper disable CheckNamespace
namespace CartoonViewer.Settings.CartoonEditorFolder.ViewModels
{
	using System;
	using System.Windows;
	using Caliburn.Micro;
	using Helpers;
	using Models.CartoonModels;
	using Models.SettingModels;
	using Newtonsoft.Json;

	public partial class EpisodesEditingViewModel : Screen, ISettingsViewModel
	{
		private BindableCollection<CartoonEpisode> _episodes = new BindableCollection<CartoonEpisode>();
		private BindableCollection<CartoonVoiceOver> _cartoonVoiceOvers = new BindableCollection<CartoonVoiceOver>();
		private BindableCollection<CartoonVoiceOver> _voiceOvers = new BindableCollection<CartoonVoiceOver>();
		private BindableCollection<EpisodeOption> _episodeOptions = new BindableCollection<EpisodeOption>();
		private BindableCollection<Jumper> _jumpers = new BindableCollection<Jumper>();

		private CartoonEpisode _selectedEpisode;
		private CartoonEpisode _editableEpisode;
		private EpisodeOption _selectedEpisodeOption;
		private EpisodeOption _importingEpisodeOption;
		private Jumper _selectedJumper;
		private EpisodeTime _editableEpisodeTime;

		private bool _isNotEditing = true;
		private bool _isJumperSelectionEnable = true;
		private bool _isJumperSwapping;

		private (int CurrentIndex, int EndIndex) EpisodeIndexes;

		#region Collections

		/// <summary>
		/// Коллекция эпизодов
		/// </summary>
		public BindableCollection<CartoonEpisode> Episodes
		{
			get => _episodes;
			set
			{
				_episodes = value;
				EpisodeIndexes.EndIndex = _episodes.Count - 1;
				NotifyOfPropertyChange(() => Episodes);
			}
		}

		/// <summary>
		/// Список озвучек выбранного м/с
		/// </summary>
		public BindableCollection<CartoonVoiceOver> CartoonVoiceOvers
		{
			get => _cartoonVoiceOvers;
			set
			{
				_cartoonVoiceOvers = value;
				NotifyOfPropertyChange(() => CartoonVoiceOvers);
			}
		}


		/// <summary>
		/// Коллекция озвучек эпизода
		/// </summary>
		public BindableCollection<CartoonVoiceOver> VoiceOvers
		{
			get => _voiceOvers;
			set
			{
				_voiceOvers = value;
				NotifyOfPropertyChange(() => VoiceOvers);
			}
		}


		public BindableCollection<EpisodeOption> EpisodeOptions
		{
			get => _episodeOptions;
			set
			{
				_episodeOptions = value;
				NotifyOfPropertyChange(() => EpisodeOptions);
			}
		}

		/// <summary>
		/// Коллекция "джамперов"
		/// </summary>
		public BindableCollection<Jumper> Jumpers
		{
			get => _jumpers;
			set
			{
				_jumpers = value;
				NotifyOfPropertyChange(() => Jumpers);
				NotifyOfPropertyChange(() => EpisodeDuration);

			}
		}

		#endregion

		/// <summary>
		/// Выбранный эпизод
		/// </summary>
		public CartoonEpisode SelectedEpisode
		{
			get => _selectedEpisode;
			set
			{
				SettingsHelper.GlobalIdList.EpisodeId = value?.CartoonEpisodeId ?? 0;
				_selectedEpisode = value;
				NotifyOfPropertyChange(() => SelectedEpisode);
				NotifyEpisodeListButtons();
			}
		}

		private CartoonVoiceOver _defaultVoiceOver;

		public CartoonVoiceOver DefaultVoiceOver
		{
			get => _defaultVoiceOver;
			set
			{
				_defaultVoiceOver = value;
				NotifyOfPropertyChange(() => DefaultVoiceOver);
			}
		}


		/// <summary>
		/// Данные эпизода до изменений
		/// </summary>
		public string TempEpisodeSnapshot { get; set; }

		/// <summary>
		/// Данные опции эпизода до изменений
		/// </summary>
		public string TempEpisodeOptionSnapshot { get; set; }

		/// <summary>
		/// Редактируемый экземпляр выбранного эпизода
		/// </summary>
		public CartoonEpisode EditableEpisode
		{
			get => _editableEpisode;
			set
			{
				_editableEpisode = value;
				NotifyOfPropertyChange(() => EditableEpisode);
			}
		}

		/// <summary>
		/// Выбранная опция эпизода
		/// </summary>
		public EpisodeOption SelectedEpisodeOption
		{
			get => _selectedEpisodeOption;
			set
			{
				_selectedEpisodeOption = value;
				NotifyOfPropertyChange(() => SelectedEpisodeOption);
			}
		}
		/// <summary>
		/// Опция эпизода выбранная для импорта данных
		/// </summary>
		public EpisodeOption ImportingEpisodeOption
		{
			get => _importingEpisodeOption;
			set
			{
				_importingEpisodeOption = value;
				NotifyOfPropertyChange(() => ImportingEpisodeOption);
			}
		}

		/// <summary>
		/// Выбранный джампер
		/// </summary>
		public Jumper SelectedJumper
		{
			get => _selectedJumper;
			set
			{
				_selectedJumper = value;
				NotifyOfPropertyChange(() => SelectedJumper);
			}
		}

		//private Jumper _swappingJumper;

		//public Jumper SwappingJumper
		//{
		//	get => _swappingJumper;
		//	set
		//	{
		//		_swappingJumper = value;
		//		NotifyOfPropertyChange(() => SwappingJumper);
		//	}
		//}

		/// <summary>
		/// Редактируемые данные времени эпизода
		/// </summary>
		public EpisodeTime EditableEpisodeTime
		{
			get => _editableEpisodeTime;
			set
			{
				_editableEpisodeTime = value;
				NotifyOfPropertyChange(() => EditableEpisodeTime);
			}
		}

		#region Operable properties
		/// <summary>
		/// Длительность эпизода
		/// </summary>
		public TimeSpan EpisodeDuration => CalculatingDuration();

		#endregion

		#region Flags

		/// <summary>
		/// Флаг для корректной работы конструктора XAML
		/// </summary>
		public bool IsDesignTime { get; set; }

		/// <summary>
		/// Флаг состояния не редактирования
		/// </summary>
		public bool IsNotEditing
		{
			get => _isNotEditing;
			set
			{
				_isNotEditing = value;
				NotifyOfPropertyChange(() => IsNotEditing);
				NotifyOfPropertyChange(() => EpisodeEditingVisibility);
				NotifyOfPropertyChange(() => DefaultVoiceOverVisibility);
				NotifyEpisodeListButtons();
				NotifyTimeProperties();
				NotifyEditingButtons();
			}
		}

		///// <summary>
		///// Флаг состояния смены местами джампера
		///// </summary>
		//public bool IsJumperSwapping
		//{
		//	get => _isJumperSwapping;
		//	set
		//	{
		//		_isJumperSwapping = value;
		//		NotifyOfPropertyChange(() => IsJumperSwapping);
		//		NotifyOfPropertyChange(() => JumperEditingVisibility);
		//		NotifyOfPropertyChange(() => SwappingJumpersVisibility);
		//		_isJumperSelectionEnable = !value;
		//		NotifyOfPropertyChange(() => IsJumperSelectionEnable);
		//	}
		//}


		/// <summary>
		/// Флаг состояния элементов связанными с выбором редактируемого джампера
		/// </summary>
		public bool IsJumperSelectionEnable
		{
			get => _isJumperSelectionEnable;
			set
			{
				_isJumperSelectionEnable = value;
				NotifyOfPropertyChange(() => IsJumperSelectionEnable);
				_isJumperSwapping = !value;
				//NotifyOfPropertyChange(() => IsJumperSwapping);
			}
		}



		/// <summary>
		/// Флаг наличия изменений
		/// </summary>
		public bool HasChanges
		{
			get
			{
				if(EditableEpisode == null || SelectedEpisodeOption == null ||
				   string.IsNullOrWhiteSpace(TempEpisodeSnapshot) ||
				   string.IsNullOrWhiteSpace(TempEpisodeOptionSnapshot))
				{
					return false;
				}

				var tempEpisode = JsonConvert.DeserializeObject<CartoonEpisode>(TempEpisodeSnapshot);
				var tempOption = JsonConvert.DeserializeObject<EpisodeOption>(TempEpisodeOptionSnapshot);

				if(Helper.IsEquals(EditableEpisode, tempEpisode) &&
					Helper.IsEquals(SelectedEpisodeOption, tempOption))
				{
					return false;
				}

				return true;
			}
		}

		#endregion

		#region Visibility

		/// <summary>
		/// Видимость элементов связанных с состоянием редактирования
		/// </summary>
		public Visibility EpisodeEditingVisibility => IsNotEditing is true
			? Visibility.Hidden
			: Visibility.Visible;

		public Visibility DefaultVoiceOverVisibility => IsNotEditing is true
			? Visibility.Visible
			: Visibility.Hidden;



		///// <summary>
		///// Видимость полей редактирования джампера
		///// </summary>
		//public Visibility JumperEditingVisibility => IsJumperSwapping is true
		//	? Visibility.Hidden
		//	: Visibility.Visible;

		//public Visibility SwappingJumpersVisibility => IsJumperSwapping is true
		//	? Visibility.Visible
		//	: Visibility.Hidden;

		#endregion

	}
}
