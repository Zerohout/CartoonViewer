using System.Collections.Generic;

namespace CartoonViewer.Models
{
	using System.ComponentModel.DataAnnotations;

	public class Cartoon
	{
		[Key]
		public int CartoonId { get; set; }
		[Required]
		[MinLength(2)]
		[MaxLength(30)]
		public string Name { get; set; }
		public bool Checked { get; set; }

		public List<Season> Seasons { get; set; }
	}
}
