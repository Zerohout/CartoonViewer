using System.Collections.Generic;

namespace CartoonViewer.Models
{
	using System.ComponentModel.DataAnnotations;

	public class Cartoon
	{
		public Cartoon()
		{
			Seasons = new List<Season>();
		}

		[Key]
		public int CartoonId { get; set; }

		[MinLength(2)]
		[MaxLength(30)]
		public string Name { get; set; }
		public bool Checked { get; set; }

		public List<Season> Seasons { get; set; }
	}
}
