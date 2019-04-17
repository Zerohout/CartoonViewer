namespace CartoonViewer.Models.CartoonModels
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	public class CartoonEpisode
	{
		public CartoonEpisode()
		{
			EpisodeVoiceOvers = new List<CartoonVoiceOver>();
		}

		[Key]
		public int CartoonEpisodeId { get; set; }

		public int Number { get; set; }
		public string NumberName => $"{Number} эпизод";


		public int SkipCount { get; set; }

		public string Name { get; set; }
		public string Description { get; set; }
		public bool Checked { get; set; }

		public TimeSpan DelayedSkip { get; set; }
		public TimeSpan CreditsStart { get; set; }
		public TimeSpan Duration { get; set; }
		public DateTime LastDateViewed { get; set; } = new DateTime(2019,01,01);

		public List<CartoonVoiceOver> EpisodeVoiceOvers { get; set; }

		[ForeignKey("CartoonSeason")]
		public int CartoonSeasonId { get; set; }
		public CartoonSeason CartoonSeason { get; set; }
	}
}
