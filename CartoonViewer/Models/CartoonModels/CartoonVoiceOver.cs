namespace CartoonViewer.Models.CartoonModels
{
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;

	public class CartoonVoiceOver
	{
		public CartoonVoiceOver()
		{
			Cartoons = new List<Cartoon>();
			CartoonEpisodes = new List<CartoonEpisode>();
		}


		[Key]
		public int CartoonVoiceOverId { get; set; }
		
		public string Name { get; set; }
		public string Description { get; set; }
		public string UrlParameter { get; set; }
		public bool Checked { get; set; }


		public List<Cartoon> Cartoons { get; set; }

		public List<CartoonEpisode> CartoonEpisodes { get; set; }




		//[ForeignKey("CartoonEpisode")]
		//public int? CartoonEpisodeId { get; set; }
		//public CartoonEpisode CartoonEpisode { get; set; }

		//[ForeignKey("Cartoon")]
		//public int? CartoonId { get; set; }
		//public Cartoon Cartoon { get; set; }
	}
}
