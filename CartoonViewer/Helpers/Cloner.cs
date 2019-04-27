using System.Collections.Generic;

namespace CartoonViewer.Helpers
{
	using Models.CartoonModels;

	public static class Cloner
	{
		/// <summary>
		/// Клонирование списка Вебсайтов
		/// </summary>
		/// <param name="webSites">Клонируемый список сайтов</param>
		/// <returns></returns>
		public static List<CartoonWebSite> CloneWebSiteList(List<CartoonWebSite> webSites)
		{
			var result = new List<CartoonWebSite>();

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
		/// <param name="cartoonWebSite">Клонируемый сайт</param>
		/// <returns></returns>
		public static CartoonWebSite CloneWebSite(CartoonWebSite cartoonWebSite) => new CartoonWebSite
		{
			CartoonWebSiteId = cartoonWebSite.CartoonWebSiteId,
			Url = cartoonWebSite.Url,
			ElementValues = CloneElementValueList(cartoonWebSite.ElementValues)
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
			CartoonWebSiteId = elementValue.CartoonWebSiteId
		};

		/// <summary>
		/// Клонирование списка мультсериалов
		/// </summary>
		/// <param name="cartoons">Клонируемый список мультсериалов</param>
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
		/// Клонирование мультсериала
		/// </summary>
		/// <param name="cartoon">Клонируемый мультсериал</param>
		/// <returns></returns>
		public static Cartoon CloneCartoon(Cartoon cartoon) => new Cartoon
		{
			CartoonId = cartoon.CartoonId,
			Name = cartoon.Name,
			Description = cartoon.Description,
			CartoonType = cartoon.CartoonType,
			CartoonUrls = cartoon.CartoonUrls,
			Checked = cartoon.Checked,
			CartoonSeasons = CloneSeasonList(cartoon.CartoonSeasons),
			CartoonWebSites = CloneWebSiteList(cartoon.CartoonWebSites)
		};

		/// <summary>
		/// Клонирование списка ссылок мультсериала
		/// </summary>
		/// <param name="cartoonUrls">Клонируемый списко ссылок мультсериала</param>
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
		/// Клонирование ссылок мультсериала
		/// </summary>
		/// <param name="cartoonUrl">Клонируемые ссылки мультсериала</param>
		/// <returns></returns>
		public static CartoonUrl CloneCartoonUrl(CartoonUrl cartoonUrl) => new CartoonUrl
		{
			CartoonUrlId = cartoonUrl.CartoonUrlId,
			Url = cartoonUrl.Url,
			WebSiteUrl = cartoonUrl.WebSiteUrl,
			Checked = cartoonUrl.Checked,
			CartoonId = cartoonUrl.CartoonId,
			CartoonWebSiteId = cartoonUrl.CartoonWebSiteId
		};

		/// <summary>
		/// Клонирование списка сезонов
		/// </summary>
		/// <param name="seasons">Клонируемый список сезонов</param>
		/// <returns></returns>
		public static List<CartoonSeason> CloneSeasonList(List<CartoonSeason> seasons)
		{
			var result = new List<CartoonSeason>();

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
		/// <param name="cartoonSeason">Клонируемый сезон</param>
		/// <returns></returns>
		public static CartoonSeason CloneSeason(CartoonSeason cartoonSeason) => new CartoonSeason
		{
			CartoonSeasonId = cartoonSeason.CartoonSeasonId,
			Name = cartoonSeason.Name,
			Number = cartoonSeason.Number,
			Description = cartoonSeason.Description,
			Checked = cartoonSeason.Checked,
			CartoonId = cartoonSeason.CartoonId,
			CartoonEpisodes = CloneEpisodeList(cartoonSeason.CartoonEpisodes)
		};

		/// <summary>
		/// Клонирование списка эпизодов
		/// </summary>
		/// <param name="episodes">Клонируемый список эпизодов</param>
		/// <returns></returns>
		public static List<CartoonEpisode> CloneEpisodeList(List<CartoonEpisode> episodes)
		{
			var result = new List<CartoonEpisode>();

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
		/// <param name="cartoonEpisode">Клонируемый эпизод</param>
		/// <returns></returns>
		public static CartoonEpisode CloneEpisode(CartoonEpisode cartoonEpisode) => new CartoonEpisode
		{
			CartoonEpisodeId = cartoonEpisode.CartoonEpisodeId,
			Name = cartoonEpisode.Name,
			Number = cartoonEpisode.Number,
			Description = cartoonEpisode.Description,
			Checked = cartoonEpisode.Checked,
			DelayedSkip = cartoonEpisode.DelayedSkip,
			SkipCount = cartoonEpisode.SkipCount,
			Duration = cartoonEpisode.Duration,
			CreditsStart = cartoonEpisode.CreditsStart,
			LastDateViewed = cartoonEpisode.LastDateViewed,
			CartoonVoiceOver = cartoonEpisode.CartoonVoiceOver,
			CartoonSeasonId = cartoonEpisode.CartoonSeasonId
		};

		/// <summary>
		/// Клонирование списка озвучек
		/// </summary>
		/// <param name="voiceOvers">Клонируемый список озвучек</param>
		/// <returns></returns>
		public static List<CartoonVoiceOver> CloneVoiceOverList(List<CartoonVoiceOver> voiceOvers)
		{
			var result = new List<CartoonVoiceOver>();

			if (voiceOvers == null || voiceOvers.Count == 0) return result;


			foreach (var voiceOver in voiceOvers)
			{
				result.Add(CloneVoiceOver(voiceOver));
			}

			return result;
		}

		/// <summary>
		/// Клонирование озвучки
		/// </summary>
		/// <param name="cartoonVoiceOver">Клонируемая озвучка</param>
		/// <returns></returns>
		public static CartoonVoiceOver CloneVoiceOver(CartoonVoiceOver cartoonVoiceOver) => new CartoonVoiceOver
		{
			CartoonVoiceOverId = cartoonVoiceOver.CartoonVoiceOverId,
			Name = cartoonVoiceOver.Name,
			Description = cartoonVoiceOver.Description,
			UrlParameter = cartoonVoiceOver.UrlParameter
		};
	}
}
