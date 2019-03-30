namespace CartoonViewer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveData : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Seasons", "CartoonId", "dbo.Cartoons");
            DropForeignKey("dbo.Episodes", "SeasonId", "dbo.Seasons");
            DropForeignKey("dbo.VoiceOvers", "EpisodeId", "dbo.Episodes");
            DropIndex("dbo.Seasons", new[] { "CartoonId" });
            DropIndex("dbo.Episodes", new[] { "SeasonId" });
            DropIndex("dbo.VoiceOvers", new[] { "EpisodeId" });
            DropTable("dbo.Cartoons");
            DropTable("dbo.Seasons");
            DropTable("dbo.Episodes");
            DropTable("dbo.VoiceOvers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.VoiceOvers",
                c => new
                    {
                        VoiceOverId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 30),
                        Url = c.String(),
                        Checked = c.Boolean(nullable: false),
                        EpisodeId = c.Int(),
                    })
                .PrimaryKey(t => t.VoiceOverId);
            
            CreateTable(
                "dbo.Episodes",
                c => new
                    {
                        EpisodeId = c.Int(nullable: false, identity: true),
                        Number = c.Int(nullable: false),
                        SkipCount = c.Int(nullable: false),
                        Name = c.String(maxLength: 100),
                        Description = c.String(maxLength: 400),
                        Checked = c.Boolean(nullable: false),
                        DelayedStart = c.Time(nullable: false, precision: 7),
                        CreditsStart = c.Time(nullable: false, precision: 7),
                        Duration = c.Time(nullable: false, precision: 7),
                        SeasonId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EpisodeId);
            
            CreateTable(
                "dbo.Seasons",
                c => new
                    {
                        SeasonId = c.Int(nullable: false, identity: true),
                        Number = c.Int(nullable: false),
                        Checked = c.Boolean(nullable: false),
                        CartoonId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SeasonId);
            
            CreateTable(
                "dbo.Cartoons",
                c => new
                    {
                        CartoonId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 30),
                        Checked = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CartoonId);
            
            CreateIndex("dbo.VoiceOvers", "EpisodeId");
            CreateIndex("dbo.Episodes", "SeasonId");
            CreateIndex("dbo.Seasons", "CartoonId");
            AddForeignKey("dbo.VoiceOvers", "EpisodeId", "dbo.Episodes", "EpisodeId");
            AddForeignKey("dbo.Episodes", "SeasonId", "dbo.Seasons", "SeasonId", cascadeDelete: true);
            AddForeignKey("dbo.Seasons", "CartoonId", "dbo.Cartoons", "CartoonId", cascadeDelete: true);
        }
    }
}
