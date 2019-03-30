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
                "dbo.Seasons",
                c => new
                    {
                        SeasonId = c.Int(nullable: false, identity: true),
                        Number = c.Int(nullable: false),
                        Checked = c.Boolean(nullable: false),
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
                        DelayedStart = c.Time(nullable: false, precision: 7),
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
                        Url = c.String(),
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
            DropIndex("dbo.VoiceOvers", new[] { "EpisodeId" });
            DropIndex("dbo.Episodes", new[] { "SeasonId" });
            DropIndex("dbo.Seasons", new[] { "CartoonId" });
            DropTable("dbo.VoiceOvers");
            DropTable("dbo.Episodes");
            DropTable("dbo.Seasons");
            DropTable("dbo.Cartoons");
        }
    }
}
