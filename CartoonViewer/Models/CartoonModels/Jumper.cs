using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartoonViewer.Models.CartoonModels
{
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using Newtonsoft.Json;

	/// <summary>
	/// Джампер или иначе - пропуск участка эпизода
	/// </summary>
	public class Jumper
	{
		[Key]
		public int JumperId { get; set; }

		public int Number { get; set; }

		public int SkipCount { get; set; } = 7;

		public TimeSpan JumperStartTime { get; set; }
		

		[ForeignKey("CartoonEpisode")]
		public int CartoonEpisodeId { get; set; }
		[JsonIgnore]
		public CartoonEpisode CartoonEpisode { get; set; }
	}
}
