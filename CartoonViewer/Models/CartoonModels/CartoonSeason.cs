namespace CartoonViewer.Models.CartoonModels
{
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Windows;

	public class CartoonSeason
	{
		public CartoonSeason()
		{
			CartoonEpisodes = new List<CartoonEpisode>();
		}

		[Key]
		public int CartoonSeasonId { get; set; }
		public int Number { get; set; }
		public bool Checked { get; set; }

		public string NumberName => $"{Number} сезон";

		[NotMapped] public Visibility CancelButtonVisibility { get; set; } = Visibility.Collapsed;

		public string Name { get; set; }
		public string Description { get; set; }

		public List<CartoonEpisode> CartoonEpisodes { get; set; }

		[ForeignKey("Cartoon")]
		public int CartoonId { get; set; }
		public Cartoon Cartoon { get; set; }
	}
}
