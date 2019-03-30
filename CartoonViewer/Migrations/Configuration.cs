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
			context.VoiceOvers.AddRange(Creator.CreateVoiceOvers());
			context.Cartoons.AddRange(Creator.CreateCartoons());
			context.SaveChanges();
		}
    }
}
