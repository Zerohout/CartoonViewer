using System;

namespace CartoonViewer.Database
{
	using System.Data.Entity;
	using Helpers;
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

		public DbSet<WebSite> WebSites { get; set; }
		public DbSet<ElementValue> ElementValues { get; set; }
		public DbSet<Cartoon> Cartoons { get; set; }
		public DbSet<CartoonUrl> CartoonUrl { get; set; }
		public DbSet<Season> Seasons { get; set; }
		public DbSet<Episode> Episodes { get; set; }
		public DbSet<VoiceOver> VoiceOvers { get; set; }
	}
}
