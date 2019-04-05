namespace CartoonViewer.Migrations
{
	using System.Data.Entity.Migrations;
	using Database;

	internal sealed class Configuration : DbMigrationsConfiguration<CVDbContext>
	{
		public Configuration()
		{
			AutomaticMigrationsEnabled = true;
		}

		protected override void Seed(CVDbContext context)
		{
			//context.VoiceOvers.AddRange(Creator.CreateDefaultVoiceOvers());
			//context.Cartoons.AddRange(Creator.CreateDefaultCartoons());
			//context.SaveChanges();
		}
	}
}
