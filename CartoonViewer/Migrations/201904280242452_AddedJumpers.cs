namespace CartoonViewer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedJumpers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Jumpers",
                c => new
                    {
                        JumperId = c.Int(nullable: false, identity: true),
                        SkipCount = c.Int(nullable: false),
                        JumperStartTime = c.Time(nullable: false, precision: 7),
                        CartoonEpisodeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.JumperId)
                .ForeignKey("dbo.CartoonEpisodes", t => t.CartoonEpisodeId, cascadeDelete: true)
                .Index(t => t.CartoonEpisodeId);
            
            DropColumn("dbo.CartoonEpisodes", "SkipCount");
            DropColumn("dbo.CartoonEpisodes", "DelayedSkip");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CartoonEpisodes", "DelayedSkip", c => c.Time(nullable: false, precision: 7));
            AddColumn("dbo.CartoonEpisodes", "SkipCount", c => c.Int(nullable: false));
            DropForeignKey("dbo.Jumpers", "CartoonEpisodeId", "dbo.CartoonEpisodes");
            DropIndex("dbo.Jumpers", new[] { "CartoonEpisodeId" });
            DropTable("dbo.Jumpers");
        }
    }
}
