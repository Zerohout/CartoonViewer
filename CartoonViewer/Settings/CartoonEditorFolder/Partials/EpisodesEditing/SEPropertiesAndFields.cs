// ReSharper disable CheckNamespace
namespace CartoonViewer.Settings.CartoonEditorFolder.ViewModels
{
	using System;
	using System.Windows;
	using Caliburn.Micro;
	using Helpers;
	using Models.CartoonModels;
	using Models.SettingModels;

	public partial class EpisodesEditingViewModel : Screen, ISettingsViewModel
	{
		private Visibility _episodeEditingVisibility = Visibility.Hidden;
		private BindableCollection<CartoonVoiceOver> _voiceOvers = new BindableCollection<CartoonVoiceOver>();
		private BindableCollection<CartoonEpisode> _episodes = new BindableCollection<CartoonEpisode>();
		private CartoonEpisode _selectedEpisode;
		private CartoonEpisode _tempEpisode;
		private EpisodeTime _episodeTime;
		private CartoonEpisode _editingEpisode;
		private bool _isNotEditing = true;


		public BindableCollection<CartoonVoiceOver> VoiceOvers
		{
			get => _voiceOvers;
			set
			{
				_voiceOvers = value;
				NotifyOfPropertyChange(() => VoiceOvers);
			}
		}

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
		/// Выбранный эпизод
		/// </summary>
		public CartoonEpisode SelectedEpisode
		{
			get => _selectedEpisode;
			set
			{
				_selectedEpisode = value;
				NotifyOfPropertyChange(() => SelectedEpisode);
				SettingsHelper.GlobalIdList.EpisodeId = value?.CartoonEpisodeId ?? 0;
				NotifyEpisodeListButtons();
			}
		}
		/// <summary>
		/// Редактируемый экземпляр выбранного эпизода
		/// </summary>
		public CartoonEpisode EditingEpisode
		{
			get => _editingEpisode;
			set
			{
				_editingEpisode = value;
				NotifyOfPropertyChange(() => EditingEpisode);
			}
		}
		/// <summary>
		/// Временный экземпляр редактируемого эпизода для отслеживания изменений
		/// </summary>
		public CartoonEpisode TempEpisode
		{
			get => _tempEpisode;
			set
			{
				_tempEpisode = value;
				NotifyOfPropertyChange(() => TempEpisode);
			}
		}

		public EpisodeTime EpisodeTime
		{
			get => _episodeTime;
			set
			{
				_episodeTime = value;

				NotifyOfPropertyChange(() => EpisodeTime);
			}
		}

		public TimeSpan EpisodeDuration => CalculatingDuration(EditingEpisode);



		#region Flags

		/// <summary>
		/// Флаг для корректной работы конструктора XAML
		/// </summary>
		public bool IsDesignTime { get; set; }

		public bool IsNotEditing
		{
			get => _isNotEditing;
			set
			{
				_isNotEditing = value;
				NotifyOfPropertyChange(() => IsNotEditing);
				NotifyEpisodeListButtons();
			}
		}

		/// <summary>
		/// Флаг наличия изменений
		/// </summary>
		public bool HasChanges
		{
			get
			{
				if(EditingEpisode == null ||
				   TempEpisode == null)
				{
					return false;
				}

				if(EditingEpisode.Name == TempEpisode.Name &&
				   EditingEpisode.Description == TempEpisode.Description &&
				   EditingEpisode.DelayedSkip == TempEpisode.DelayedSkip &&
				   EditingEpisode.SkipCount == TempEpisode.SkipCount &&
				   EditingEpisode.CreditsStart == TempEpisode.CreditsStart)
				{
					return false;
				}

				return true;
			}
		}

		#endregion

		#region Visibility

		public Visibility EpisodeEditingVisibility
		{
			get => _episodeEditingVisibility;
			set
			{
				_episodeEditingVisibility = value;
				NotifyOfPropertyChange(() => EpisodeEditingVisibility);
			}
		}

		#endregion

	}
}
