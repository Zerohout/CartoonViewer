﻿namespace CartoonViewer.Models.CartoonModels
{
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;

	/// <summary>
	/// Мультсериал
	/// </summary>
	public class Cartoon
	{
		public Cartoon()
		{
			CartoonWebSites = new List<CartoonWebSite>();
			CartoonUrls = new List<CartoonUrl>();
			CartoonSeasons = new List<CartoonSeason>();
			CartoonEpisodes = new List<CartoonEpisode>();
			CartoonVoiceOvers = new List<CartoonVoiceOver>();
		}

		[Key]
		public int CartoonId { get; set; }

		public string Name { get; set; } = "";
		public string Description { get; set; } = "";
		public string CartoonType { get; set; } = "";
		public bool Checked { get; set; } = true;

		public List<CartoonWebSite> CartoonWebSites { get; set; }
		public List<CartoonUrl> CartoonUrls { get; set; }
		public List<CartoonSeason> CartoonSeasons { get; set; }

		public List<CartoonEpisode> CartoonEpisodes { get; set; }
		public List<CartoonVoiceOver> CartoonVoiceOvers { get; set; }

	}
}
