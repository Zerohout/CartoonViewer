using System;

namespace CartoonViewer.Database
{
	using System.Data.Entity;
	using Models;

	public class CVDbContext : DbContext
	{
		public CVDbContext():base("CVDb")
		{
			AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);

			Database.CreateIfNotExists();

			//Database.SetInitializer(new DropCreateDatabaseAlways<FWUDbContext>());
			Database.SetInitializer(new DropCreateDatabaseIfModelChanges<CVDbContext>());
		}

		public DbSet<Cartoon> Cartoons { get; set; }
		public DbSet<Season> Seasons { get; set; }
		public DbSet<Episode> Episodes { get; set; }
		public DbSet<VoiceOver> VoiceOvers { get; set; }
	}
}
