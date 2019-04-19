﻿namespace CartoonViewer.ViewModels
{
	using System.Windows;
	using static Helpers.Helper;
	using Screen = Caliburn.Micro.Screen;

	public class DialogViewModel : Screen
	{
		private DialogState _currentState;

		private Visibility _visibility_Yes_No;
		private Visibility _visibility_Ok_Cancel;
		private Visibility _saveChangesVisibility;
		private Visibility _removeVisibility;

		public Visibility RemoveVisibility
		{
			get => _removeVisibility;
			set
			{
				_removeVisibility = value;
				NotifyOfPropertyChange(() => RemoveVisibility);
			}
		}


		public Visibility SaveChangesVisibility
		{
			get => _saveChangesVisibility;
			set
			{
				_saveChangesVisibility = value;
				NotifyOfPropertyChange(() => SaveChangesVisibility);
			}
		}

		public DialogResult DialogResult { get; set; }


		





		private string _errorTitle;
		private string _dialogTitle;
		private string _errorMessage;
		private string _message;
		private string _text_Ok_Cancel;

		public DialogState CurrentState
		{
			get => _currentState;
			set
			{
				_currentState = value;
				NotifyOfPropertyChange(() => CurrentState);
			}
		}

		public string Test { get; set; }
		public DialogViewModel(string message,
			DialogState currentState = DialogState.OK,
			string dialogTitle = null)
		{
			switch(currentState)
			{
				case DialogState.SAVE_CHANGES:
					SaveChangesVisibility = Visibility.Visible;
					RemoveVisibility = Visibility.Hidden;
					CancelChangesVisibility = Visibility.Hidden;
					_message = message ?? "Сохранить ваши изменения?";
					_dialogTitle = dialogTitle ?? "Сохранить изменения?";
					return;
				case DialogState.CANCEL_CHANGES:
					SaveChangesVisibility = Visibility.Hidden;
					RemoveVisibility = Visibility.Hidden;
					CancelChangesVisibility = Visibility.Visible;
					_message = message ?? "Отменить ваши изменения?";
					_dialogTitle = dialogTitle ?? "Отменить изменения?";
					return;
				case DialogState.REMOVE:
					SaveChangesVisibility = Visibility.Hidden;
					RemoveVisibility = Visibility.Visible;
					CancelChangesVisibility = Visibility.Hidden;
					_message = $"Вы действительно хотите удалить этот {message}?";
					_dialogTitle = dialogTitle ?? "Удалить объект?";
					return;
				case DialogState.YES_NO:
					SaveChangesVisibility = Visibility.Hidden;
					Visibility_Yes_No = Visibility.Visible;
					Visibility_Ok_Cancel = Visibility.Hidden;
					_message = message;
					_dialogTitle = dialogTitle ?? "У нас к вам вопрос:";
					return;
				case DialogState.YES_NO_CANCEL:
					SaveChangesVisibility = Visibility.Hidden;
					Visibility_Yes_No = Visibility.Visible;
					Visibility_Ok_Cancel = Visibility.Visible;
					_message = message;
					_dialogTitle = dialogTitle ?? "Вы уверены?";
					_text_Ok_Cancel = "Отмена";
					return;
				case DialogState.OK:
					SaveChangesVisibility = Visibility.Hidden;
					Visibility_Yes_No = Visibility.Hidden;
					Visibility_Ok_Cancel = Visibility.Visible;
					_message = message;
					_dialogTitle = dialogTitle ?? "Прочтите внимательно!";
					_text_Ok_Cancel = "ОК";
					return;
			}
		}

		

		public DialogViewModel()
		{

		}

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

		public void Button_Yes()
		{
			//DialogResult = true;
			TryClose();
		}

		public void Button_No()
		{
			//DialogResult = false;
			TryClose();
		}

		public void Button_Ok_Cancel()
		{
			//DialogResult = null;
			TryClose();
		}

		public void Closed()
		{
			//DialogResult = null;
			TryClose();
		}


		public string DialogTitle
		{
			get => _dialogTitle;
			set
			{
				_dialogTitle = value;
				NotifyOfPropertyChange(() => DialogTitle);
			}
		}

		public string ErrorTitle
		{
			get => _errorTitle;
			set
			{
				_errorTitle = value;
				NotifyOfPropertyChange(() => ErrorTitle);
			}
		}

		public string ErrorMessage
		{
			get => _errorMessage;
			set
			{
				_errorMessage = value;
				NotifyOfPropertyChange(() => ErrorMessage);
			}
		}

		public string Message
		{
			get => _message;
			set
			{
				_message = value;
				NotifyOfPropertyChange(() => Message);
			}
		}

		public string Text_Ok_Cancel
		{
			get => _text_Ok_Cancel;
			set
			{
				_text_Ok_Cancel = value;
				NotifyOfPropertyChange(() => Text_Ok_Cancel);
			}
		}

		public Visibility Visibility_Yes_No
		{
			get => _visibility_Yes_No;
			set
			{
				_visibility_Yes_No = value;
				NotifyOfPropertyChange(() => Visibility_Yes_No);
			}
		}

		public Visibility Visibility_Ok_Cancel
		{
			get => _visibility_Ok_Cancel;
			set
			{
				_visibility_Ok_Cancel = value;
				NotifyOfPropertyChange(() => Visibility_Ok_Cancel);
			}
		}

		private Visibility _cancelChangesVisibility;

		public Visibility CancelChangesVisibility
		{
			get => _cancelChangesVisibility;
			set
			{
				_cancelChangesVisibility = value;
				NotifyOfPropertyChange(() => CancelChangesVisibility);
			}
		}

	}
}
