namespace CartoonViewer.Models.CartoonModels
{
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	/// <summary>
	/// Ссылки мультсериала (Сайт с м/с или сама ссылка на м/с, статус выбора)
	/// </summary>
	[Table("CartoonUrls")]
	public class CartoonUrl
	{
		[Key]
		public int CartoonUrlId { get; set; }

		public string Url { get; set; } = "";
		public string WebSiteUrl { get; set; } = "";
		public bool Checked { get; set; }

		[ForeignKey("CartoonWebSite")]
		public int CartoonWebSiteId { get; set; }
		public CartoonWebSite CartoonWebSite { get; set; }

		[ForeignKey("Cartoon")]
		public int CartoonId { get; set; }
		public Cartoon Cartoon { get; set; }
	}
}
