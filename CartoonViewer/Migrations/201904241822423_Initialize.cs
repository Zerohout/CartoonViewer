namespace CartoonViewer.Migrations
{
	using System.Data.Entity.Migrations;

	public partial class Initialize : DbMigration
	{
		public override void Up()
		{
			CreateTable(
				"dbo.CartoonEpisodes",
				c => new
				{
					CartoonEpisodeId = c.Int(nullable: false, identity: true),
					Number = c.Int(nullable: false),
					SkipCount = c.Int(nullable: false),
					Name = c.String(),
					Description = c.String(),
					Checked = c.Boolean(nullable: false),
					DelayedSkip = c.Time(nullable: false, precision: 7),
					CreditsStart = c.Time(nullable: false, precision: 7),
					Duration = c.Time(nullable: false, precision: 7),
					LastDateViewed = c.DateTime(nullable: false),
					CartoonSeasonId = c.Int(nullable: false),
					CartoonVoiceOver_CartoonVoiceOverId = c.Int(),
				})
				.PrimaryKey(t => t.CartoonEpisodeId)
				.ForeignKey("dbo.CartoonSeasons", t => t.CartoonSeasonId, cascadeDelete: true)
				.ForeignKey("dbo.CartoonVoiceOvers", t => t.CartoonVoiceOver_CartoonVoiceOverId)
				.Index(t => t.CartoonSeasonId)
				.Index(t => t.CartoonVoiceOver_CartoonVoiceOverId);

			CreateTable(
				"dbo.CartoonSeasons",
				c => new
				{
					CartoonSeasonId = c.Int(nullable: false, identity: true),
					Number = c.Int(nullable: false),
					Checked = c.Boolean(nullable: false),
					Name = c.String(),
					Description = c.String(),
					CartoonId = c.Int(nullable: false),
				})
				.PrimaryKey(t => t.CartoonSeasonId)
				.ForeignKey("dbo.Cartoons", t => t.CartoonId, cascadeDelete: true)
				.Index(t => t.CartoonId);

			CreateTable(
				"dbo.Cartoons",
				c => new
				{
					CartoonId = c.Int(nullable: false, identity: true),
					Name = c.String(),
					Description = c.String(),
					CartoonType = c.String(),
					Checked = c.Boolean(nullable: false),
				})
				.PrimaryKey(t => t.CartoonId);

			CreateTable(
				"dbo.CartoonUrls",
				c => new
				{
					CartoonUrlId = c.Int(nullable: false, identity: true),
					Url = c.String(),
					WebSiteUrl = c.String(),
					Checked = c.Boolean(nullable: false),
					CartoonWebSiteId = c.Int(nullable: false),
					CartoonId = c.Int(nullable: false),
				})
				.PrimaryKey(t => t.CartoonUrlId)
				.ForeignKey("dbo.Cartoons", t => t.CartoonId, cascadeDelete: true)
				.ForeignKey("dbo.CartoonWebSites", t => t.CartoonWebSiteId, cascadeDelete: true)
				.Index(t => t.CartoonWebSiteId)
				.Index(t => t.CartoonId);

			CreateTable(
				"dbo.CartoonWebSites",
				c => new
				{
					CartoonWebSiteId = c.Int(nullable: false, identity: true),
					Url = c.String(),
				})
				.PrimaryKey(t => t.CartoonWebSiteId);

			CreateTable(
				"dbo.ElementValues",
				c => new
				{
					ElementValueId = c.Int(nullable: false, identity: true),
					ElementName = c.String(),
					Id = c.String(),
					Name = c.String(),
					ClassName = c.String(),
					TagName = c.String(),
					CssSelector = c.String(),
					LinkText = c.String(),
					PartialLinkText = c.String(),
					XPath = c.String(),
					CartoonWebSiteId = c.Int(nullable: false),
				})
				.PrimaryKey(t => t.ElementValueId)
				.ForeignKey("dbo.CartoonWebSites", t => t.CartoonWebSiteId, cascadeDelete: true)
				.Index(t => t.CartoonWebSiteId);

			CreateTable(
				"dbo.CartoonVoiceOvers",
				c => new
				{
					CartoonVoiceOverId = c.Int(nullable: false, identity: true),
					Name = c.String(),
					Description = c.String(),
					UrlParameter = c.String(),
				})
				.PrimaryKey(t => t.CartoonVoiceOverId);

			CreateTable(
				"dbo.CartoonWebSiteCartoons",
				c => new
				{
					CartoonWebSite_CartoonWebSiteId = c.Int(nullable: false),
					Cartoon_CartoonId = c.Int(nullable: false),
				})
				.PrimaryKey(t => new { t.CartoonWebSite_CartoonWebSiteId, t.Cartoon_CartoonId })
				.ForeignKey("dbo.CartoonWebSites", t => t.CartoonWebSite_CartoonWebSiteId, cascadeDelete: true)
				.ForeignKey("dbo.Cartoons", t => t.Cartoon_CartoonId, cascadeDelete: true)
				.Index(t => t.CartoonWebSite_CartoonWebSiteId)
				.Index(t => t.Cartoon_CartoonId);

			CreateTable(
				"dbo.CartoonVoiceOverCartoonEpisodes",
				c => new
				{
					CartoonVoiceOver_CartoonVoiceOverId = c.Int(nullable: false),
					CartoonEpisode_CartoonEpisodeId = c.Int(nullable: false),
				})
				.PrimaryKey(t => new { t.CartoonVoiceOver_CartoonVoiceOverId, t.CartoonEpisode_CartoonEpisodeId })
				.ForeignKey("dbo.CartoonVoiceOvers", t => t.CartoonVoiceOver_CartoonVoiceOverId, cascadeDelete: true)
				.ForeignKey("dbo.CartoonEpisodes", t => t.CartoonEpisode_CartoonEpisodeId, cascadeDelete: true)
				.Index(t => t.CartoonVoiceOver_CartoonVoiceOverId)
				.Index(t => t.CartoonEpisode_CartoonEpisodeId);

			CreateTable(
				"dbo.CartoonVoiceOverCartoons",
				c => new
				{
					CartoonVoiceOver_CartoonVoiceOverId = c.Int(nullable: false),
					Cartoon_CartoonId = c.Int(nullable: false),
				})
				.PrimaryKey(t => new { t.CartoonVoiceOver_CartoonVoiceOverId, t.Cartoon_CartoonId })
				.ForeignKey("dbo.CartoonVoiceOvers", t => t.CartoonVoiceOver_CartoonVoiceOverId, cascadeDelete: true)
				.ForeignKey("dbo.Cartoons", t => t.Cartoon_CartoonId, cascadeDelete: true)
				.Index(t => t.CartoonVoiceOver_CartoonVoiceOverId)
				.Index(t => t.Cartoon_CartoonId);

		}

		public override void Down()
		{
			DropForeignKey("dbo.CartoonEpisodes", "CartoonVoiceOver_CartoonVoiceOverId", "dbo.CartoonVoiceOvers");
			DropForeignKey("dbo.CartoonEpisodes", "CartoonSeasonId", "dbo.CartoonSeasons");
			DropForeignKey("dbo.CartoonSeasons", "CartoonId", "dbo.Cartoons");
			DropForeignKey("dbo.CartoonVoiceOverCartoons", "Cartoon_CartoonId", "dbo.Cartoons");
			DropForeignKey("dbo.CartoonVoiceOverCartoons", "CartoonVoiceOver_CartoonVoiceOverId", "dbo.CartoonVoiceOvers");
			DropForeignKey("dbo.CartoonVoiceOverCartoonEpisodes", "CartoonEpisode_CartoonEpisodeId", "dbo.CartoonEpisodes");
			DropForeignKey("dbo.CartoonVoiceOverCartoonEpisodes", "CartoonVoiceOver_CartoonVoiceOverId", "dbo.CartoonVoiceOvers");
			DropForeignKey("dbo.CartoonUrls", "CartoonWebSiteId", "dbo.CartoonWebSites");
			DropForeignKey("dbo.ElementValues", "CartoonWebSiteId", "dbo.CartoonWebSites");
			DropForeignKey("dbo.CartoonWebSiteCartoons", "Cartoon_CartoonId", "dbo.Cartoons");
			DropForeignKey("dbo.CartoonWebSiteCartoons", "CartoonWebSite_CartoonWebSiteId", "dbo.CartoonWebSites");
			DropForeignKey("dbo.CartoonUrls", "CartoonId", "dbo.Cartoons");
			DropIndex("dbo.CartoonVoiceOverCartoons", new[] { "Cartoon_CartoonId" });
			DropIndex("dbo.CartoonVoiceOverCartoons", new[] { "CartoonVoiceOver_CartoonVoiceOverId" });
			DropIndex("dbo.CartoonVoiceOverCartoonEpisodes", new[] { "CartoonEpisode_CartoonEpisodeId" });
			DropIndex("dbo.CartoonVoiceOverCartoonEpisodes", new[] { "CartoonVoiceOver_CartoonVoiceOverId" });
			DropIndex("dbo.CartoonWebSiteCartoons", new[] { "Cartoon_CartoonId" });
			DropIndex("dbo.CartoonWebSiteCartoons", new[] { "CartoonWebSite_CartoonWebSiteId" });
			DropIndex("dbo.ElementValues", new[] { "CartoonWebSiteId" });
			DropIndex("dbo.CartoonUrls", new[] { "CartoonId" });
			DropIndex("dbo.CartoonUrls", new[] { "CartoonWebSiteId" });
			DropIndex("dbo.CartoonSeasons", new[] { "CartoonId" });
			DropIndex("dbo.CartoonEpisodes", new[] { "CartoonVoiceOver_CartoonVoiceOverId" });
			DropIndex("dbo.CartoonEpisodes", new[] { "CartoonSeasonId" });
			DropTable("dbo.CartoonVoiceOverCartoons");
			DropTable("dbo.CartoonVoiceOverCartoonEpisodes");
			DropTable("dbo.CartoonWebSiteCartoons");
			DropTable("dbo.CartoonVoiceOvers");
			DropTable("dbo.ElementValues");
			DropTable("dbo.CartoonWebSites");
			DropTable("dbo.CartoonUrls");
			DropTable("dbo.Cartoons");
			DropTable("dbo.CartoonSeasons");
			DropTable("dbo.CartoonEpisodes");
		}
	}
}
