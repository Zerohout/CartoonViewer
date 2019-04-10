using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartoonViewer.Models.CartoonModels
{
	using System.ComponentModel.DataAnnotations;

	public class WebSite
	{
		public WebSite()
		{
			ElementValues = new List<ElementValue>();
			Cartoons = new List<Cartoon>();
		}

		[Key]
		public int WebSiteId { get; set; }

		public string Url { get; set; }

		public List<ElementValue> ElementValues { get; set; }

		public List<Cartoon> Cartoons { get; set; }

	}
}
