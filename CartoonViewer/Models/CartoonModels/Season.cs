namespace CartoonViewer.Models.CartoonModels
{
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	public class Season
	{
		[Key]
		public int SeasonId { get; set; }
		public int Number { get; set; }
		public bool Checked { get; set; }

		public string Name { get; set; }
		public string Description { get; set; }

		public List<Episode> Episodes { get; set; }

		[ForeignKey("Cartoon")]
		public int CartoonId { get; set; }
		public Cartoon Cartoon { get; set; }
	}
}
