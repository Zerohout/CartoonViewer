namespace CartoonViewer.Models.CartoonModels
{
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	public class ElementValue
	{
		[Key]
		public int ElementValueId { get; set; }
		
		public string UserElementName { get; set; }

		public string Id { get; set; }
		public string Name { get; set; }
		public string ClassName { get; set; }
		public string TagName { get; set; }
		public string CssSelector { get; set; }
		public string LinkText { get; set; }
		public string PartialLinkText { get; set; }
		public string XPath { get; set; }

		[ForeignKey("Cartoon")]
		public int CartoonId { get; set; }
		public Cartoon Cartoon { get; set; }
	}
}
