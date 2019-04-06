namespace CartoonViewer.Migrations
{
	using System.Data.Entity.Migrations;
	using Database;
	using Helpers;

	internal sealed class Configuration : DbMigrationsConfiguration<CVDbContext>
	{
		public Configuration()
		{
			AutomaticMigrationsEnabled = true;
		}

		protected override void Seed(CVDbContext context)
		{
			context.VoiceOvers.AddRange(Creator.CreateDefaultVoiceOvers());
			context.Cartoons.AddRange(Creator.CreateDefaultCartoons());
			context.SaveChanges();

			FillCartoons(context);
			context.SaveChanges();
		}

		private void FillCartoons(CVDbContext context)
		{
			foreach (var c in context.Cartoons)
			{
				switch (c.Name)
				{
					case "����� ����":
						c.CartoonUrl = Creator.CreateDefaultCartoonUrl("sp");
						break;
					case "��������":
						c.CartoonUrl = Creator.CreateDefaultCartoonUrl("grif");
						break;
					case "��������":
						c.CartoonUrl = Creator.CreateDefaultCartoonUrl("simp");
						break;
					case "������������ ������":
						c.CartoonUrl = Creator.CreateDefaultCartoonUrl("dad");
						break;
				}

				c.ElementValues.Add(Creator.CreateDefaultElementValue(c.CartoonId));
			}
		}
	}
}
