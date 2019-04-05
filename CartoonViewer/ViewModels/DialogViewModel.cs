﻿namespace CartoonViewer.ViewModels
{
	using System.Windows;
	using Caliburn.Micro;
	using static Helpers.Helper;

	public class DialogViewModel : Screen
	{
		private DialogState _currentState;

		private Visibility _visibility_Yes_No;
		private Visibility _visibility_Ok_Cancel;
		private Visibility _visibility_Error;

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

		public DialogViewModel(string message,
			DialogState currentState = DialogState.OK,
			string dialogTitle = null,
			string errorTitle = null)
		{
			switch (currentState)
			{
				case DialogState.YES_NO:
					Visibility_Yes_No = Visibility.Visible;
					Visibility_Ok_Cancel = Visibility.Hidden;
					Visibility_Error = Visibility.Hidden;
					_message = message;
					_dialogTitle = dialogTitle ?? "У нас к вам вопрос:";
					return;
				case DialogState.YES_NO_CANCEL:
					Visibility_Yes_No = Visibility.Visible;
					Visibility_Ok_Cancel = Visibility.Visible;
					Visibility_Error = Visibility.Hidden;
					_message = message;
					_dialogTitle = dialogTitle ?? "Вы уверены?";
					_text_Ok_Cancel = "Я передумал";
					return;
				case DialogState.OK:
					Visibility_Yes_No = Visibility.Hidden;
					Visibility_Ok_Cancel = Visibility.Visible;
					Visibility_Error = Visibility.Hidden;
					_message = message;
					_dialogTitle = dialogTitle ?? "Прочтите внимательно!";
					_text_Ok_Cancel = "Понятно";
					return;
				case DialogState.ERROR:
					Visibility_Yes_No = Visibility.Hidden;
					Visibility_Ok_Cancel = Visibility.Hidden;
					Visibility_Error = Visibility.Visible;
					_errorMessage = message;
					_errorTitle = errorTitle ?? "Произошла неожиданная ошибка. Подробности ниже:";
					_dialogTitle = dialogTitle ?? "Произошла ошибка";
					return;
			}
		}

		public DialogViewModel()
		{

		}

		public void Button_Yes()
		{
			TryClose(true);
		}

		public void Button_No()
		{
			TryClose(false);
		}

		public void Button_Ok_Cancel()
		{
			TryClose();
		}

		public void Closed()
		{
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
		
		public Visibility Visibility_Error
		{
			get => _visibility_Error;
			set
			{
				_visibility_Error = value;
				NotifyOfPropertyChange(() => Visibility_Error);
			}
		}
	}
}