using System.Collections.Generic;

namespace CartoonViewer.Models.CartoonModels
{
	using System.ComponentModel.DataAnnotations;

	/// <summary>
	/// Веб сайт с мультсериалами
	/// </summary>
	public class CartoonWebSite
	{
		public CartoonWebSite()
		{
			ElementValues = new List<ElementValue>();
			Cartoons = new List<Cartoon>();
		}

		[Key]
		public int CartoonWebSiteId { get; set; }

		public string Url { get; set; } = "";

		public List<ElementValue> ElementValues { get; set; }

		public List<Cartoon> Cartoons { get; set; }

	}
}
