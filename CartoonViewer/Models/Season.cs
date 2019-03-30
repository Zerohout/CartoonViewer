namespace CartoonViewer.Models
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

		public List<Episode> Episodes { get; set; }

		[ForeignKey("Cartoon")]
		public int CartoonId { get; set; }
		public Cartoon Cartoon { get; set; }
	}
}
