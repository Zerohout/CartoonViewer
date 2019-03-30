namespace CartoonViewer.Models
{
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	public class VoiceOver
	{
		[Key]
		public int VoiceOverId { get; set; }
		[Required]
		[MinLength(2)]
		[MaxLength(30)]
		public string Name { get; set; }
		public string Url { get; set; }
		public bool Checked { get; set; }

		public List<Episode> Episodes{ get; set; }

		[ForeignKey("Episode")]
		public int? EpisodeId { get; set; }
	}
}
