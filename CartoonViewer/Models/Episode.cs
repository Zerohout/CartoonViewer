using System;

namespace CartoonViewer.Models
{
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	public class Episode
	{
		public Episode()
		{
			VoiceOvers = new List<VoiceOver>();
		}

		[Key]
		public int EpisodeId { get; set; }
		
		public int Number { get; set; }
		
		public int SkipCount { get; set; }

		[MaxLength(100)]
		public string Name { get; set; }
		[MaxLength(400)]
		public string Description { get; set; }
		public bool Checked { get; set; }

		public TimeSpan DelayedStart { get; set; }
		public TimeSpan CreditsStart { get; set; }
		public TimeSpan Duration { get; set; }

		public List<VoiceOver> VoiceOvers { get; set; }

		[ForeignKey("Season")]
		public int SeasonId { get; set; }
		public Season Season { get; set; }
	}
}
