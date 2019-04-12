using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartoonViewer.Helpers
{
	using Models.CartoonModels;

	public static class Cloner
	{
		/// <summary>
		/// Клоникрование списка Вебсайтов
		/// </summary>
		/// <param name="webSites">Клонируемый список сайтов</param>
		/// <returns></returns>
		public static List<WebSite> CloneWebSiteList(List<WebSite> webSites)
		{
			var result = new List<WebSite>();

			if (webSites == null || webSites.Count == 0) return result;


			foreach (var webSite in webSites)
			{
				result.Add(CloneWebSite(webSite));
			}

			return result;
		}

		/// <summary>
		/// Клонирование Вебсайта
		/// </summary>
		/// <param name="webSite">Клонируемый сайт</param>
		/// <returns></returns>
		public static WebSite CloneWebSite(WebSite webSite) => new WebSite
		{
			WebSiteId = webSite.WebSiteId,
			Url = webSite.Url,
			ElementValues = CloneElementValueList(webSite.ElementValues)
		};

		/// <summary>
		/// Клонирование списка значений элементов
		/// </summary>
		/// <param name="elementValues">Клонируемый список значений элементов</param>
		/// <returns></returns>
		public static List<ElementValue> CloneElementValueList(List<ElementValue> elementValues)
		{
			var result = new List<ElementValue>();

			if (elementValues == null || elementValues.Count == 0) return result;

			foreach (var elementValue in elementValues)
			{
				result.Add(CloneElementValue(elementValue));
			}

			return result;
		}

		/// <summary>
		/// Клонирование значений элемента
		/// </summary>
		/// <param name="elementValue">Клонируемые значения элемента</param>
		/// <returns></returns>
		public static ElementValue CloneElementValue(ElementValue elementValue) => new ElementValue
		{
			ElementValueId = elementValue.ElementValueId,
			Id = elementValue.Id,
			Name = elementValue.Name,
			ElementName = elementValue.ElementName,
			ClassName = elementValue.ClassName,
			CssSelector = elementValue.CssSelector,
			LinkText = elementValue.LinkText,
			PartialLinkText = elementValue.PartialLinkText,
			TagName = elementValue.TagName,
			XPath = elementValue.XPath,
			WebSiteId = elementValue.WebSiteId
		};

		/// <summary>
		/// Клонирование списка мультфильмов
		/// </summary>
		/// <param name="cartoons">Клонируемый список мультфильмов</param>
		/// <returns></returns>
		public static List<Cartoon> CloneCartoonList(List<Cartoon> cartoons)
		{
			var result = new List<Cartoon>();

			if (cartoons == null ||
			    cartoons.Count == 0) return result;

			foreach (var cartoon in cartoons)
			{
				result.Add(CloneCartoon(cartoon));
			}

			return result;
		}

		/// <summary>
		/// Клонирование мультфильма
		/// </summary>
		/// <param name="cartoon">Клонируемый мультфильм</param>
		/// <returns></returns>
		public static Cartoon CloneCartoon(Cartoon cartoon) => new Cartoon
		{
			CartoonId = cartoon.CartoonId,
			Name = cartoon.Name,
			Description = cartoon.Description,
			CartoonType = cartoon.CartoonType,
			CartoonUrls = cartoon.CartoonUrls,
			Checked = cartoon.Checked,
			WebSites = CloneWebSiteList(cartoon.WebSites),
			Seasons = CloneSeasonList(cartoon.Seasons)
		};

		/// <summary>
		/// Клонирование списка ссылок мультфильма
		/// </summary>
		/// <param name="cartoonUrls">Клонируемый списко ссылок мультфильма</param>
		/// <returns></returns>
		public static List<CartoonUrl> CloneCartoonUrlList(List<CartoonUrl> cartoonUrls)
		{
			var result = new List<CartoonUrl>();

			if (cartoonUrls == null ||
			    cartoonUrls.Count == 0) return result;

			foreach (var cartoonUrl in cartoonUrls)
			{
				result.Add(CloneCartoonUrl(cartoonUrl));
			}

			return result;
		}

		/// <summary>
		/// Клонирование ссылок мультфильма
		/// </summary>
		/// <param name="cartoonUrl">Клонируемые ссылки мультфильма</param>
		/// <returns></returns>
		public static CartoonUrl CloneCartoonUrl(CartoonUrl cartoonUrl) => new CartoonUrl
		{
			CartoonUrlId = cartoonUrl.CartoonUrlId,
			Url = cartoonUrl.Url,
			WebSiteUrl = cartoonUrl.WebSiteUrl,
			Checked = cartoonUrl.Checked,
			CartoonId = cartoonUrl.CartoonId,
			WebSiteId = cartoonUrl.WebSiteId
		};

		/// <summary>
		/// Клонирование списка сезонов
		/// </summary>
		/// <param name="seasons">Клонируемый список сезонов</param>
		/// <returns></returns>
		public static List<Season> CloneSeasonList(List<Season> seasons)
		{
			var result = new List<Season>();

			if (seasons == null || seasons.Count == 0) return result;


			foreach (var season in seasons)
			{
				result.Add(CloneSeason(season));
			}

			return result;
		}

		/// <summary>
		/// Клонирование сезона
		/// </summary>
		/// <param name="season">Клонируемый сезон</param>
		/// <returns></returns>
		public static Season CloneSeason(Season season) => new Season
		{
			SeasonId = season.SeasonId,
			Name = season.Name,
			Number = season.Number,
			Description = season.Description,
			Checked = season.Checked,
			CartoonId = season.CartoonId,
			Episodes = CloneEpisodeList(season.Episodes)
		};

		/// <summary>
		/// Клонирование списка эпизодов
		/// </summary>
		/// <param name="episodes">Клонируемый список эпизодов</param>
		/// <returns></returns>
		public static List<Episode> CloneEpisodeList(List<Episode> episodes)
		{
			var result = new List<Episode>();

			if (episodes == null || episodes.Count == 0) return result;


			foreach (var episode in episodes)
			{
				result.Add(CloneEpisode(episode));
			}

			return result;
		}

		/// <summary>
		/// Клонирование эпизода
		/// </summary>
		/// <param name="episode">Клонируемый эпизод</param>
		/// <returns></returns>
		public static Episode CloneEpisode(Episode episode) => new Episode
		{
			EpisodeId = episode.EpisodeId,
			Name = episode.Name,
			Number = episode.Number,
			Description = episode.Description,
			Checked = episode.Checked,
			DelayedSkip = episode.DelayedSkip,
			SkipCount = episode.SkipCount,
			Duration = episode.Duration,
			CreditsStart = episode.CreditsStart,
			VoiceOvers = CopyVoiceOverList(episode.VoiceOvers),
			SeasonId = episode.SeasonId
		};

		/// <summary>
		/// Клонирование списка озвучек
		/// </summary>
		/// <param name="voiceOvers">Клонируемый список озвучек</param>
		/// <returns></returns>
		public static List<VoiceOver> CopyVoiceOverList(List<VoiceOver> voiceOvers)
		{
			var result = new List<VoiceOver>();

			if (voiceOvers == null || voiceOvers.Count == 0) return result;


			foreach (var voiceOver in voiceOvers)
			{
				result.Add(CopyVoiceOver(voiceOver));
			}

			return result;
		}

		/// <summary>
		/// Клонирование озвучки
		/// </summary>
		/// <param name="voiceOver">Клонируемая озвучка</param>
		/// <returns></returns>
		public static VoiceOver CopyVoiceOver(VoiceOver voiceOver) => new VoiceOver
		{
			VoiceOverId = voiceOver.VoiceOverId,
			Name = voiceOver.Name,
			Description = voiceOver.Description,
			UrlParameter = voiceOver.UrlParameter,
			Checked = voiceOver.Checked,
			EpisodeId = voiceOver.EpisodeId
		};
	}
}
