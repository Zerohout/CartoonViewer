namespace CartoonViewer.ViewModels
{
	using System;
	using System.Windows;
	using static Helpers.Helper;
	using Screen = Caliburn.Micro.Screen;

	public class DialogViewModel : Screen
	{
		private string _dialogTitle;
		private string _message;
		private DialogType _currentType;

		public DialogViewModel()
		{

		}

		public DialogViewModel(string message,
			DialogType currentType,
			string dialogTitle = null)
		{
			CurrentType = currentType;
			_message = message;
			_dialogTitle = dialogTitle;
		}

		public DialogType CurrentType
		{
			get => _currentType;
			set
			{
				_currentType = value;
				NotifyOfPropertyChange(() => CurrentType);
				NotifyOfPropertyChange(() => YesVisibility);
				NotifyOfPropertyChange(() => NoVisibility);
				NotifyOfPropertyChange(() => CancelVisibility);
				NotifyOfPropertyChange(() => OkVisibility);
			}
		}

		public DialogResult DialogResult { get; set; }

		/// <summary>
		/// Заголовок диалогового окна
		/// </summary>
		public string DialogTitle
		{
			get
			{
				if(_dialogTitle != null) return _dialogTitle;

				switch(_currentType)
				{
					case DialogType.SAVE_CHANGES:
						return "Сохранить изменения?";
					case DialogType.CANCEL_CHANGES:
						return "Отменить изменения?";
					case DialogType.REMOVE_OBJECT:
						return "Удалить объект?";
					case DialogType.OVERWRITE_FILE:
						return "Перезаписать файл?";
					case DialogType.INFO:
						return "Информация.";
					default:
						throw new Exception("Некорректный DialogType");
				}
			}
			}
		/// <summary>
		/// Сообщение внутри диалогового окна
		/// </summary>
		public string Message
		{
			get
			{
				if (_message != null) return _message;

				switch(_currentType)
				{
					case DialogType.SAVE_CHANGES:
						return "Сохранить ваши изменения?";
					case DialogType.CANCEL_CHANGES:
						return "Отменить ваши изменения?";
					case DialogType.REMOVE_OBJECT:
						return $"Вы действительно хотите удалить выбранный объект?";
					case DialogType.OVERWRITE_FILE:
						return "Файл уже существует, перезаписать его?";
					case DialogType.INFO:
						return "Информация для пользователя.";
					default:
						throw new Exception("Некорректный DialogType");
				}
			}
		}

		/// <summary>
		/// Видимость кнопки Yes
		/// </summary>
		public Visibility YesVisibility =>
			CurrentType == DialogType.SAVE_CHANGES ||
			CurrentType == DialogType.CANCEL_CHANGES ||
			CurrentType == DialogType.REMOVE_OBJECT ||
			CurrentType == DialogType.OVERWRITE_FILE
				? Visibility.Visible
				: Visibility.Hidden;
		/// <summary>
		/// Видимость кнопки No
		/// </summary>
		public Visibility NoVisibility => 
			CurrentType == DialogType.SAVE_CHANGES ||
			CurrentType == DialogType.CANCEL_CHANGES ||
			CurrentType == DialogType.REMOVE_OBJECT ||
			CurrentType == DialogType.OVERWRITE_FILE
				? Visibility.Visible
				: Visibility.Hidden;

		/// <summary>
		/// Видимость кнопки Cancel
		/// </summary>
		public Visibility CancelVisibility =>
			CurrentType == DialogType.SAVE_CHANGES ||
			CurrentType == DialogType.OVERWRITE_FILE
				? Visibility.Visible
				: Visibility.Hidden;
		/// <summary>
		/// Видимость кнопки Ok
		/// </summary>
		public Visibility OkVisibility =>
			CurrentType == DialogType.INFO
				? Visibility.Visible
				: Visibility.Hidden;
			
		public void YesAction()
		{
			DialogResult = DialogResult.YES_ACTION;
			TryClose();
		}

		public void NoAction()
		{
			DialogResult = DialogResult.NO_ACTION;
			TryClose();
		}

		public void CancelAction()
		{
			DialogResult = DialogResult.CANCEL_ACTION;
			TryClose();
		}
	}
}
