namespace CartoonViewer.Models.SettingModels
{
	public class EpisodeTime
	{
		private string _delayedSkipMinutesString;
		private string _delayedSkipSecondsString;
		private string _skipCountString;
		private string _creditsStartHoursString;
		private string _creditsStartMinutesString;
		private string _creditsStartSecondsString;

		public string DelayedSkipMinutesString
		{
			get => _delayedSkipMinutesString;
			set => _delayedSkipMinutesString = AdditionalMinuteSecondVerification(VerificationValue(value));
		}

		public string DelayedSkipSecondsString
		{
			get => _delayedSkipSecondsString;
			set => _delayedSkipSecondsString = AdditionalMinuteSecondVerification(VerificationValue(value));
		}

		public string SkipCountString
		{
			get => _skipCountString;
			set => _skipCountString = VerificationValue(value);
		}

		public string CreditsStartHoursString
		{
			get => _creditsStartHoursString;
			set => _creditsStartHoursString = AdditionalHourVerification(VerificationValue(value));
		}

		public string CreditsStartMinutesString
		{
			get => _creditsStartMinutesString;
			set => _creditsStartMinutesString = AdditionalMinuteSecondVerification(VerificationValue(value));
		}

		public string CreditsStartSecondsString
		{
			get => _creditsStartSecondsString;
			set => _creditsStartSecondsString = AdditionalMinuteSecondVerification(VerificationValue(value));
		}

		private string VerificationValue(string value)
		{
			if(string.IsNullOrWhiteSpace(value) ||
				!int.TryParse(value, out var numValue))
			{
				return "0";
			}

			return value;
		}

		private string AdditionalHourVerification(string value)
		{
			if(int.Parse(value) >= 24)
			{
				return "23";
			}

			if(int.Parse(value) < 0)
			{
				return "0";
			}

			return value;
		}

		private string AdditionalMinuteSecondVerification(string value)
		{
			if(int.Parse(value) >= 60)
			{
				return "59";
			}

			if(int.Parse(value) < 0)
			{
				return "0";
			}

			return value;
		}
	}
}
