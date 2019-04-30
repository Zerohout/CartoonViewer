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
			EpisodeOptions = new List<EpisodeOption>();
			EpisodeVoiceOvers = new List<CartoonVoiceOver>();
		}

		[Key]
		public int CartoonEpisodeId { get; set; }

		public int Number { get; set; }
		public string NumberName => $"{Number} эпизод";

		public int FullNumber => (CartoonSeason?.Number ?? 0) * 100 + Number;
		public string Name { get; set; } = "";
		public string Description { get; set; } = "";
		public bool Checked { get; set; } = true;

		public List<EpisodeOption> EpisodeOptions { get; set; }
		
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
