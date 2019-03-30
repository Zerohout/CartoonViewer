namespace CartoonViewer.Models
{
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	public class VoiceOver
	{
		[Key]
		public int VoiceOverId { get; set; }
		
		public string Name { get; set; }
		public string Url { get; set; }
		public bool Checked { get; set; }

		[ForeignKey("Episode")]
		public int? EpisodeId { get; set; }
		public Episode Episode { get; set; }
	}
}
