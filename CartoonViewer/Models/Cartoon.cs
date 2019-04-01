using System.Collections.Generic;

namespace CartoonViewer.Models
{
	using System.ComponentModel.DataAnnotations;

	public class Cartoon
	{
		[Key]
		public int CartoonId { get; set; }

		public string Name { get; set; }
		public string Url { get; set; }
		public bool Checked { get; set; }

		public List<Season> Seasons { get; set; }
	}
}
