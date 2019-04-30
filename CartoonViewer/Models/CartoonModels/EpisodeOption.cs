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

	public class EpisodeOption
	{
		[Key]
		public int EpisodeOptionId { get; set; }

		public string Name { get; set; }
		public TimeSpan CreditsStart { get; set; } = new TimeSpan(0, 21, 30);
		public TimeSpan Duration { get; set; }
		public DateTime LastDateViewed { get; set; } = Helpers.SettingsHelper.ResetTime;

		public List<Jumper> Jumpers { get; set; }

		[ForeignKey("CartoonEpisode")]
		public int CartoonEpisodeId { get; set; }
		[JsonIgnore]
		public CartoonEpisode CartoonEpisode { get; set; }

		[ForeignKey("CartoonVoiceOver")]
		public int? CartoonVoiceOverId { get; set; }
		public CartoonVoiceOver CartoonVoiceOver { get; set; }
	}

}
