namespace CartoonViewer.Helpers
{
	using System.Collections.Generic;
	using Models;

	public static class Creator
	{
		public static List<VoiceOver> CreateVoiceOvers()
		{
			var result = new List<VoiceOver>();

			result.AddRange(new List<VoiceOver>
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
			});
			return result;
		}
	}
}
