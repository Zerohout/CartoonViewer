namespace CartoonViewer.Helpers
{
	using System.Collections.Generic;
	using Models;

	public static class Creator
	{
		public static List<VoiceOver> CreateVoiceOvers()
		{
			return new List<VoiceOver>
			{
				new VoiceOver
				{
					Name = "MTV(Default)",
					Url = ""
				},
				new VoiceOver
				{
					Name = "Paramaunt(Default)",
					Url = ""
				},
				new VoiceOver
				{
					Name = "Paramaunt",
					Url = "?v=par"
				},
				new VoiceOver
				{
					Name = "РенТВ(Default)",
					Url = ""
				},
				new VoiceOver
				{
					Name = "VO(Default)",
					Url = ""
				},
				new VoiceOver
				{
					Name = "КвК(Default)",
					Url = ""
				},
				new VoiceOver
				{
					Name = "L0cDoG(Default)",
					Url = ""
				},
				new VoiceOver
				{
					Name = "L0cDoG",
					Url = "?v=ld"
				},
				new VoiceOver
				{
					Name = "Jaskier(Default)",
					Url = ""
				},
				new VoiceOver
				{
					Name = "Jaskier",
					Url = "?v=js"
				},
				new VoiceOver
				{
					Name = "Гоблин",
					Url = "?v=goblin"
				},
				new VoiceOver
				{
					Name = "Англ.",
					Url = "?v=en"
				}
			};
		}

		public static List<Cartoon> CreateCartoons()
		{
			return new List<Cartoon>
			{
				new Cartoon
				{
					Name = "Южный парк",
					Url = "sp"
				},
				new Cartoon
				{
					Name = "Гриффины",
					Url = "grif"
				},
				new Cartoon
				{
					Name = "Симпсоны",
					Url = "simp"
				},
				new Cartoon
				{
					Name = "Американский папаша",
					Url = "dad"
				}
			};

			
		}
	}
}
