namespace CartoonViewer.Models.CartoonModels
{
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Linq;
	using Newtonsoft.Json;

	/// <summary>
	/// Озвучка мультсериала
	/// </summary>
	public class CartoonVoiceOver
	{
		public CartoonVoiceOver()
		{
			Cartoons = new List<Cartoon>();
			CartoonEpisodes = new List<CartoonEpisode>();
			CheckedEpisodes = new List<CartoonEpisode>();

		}


		[Key]
		public int CartoonVoiceOverId { get; set; }


		public string Name { get; set; } = "";
		public string Description { get; set; } = "";
		public string UrlParameter { get; set; } = "";

		[NotMapped]
		public int SelectedEpisodeId { private get; set; }

		[NotMapped]
		[JsonIgnore]
		public bool Checked

		{
			get => CheckedEpisodes.Any(ce => ce.CartoonEpisodeId == SelectedEpisodeId);

			set
			{
				if(value is true)
				{
					if (CheckedEpisodes.Any(ce => ce.CartoonEpisodeId == SelectedEpisodeId) is false)
					{
						var addingEpisode = CartoonEpisodes
							.First(ce => ce.CartoonEpisodeId == SelectedEpisodeId);

						CheckedEpisodes.Add(addingEpisode);
					}
				}
				else
				{
					if(CheckedEpisodes.Any(ce => ce.CartoonEpisodeId == SelectedEpisodeId) is true)
					{
						var removingEpisode = CheckedEpisodes
							.First(ce => ce.CartoonEpisodeId == SelectedEpisodeId);

						CheckedEpisodes.Remove(removingEpisode);
					}
				}
			}
		}
		[JsonIgnore]
		public ICollection<Cartoon> Cartoons { get; set; }
		[JsonIgnore]
		public ICollection<CartoonEpisode> CartoonEpisodes { get; set; }
		[JsonIgnore]
		public ICollection<CartoonEpisode> CheckedEpisodes { get; set; }

	}
}
