namespace CartoonViewer.Models.CartoonModels
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Entity.ModelConfiguration.Conventions;
	using System.Security.Cryptography;

	/// <summary>
	/// Эпизод (серия) мультсериала
	/// </summary>
	public class CartoonEpisode
	{
		public CartoonEpisode()
		{
			Jumpers = new List<Jumper>();
			EpisodeVoiceOvers = new List<CartoonVoiceOver>();
		}

		[Key]
		public int CartoonEpisodeId { get; set; }

		public int Number { get; set; }
		public string NumberName => $"{Number} эпизод";


		//public int SkipCount { get; set; }

		public string Name { get; set; } = "";
		public string Description { get; set; } = "";
		public bool Checked { get; set; } = true;

		//public TimeSpan DelayedSkip { get; set; }
		public TimeSpan CreditsStart { get; set; } = new TimeSpan(0,21,30);
		public TimeSpan Duration { get; set; }
		public DateTime LastDateViewed { get; set; } = Helpers.SettingsHelper.ResetTime;

		public List<Jumper> Jumpers { get; set; }
		public List<CartoonVoiceOver> EpisodeVoiceOvers { get; set; }

		[InverseProperty("CheckedEpisodes")]
		public CartoonVoiceOver CartoonVoiceOver { get; set; }

		[ForeignKey("CartoonSeason")]
		public int CartoonSeasonId { get; set; }
		public CartoonSeason CartoonSeason { get; set; }

		[ForeignKey("Cartoon")]
		public int? CartoonId { get; set; }
		public Cartoon Cartoon { get; set; }
	}
}
