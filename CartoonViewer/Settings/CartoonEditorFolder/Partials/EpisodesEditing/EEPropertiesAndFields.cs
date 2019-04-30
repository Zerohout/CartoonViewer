// ReSharper disable CheckNamespace
namespace CartoonViewer.Settings.CartoonEditorFolder.ViewModels
{
	using System;
	using System.Linq;
	using System.Windows;
	using Caliburn.Micro;
	using Helpers;
	using Models.CartoonModels;
	using Models.SettingModels;
	using Newtonsoft.Json;

	public partial class EpisodesEditingViewModel : Screen, ISettingsViewModel
	{
		private BindableCollection<CartoonEpisode> _episodes = new BindableCollection<CartoonEpisode>();
		private BindableCollection<CartoonVoiceOver> _voiceOvers = new BindableCollection<CartoonVoiceOver>();
		private BindableCollection<EpisodeOption> _episodeOptions = new BindableCollection<EpisodeOption>();
		private BindableCollection<Jumper> _jumpers = new BindableCollection<Jumper>();

		private CartoonEpisode _selectedEpisode;
		private CartoonEpisode _editableEpisode;
		private EpisodeOption _selectedEpisodeOption;
		private Jumper _selectedJumper;
		private EpisodeTime _editableEpisodeTime;

		private bool _isNotEditing = true;
		private bool _isAdvancedSettings;

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

		
		/// <summary>
		/// Данные эпизода до изменений
		/// </summary>
		public string TempEpisodeSnapshot { get; set; }

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


		public EpisodeOption SelectedEpisodeOption
		{
			get => _selectedEpisodeOption;
			set
			{
				_selectedEpisodeOption = value;
				NotifyOfPropertyChange(() => SelectedEpisodeOption);
			}
		}

		public Jumper SelectedJumper
		{
			get => _selectedJumper;
			set
			{
				_selectedJumper = value;
				NotifyOfPropertyChange(() => SelectedJumper);
			}
		}

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
				NotifyEpisodeListButtons();
				NotifyEditingProperties();
				NotifyTimeProperties();
			}
		}
		/// <summary>
		/// Флаг состояния расширенных настроек
		/// </summary>
		public bool IsAdvancedSettings
		{
			get => _isAdvancedSettings;
			set
			{
				_isAdvancedSettings = value;
				NotifyOfPropertyChange(() => IsAdvancedSettings);
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

				if (Helper.IsEquals(EditableEpisode, tempEpisode) &&
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

		public Visibility JumperEditingVisibility => SelectedJumper != null
			? Visibility.Visible
			: Visibility.Hidden;

		

		#endregion

	}
}
