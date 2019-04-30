using System;

namespace CartoonViewer.Database
{
	using System.Data.Entity;
	using System.IO;
	using Models.CartoonModels;
	using static Helpers.SettingsHelper;

	public class CVDbContext : DbContext
	{
		public CVDbContext() : base("CVDb")
		{
			//AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);

			if(Directory.Exists(AppDataPath) is false)
			{
				Directory.CreateDirectory(AppDataPath);
			}

			AppDomain.CurrentDomain.SetData("DataDirectory", AppDataPath);

			Database.SetInitializer(new CreateDatabaseIfNotExists<CVDbContext>());
			//Database.SetInitializer(new DropCreateDatabaseIfModelChanges<CVDbContext>());
		}

		public CVDbContext(string path) : base("CVDb")
		{
			if (Directory.Exists(path) is false)
			{
				Directory.CreateDirectory(path);
			}

			AppDomain.CurrentDomain.SetData("DataDirectory", path);

			Database.SetInitializer(new CreateDatabaseIfNotExists<CVDbContext>());
		}

		public DbSet<CartoonWebSite> CartoonWebSites { get; set; }
		public DbSet<ElementValue> ElementValues { get; set; }
		public DbSet<Cartoon> Cartoons { get; set; }
		public DbSet<CartoonUrl> CartoonUrls { get; set; }
		public DbSet<CartoonSeason> CartoonSeasons { get; set; }
		public DbSet<CartoonEpisode> CartoonEpisodes { get; set; }
		public DbSet<EpisodeOption> EpisodeOptions { get; set; }
		public DbSet<Jumper> Jumpers { get; set; }
		public DbSet<CartoonVoiceOver> VoiceOvers { get; set; }
	}
}
