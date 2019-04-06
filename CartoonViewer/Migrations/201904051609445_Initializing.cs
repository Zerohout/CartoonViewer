namespace CartoonViewer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initializing : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cartoons",
                c => new
                    {
                        CartoonId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Checked = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CartoonId);
            
            CreateTable(
                "dbo.CartoonUrl",
                c => new
                    {
                        CartoonId = c.Int(nullable: false),
                        MainUrl = c.String(),
                        AdditionalUrl = c.String(),
                        UrlParameter = c.String(),
                        AdditionalUrlParameter = c.String(),
                    })
                .PrimaryKey(t => t.CartoonId)
                .ForeignKey("dbo.Cartoons", t => t.CartoonId)
                .Index(t => t.CartoonId);
            
            CreateTable(
                "dbo.ElementValues",
                c => new
                    {
                        ElementValueId = c.Int(nullable: false, identity: true),
                        UserElementName = c.String(),
                        Id = c.String(),
                        Name = c.String(),
                        ClassName = c.String(),
                        TagName = c.String(),
                        CssSelector = c.String(),
                        LinkText = c.String(),
                        PartialLinkText = c.String(),
                        XPath = c.String(),
                        CartoonId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ElementValueId)
                .ForeignKey("dbo.Cartoons", t => t.CartoonId, cascadeDelete: true)
                .Index(t => t.CartoonId);
            
            CreateTable(
                "dbo.Seasons",
                c => new
                    {
                        SeasonId = c.Int(nullable: false, identity: true),
                        Number = c.Int(nullable: false),
                        Checked = c.Boolean(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        CartoonId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SeasonId)
                .ForeignKey("dbo.Cartoons", t => t.CartoonId, cascadeDelete: true)
                .Index(t => t.CartoonId);
            
            CreateTable(
                "dbo.Episodes",
                c => new
                    {
                        EpisodeId = c.Int(nullable: false, identity: true),
                        Number = c.Int(nullable: false),
                        SkipCount = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        Checked = c.Boolean(nullable: false),
                        DelayedSkip = c.Time(nullable: false, precision: 7),
                        CreditsStart = c.Time(nullable: false, precision: 7),
                        Duration = c.Time(nullable: false, precision: 7),
                        SeasonId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EpisodeId)
                .ForeignKey("dbo.Seasons", t => t.SeasonId, cascadeDelete: true)
                .Index(t => t.SeasonId);
            
            CreateTable(
                "dbo.VoiceOvers",
                c => new
                    {
                        VoiceOverId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        UrlParameter = c.String(),
                        Checked = c.Boolean(nullable: false),
                        EpisodeId = c.Int(),
                    })
                .PrimaryKey(t => t.VoiceOverId)
                .ForeignKey("dbo.Episodes", t => t.EpisodeId)
                .Index(t => t.EpisodeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VoiceOvers", "EpisodeId", "dbo.Episodes");
            DropForeignKey("dbo.Episodes", "SeasonId", "dbo.Seasons");
            DropForeignKey("dbo.Seasons", "CartoonId", "dbo.Cartoons");
            DropForeignKey("dbo.ElementValues", "CartoonId", "dbo.Cartoons");
            DropForeignKey("dbo.CartoonUrl", "CartoonId", "dbo.Cartoons");
            DropIndex("dbo.VoiceOvers", new[] { "EpisodeId" });
            DropIndex("dbo.Episodes", new[] { "SeasonId" });
            DropIndex("dbo.Seasons", new[] { "CartoonId" });
            DropIndex("dbo.ElementValues", new[] { "CartoonId" });
            DropIndex("dbo.CartoonUrl", new[] { "CartoonId" });
            DropTable("dbo.VoiceOvers");
            DropTable("dbo.Episodes");
            DropTable("dbo.Seasons");
            DropTable("dbo.ElementValues");
            DropTable("dbo.CartoonUrl");
            DropTable("dbo.Cartoons");
        }
    }
}
