namespace CartoonViewer.Models.CartoonModels
{
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	public class ElementValue
	{
		[Key]
		public int ElementValueId { get; set; }
		
		public string ElementName { get; set; }

		public string Id { get; set; }
		public string Name { get; set; }
		public string ClassName { get; set; }
		public string TagName { get; set; }
		public string CssSelector { get; set; }
		public string LinkText { get; set; }
		public string PartialLinkText { get; set; }
		public string XPath { get; set; }

		[ForeignKey("CartoonWebSite")]
		public int CartoonWebSiteId { get; set; }
		public CartoonWebSite CartoonWebSite { get; set; }
	}
}
