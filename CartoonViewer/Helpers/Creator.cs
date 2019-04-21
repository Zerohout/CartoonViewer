namespace CartoonViewer.Helpers
{
	using System.Collections.Generic;
	using System.IO;
	using Models.CartoonModels;
	using ViewModels;
	using static Helper;

	public static class Creator
	{
		/// <summary>
		/// Создать список озвучек по умолчанию
		/// </summary>
		/// <returns></returns>
		public static List<CartoonVoiceOver> CreateSouthParkVoiceOverList()
		{
			return new List<CartoonVoiceOver>
			{
				new CartoonVoiceOver
				{
					Name = "MTV(Default)",
					UrlParameter = ""
				},
				new CartoonVoiceOver
				{
					Name = "Paramaunt(Default)",
					UrlParameter = ""
				},
				new CartoonVoiceOver
				{
					Name = "Paramaunt",
					UrlParameter = "?v=par"
				},
				new CartoonVoiceOver
				{
					Name = "РенТВ(Default)",
					UrlParameter = ""
				},
				new CartoonVoiceOver
				{
					Name = "VO(Default)",
					UrlParameter = ""
				},
				new CartoonVoiceOver
				{
					Name = "КвК(Default)",
					UrlParameter = ""
				},
				new CartoonVoiceOver
				{
					Name = "L0cDoG(Default)",
					UrlParameter = ""
				},
				new CartoonVoiceOver
				{
					Name = "L0cDoG",
					UrlParameter = "?v=ld"
				},
				new CartoonVoiceOver
				{
					Name = "Jaskier(Default)",
					UrlParameter = ""
				},
				new CartoonVoiceOver
				{
					Name = "Jaskier",
					UrlParameter = "?v=js"
				},
				new CartoonVoiceOver
				{
					Name = "Гоблин",
					UrlParameter = "?v=goblin"
				},
				new CartoonVoiceOver
				{
					Name = "Англ.",
					UrlParameter = "?v=en"
				}
			};
		}

		public static ElementValue CreateElementValue() => new ElementValue
		{
			ElementName = "Нижняя кнопка старта",
			CssSelector = "pjsdiv:nth-child(8) > pjsdiv > pjsdiv"
		};

		public static CartoonWebSite CreateWebSite(string url) => new CartoonWebSite
		{
			Url = url
		};

		/// <summary>
		/// Создать список мультфильмов по умолчанию
		/// </summary>
		/// <returns></returns>
		public static List<Cartoon> CreateCartoonList()
		{
			return new List<Cartoon>
			{
				new Cartoon
				{
					Name = "Южный парк",
					CartoonType = "Сериал",
					Checked = true,
					Description = "Aмериканский мультсериал, который создают Трей Паркер и Мэтт Стоун. " +
					              "Основу сюжета составляют приключения четырёх мальчиков и их друзей, " +
					              "живущих в маленьком городке Саут-Парк, штат Колорадо. " +
					              "Сериал высмеивает недостатки американской культуры и текущие мировые события, " +
					              "а также подвергает критике множество глубоких убеждений и табу " +
					              "посредством пародии и чёрного юмора. " +
								  "«Южный Парк» позиционируется как мультфильм для взрослых."
				},
				new Cartoon
				{
					Name = "Гриффины",
					CartoonType = "Сериал",
					Checked = true,
					Description = "Американский анимационный ситком, созданный Сетом Макфарлейном " +
					              "для телекомпании Fox Broadcasting Company. " +
					              "В центре сюжета неблагополучная семья Гриффинов, " +
					              "состоящая из родителей, Питера и Лоис, " +
					              "их детей, Криса, Мэг и Стьюи, а также Брайана — антропоморфного пса. " +
					              "Действие ситкома происходит в Куахоге, вымышленном пригороде Провиденса, " +
					              "штат Род-Айленд. Бо́льшая часть юмора сериала представлена в форме так называемых врезок, " +
					              "которые зачастую не имеют ничего общего с сюжетом и содержат шутки " +
					              "на различные щепетильные и спорные темы, такие как политика, " +
					              "рабство, инвалидность, феминизм, ожирение и другие."
				},
				new Cartoon
				{
					Name = "Симпсоны"
				},
				new Cartoon
				{
					Name = "Американский папаша"
				}
			};
		}

		/// <summary>
		/// Создание файла
		/// </summary>
		/// <param name="fileName">Имя файла (без расширения и указания папки)</param>
		/// <param name="fileExtension">Расширение файла (по умолчанию .cview)</param>
		/// <param name="folderPath">Путь до файла (по умолчанию WorkingData)</param>
		public static void CreateFile(string fileName, string fileExtension = null, string folderPath = null)
		{
			if (folderPath == null)
			{
				folderPath = $"{AppPath}\\{WorkingDataPath}";
			}

			if (fileExtension == null)
			{
				fileExtension = ".cview";
			}

			var path = $"{folderPath}\\{fileName}{fileExtension}";

			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}

			if (!File.Exists(path))
			{
				using (var fs = new FileStream(path, FileMode.Create))
				{
					fs.Dispose();
				}
			}
			else
			{
				if (WinMan.ShowDialog(new DialogViewModel(
										  "Файл уже существует, хотите его перезаписать?",
										  DialogState.YES_NO)) ?? false)
				{
					using (var fs = new FileStream(path, FileMode.Create))
					{
						fs.Dispose();
					}
				}
			}
		}
	}
}
