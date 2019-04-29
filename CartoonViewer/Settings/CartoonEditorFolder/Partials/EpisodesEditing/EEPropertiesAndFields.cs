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
		private BindableCollection<Jumper> _jumpers = new BindableCollection<Jumper>();
		private BindableCollection<CartoonVoiceOver> _voiceOvers = new BindableCollection<CartoonVoiceOver>();
		
		private CartoonEpisode _selectedEpisode;
		private CartoonEpisode _editableEpisode;
		private Jumper _selectedJumper;
		private EpisodeTime _editableEpisodeTime;

		private bool _isNotEditing = true;

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

		public string EditableEpisodeSnapshot { get; set; }

		/// <summary>
		/// Данные эпизода до изменений
		/// </summary>
		public string TempEpisodeSnapshot { get; set; }

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
		/// Флаг наличия изменений
		/// </summary>
		public bool HasChanges
		{
			get
			{
				if(EditableEpisode == null)
				{
					return false;
				}


				if (Helper.IsEquals(EditableEpisodeSnapshot, TempEpisodeSnapshot) &&
				    Helper.IsEquals(SelectedJumperSnapshot, TempJumperSnapshot))
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

		public string SelectedJumperSnapshot { get; set; }
		public string TempJumperSnapshot { get; set; }

		#endregion

	}
}
