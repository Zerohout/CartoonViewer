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
		public static List<VoiceOver> CreateDefaultVoiceOvers()
		{
			return new List<VoiceOver>
			{
				new VoiceOver
				{
					Name = "MTV(Default)",
					UrlParameter = "",
					Checked = true
				},
				new VoiceOver
				{
					Name = "Paramaunt(Default)",
					UrlParameter = "",
					Checked = true
				},
				new VoiceOver
				{
					Name = "Paramaunt",
					UrlParameter = "?v=par"
				},
				new VoiceOver
				{
					Name = "РенТВ(Default)",
					UrlParameter = "",
					Checked = true
				},
				new VoiceOver
				{
					Name = "VO(Default)",
					UrlParameter = "",
					Checked = true
				},
				new VoiceOver
				{
					Name = "КвК(Default)",
					UrlParameter = "",
					Checked = true
				},
				new VoiceOver
				{
					Name = "L0cDoG(Default)",
					UrlParameter = "",
					Checked = true
				},
				new VoiceOver
				{
					Name = "L0cDoG",
					UrlParameter = "?v=ld"
				},
				new VoiceOver
				{
					Name = "Jaskier(Default)",
					UrlParameter = "",
					Checked = true
				},
				new VoiceOver
				{
					Name = "Jaskier",
					UrlParameter = "?v=js"
				},
				new VoiceOver
				{
					Name = "Гоблин",
					UrlParameter = "?v=goblin"
				},
				new VoiceOver
				{
					Name = "Англ.",
					UrlParameter = "?v=en"
				}
			};
		}

		public static List<CartoonUrl> CreateDefaultListCartoonUrl() => new List<CartoonUrl>
		{
			CreateDefaultCartoonUrl("sp"),
			CreateDefaultCartoonUrl("grif"),
			CreateDefaultCartoonUrl("simp"),
			CreateDefaultCartoonUrl("dad")
		};

		public static CartoonUrl CreateDefaultCartoonUrl(string param) => new CartoonUrl
		{
			MainUrl = $"http://{param}.freehat.cc/episode/",
			UrlParameter = "rand.php"
		};

		public static ElementValue CreateDefaultElementValue(int id) => new ElementValue
		{
			CartoonId = id,
			UserElementName = "Нижняя кнопка старта",
			CssSelector = "pjsdiv:nth-child(8) > pjsdiv > pjsdiv"
		};

		/// <summary>
		/// Создать список мультфильмов по умолчанию
		/// </summary>
		/// <returns></returns>
		public static List<Cartoon> CreateDefaultCartoons()
		{
			return new List<Cartoon>
			{
				new Cartoon
				{
					Name = "Южный парк"
				},
				new Cartoon
				{
					Name = "Гриффины"
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
