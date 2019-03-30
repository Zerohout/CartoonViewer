namespace CartoonViewer.Helpers
{
	using System.Collections.Generic;
	using Models;

	public class Creator
	{
		public void CreateVoiceOvers()
		{
			var result = new List<VoiceOver>();

			result.AddRange(new List<VoiceOver>
			{
				new VoiceOver
				{
					Name = "МТВ(Default)",
					Url = ""
				},
				new VoiceOver
				{
					Name = "Парамаунт(Default)",
					Url = ""
				},
				new VoiceOver
				{
					Name = "Парамаунт",
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
					Name = "Гоблин",
					Url = "?v=goblin"
				},
				new VoiceOver
				{
					Name = "Англ.",
					Url = "?v=en"
				},
			});
		}
	}
}
