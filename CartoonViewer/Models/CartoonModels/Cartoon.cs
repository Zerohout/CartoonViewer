namespace CartoonViewer.Models.CartoonModels
{
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;

	public class Cartoon
	{
		public Cartoon()
		{
			ElementValues = new List<ElementValue>();
		}

		[Key]
		public int CartoonId { get; set; }

		public string Name { get; set; }
		public bool Checked { get; set; }

		public List<Season> Seasons { get; set; }

		public List<ElementValue> ElementValues { get; set; }
		public CartoonUrl CartoonUrl { get; set; }

	}
}
