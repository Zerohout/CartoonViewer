namespace CartoonViewer.Models.CartoonModels
{
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	public class Cartoon
	{
		public Cartoon()
		{
			WebSites = new List<WebSite>();
			CartoonUrls = new List<CartoonUrl>();
			Seasons = new List<Season>();
		}

		[Key]
		public int CartoonId { get; set; }

		public string Name { get; set; }
		public string Description { get; set; }
		public string CartoonType { get; set; }
		public bool Checked { get; set; }

		public List<WebSite> WebSites { get; set; }
		public List<CartoonUrl> CartoonUrls { get; set; }
		public List<Season> Seasons { get; set; }

	}
}
