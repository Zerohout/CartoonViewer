using System;

namespace CartoonViewer.Database
{
	using System.Data.Entity;
	using Models.CartoonModels;

	public class CVDbContext : DbContext
	{
		public CVDbContext() : base("CVDb")
		{
			AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);

			Database.SetInitializer(new CreateDatabaseIfNotExists<CVDbContext>());
			
			//Database.SetInitializer(new DropCreateDatabaseAlways<CVDbContext>());
			

			//Database.SetInitializer(new DropCreateDatabaseIfModelChanges<CVDbContext>());
		}

		public CVDbContext(string path) : base("CVDb")
		{
			AppDomain.CurrentDomain.SetData("DataDirectory", path);

		}

		public DbSet<CartoonWebSite> CartoonWebSites { get; set; }
		public DbSet<ElementValue> ElementValues { get; set; }
		public DbSet<Cartoon> Cartoons { get; set; }
		public DbSet<CartoonUrl> CartoonUrl { get; set; }
		public DbSet<CartoonSeason> CartoonSeasons { get; set; }
		public DbSet<CartoonEpisode> CartoonEpisodes { get; set; }
		public DbSet<CartoonVoiceOver> VoiceOvers { get; set; }
	}
}
