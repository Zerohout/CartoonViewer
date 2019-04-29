namespace CartoonViewer.Models.SettingModels
{
	/// <summary>
	/// Класс для отображения времени эпизода
	/// </summary>
	public class EpisodeTime
	{
		private int? _jumperTimeHours;
		private int? _jumperTimeMinutes;
		private int? _jumperTimeSeconds;

		private int? _skipCount;

		private int? _creditsTimeHours;
		private int? _creditsTimeMinutes;
		private int? _creditsTimeSeconds;

		public int? JumperTimeHours
		{
			get => _jumperTimeHours;
			set => _jumperTimeHours = HoursVerify(value);
		}
		
		public int? JumperTimeMinutes
		{
			get => _jumperTimeMinutes;
			set => _jumperTimeMinutes = MinSecVerify(value);
		}

		public int? JumperTimeSeconds
		{
			get => _jumperTimeSeconds;
			set => _jumperTimeSeconds = MinSecVerify(value);
		}
		public int? SkipCount
		{
			get => _skipCount;
			set
			{
				if(value == null || value < 0)
					value = 0;
				_skipCount = value;
			}
		}
		public int? CreditsTimeHours
		{
			get => _creditsTimeHours;
			set => _creditsTimeHours = HoursVerify(value);
		}
		
		public int? CreditsTimeMinutes
		{
			get => _creditsTimeMinutes;
			set => _creditsTimeMinutes = MinSecVerify(value);
		}

		public int? CreditsTimeSeconds
		{
			get => _creditsTimeSeconds;
			set => _creditsTimeSeconds = MinSecVerify(value);
		}


		


		private int? MinSecVerify(int? value)
		{
			if(value == null || value < 0)
				return 0;
			return value >= 60
				? 59
				: value;
		}

		private int? HoursVerify(int? value)
		{
			if(value == null || value < 0)
				return 0;
			return value >= 24
				? 23
				: value;
		}
	}
}
