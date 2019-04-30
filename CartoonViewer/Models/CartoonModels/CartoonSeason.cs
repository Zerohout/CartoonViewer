namespace CartoonViewer.Models.CartoonModels
{
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using Newtonsoft.Json;

	/// <summary>
	/// Сезон мультсериала
	/// </summary>
	public class CartoonSeason
	{
		public CartoonSeason()
		{
			CartoonEpisodes = new List<CartoonEpisode>();
			Name = $"{Number} сезон";
			Description = $"Описание {Number} сезона";
		}

		[Key]
		public int CartoonSeasonId { get; set; }
		public int Number { get; set; }
		public bool Checked { get; set; } = true;

		public string NumberName => $"{Number} сезон";

		public string Name { get; set; }
		public string Description { get; set; }

		public List<CartoonEpisode> CartoonEpisodes { get; set; }

		[ForeignKey("Cartoon")]
		public int CartoonId { get; set; }
		[JsonIgnore]
		public Cartoon Cartoon { get; set; }
	}
}
