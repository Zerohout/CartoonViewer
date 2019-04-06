namespace CartoonViewer.Models.CartoonModels
{
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	[Table("CartoonUrl")]
	public class CartoonUrl
	{
		[Key]
		[ForeignKey("Cartoon")]
		public int CartoonId { get; set; }

		public string MainUrl { get; set; }
		public string AdditionalUrl { get; set; }
		public string UrlParameter { get; set; }
		public string AdditionalUrlParameter { get; set; }


		public Cartoon Cartoon { get; set; }
	}
}
