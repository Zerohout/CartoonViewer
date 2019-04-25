namespace CartoonViewer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedVoiceOverModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CartoonVoiceOverCartoonEpisodes", "CartoonVoiceOver_CartoonVoiceOverId", "dbo.CartoonVoiceOvers");
            DropForeignKey("dbo.CartoonVoiceOverCartoonEpisodes", "CartoonEpisode_CartoonEpisodeId", "dbo.CartoonEpisodes");
            DropIndex("dbo.CartoonVoiceOverCartoonEpisodes", new[] { "CartoonVoiceOver_CartoonVoiceOverId" });
            DropIndex("dbo.CartoonVoiceOverCartoonEpisodes", new[] { "CartoonEpisode_CartoonEpisodeId" });
            AddColumn("dbo.CartoonEpisodes", "CartoonVoiceOver_CartoonVoiceOverId", c => c.Int());
            AddColumn("dbo.CartoonEpisodes", "CartoonVoiceOver_CartoonVoiceOverId1", c => c.Int());
            AddColumn("dbo.CartoonVoiceOvers", "CartoonEpisode_CartoonEpisodeId", c => c.Int());
            CreateIndex("dbo.CartoonEpisodes", "CartoonVoiceOver_CartoonVoiceOverId");
            CreateIndex("dbo.CartoonEpisodes", "CartoonVoiceOver_CartoonVoiceOverId1");
            CreateIndex("dbo.CartoonVoiceOvers", "CartoonEpisode_CartoonEpisodeId");
            AddForeignKey("dbo.CartoonEpisodes", "CartoonVoiceOver_CartoonVoiceOverId", "dbo.CartoonVoiceOvers", "CartoonVoiceOverId");
            AddForeignKey("dbo.CartoonEpisodes", "CartoonVoiceOver_CartoonVoiceOverId1", "dbo.CartoonVoiceOvers", "CartoonVoiceOverId");
            AddForeignKey("dbo.CartoonVoiceOvers", "CartoonEpisode_CartoonEpisodeId", "dbo.CartoonEpisodes", "CartoonEpisodeId");
            DropColumn("dbo.CartoonVoiceOvers", "Checked");
            DropTable("dbo.CartoonVoiceOverCartoonEpisodes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.CartoonVoiceOverCartoonEpisodes",
                c => new
                    {
                        CartoonVoiceOver_CartoonVoiceOverId = c.Int(nullable: false),
                        CartoonEpisode_CartoonEpisodeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.CartoonVoiceOver_CartoonVoiceOverId, t.CartoonEpisode_CartoonEpisodeId });
            
            AddColumn("dbo.CartoonVoiceOvers", "Checked", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.CartoonVoiceOvers", "CartoonEpisode_CartoonEpisodeId", "dbo.CartoonEpisodes");
            DropForeignKey("dbo.CartoonEpisodes", "CartoonVoiceOver_CartoonVoiceOverId1", "dbo.CartoonVoiceOvers");
            DropForeignKey("dbo.CartoonEpisodes", "CartoonVoiceOver_CartoonVoiceOverId", "dbo.CartoonVoiceOvers");
            DropIndex("dbo.CartoonVoiceOvers", new[] { "CartoonEpisode_CartoonEpisodeId" });
            DropIndex("dbo.CartoonEpisodes", new[] { "CartoonVoiceOver_CartoonVoiceOverId1" });
            DropIndex("dbo.CartoonEpisodes", new[] { "CartoonVoiceOver_CartoonVoiceOverId" });
            DropColumn("dbo.CartoonVoiceOvers", "CartoonEpisode_CartoonEpisodeId");
            DropColumn("dbo.CartoonEpisodes", "CartoonVoiceOver_CartoonVoiceOverId1");
            DropColumn("dbo.CartoonEpisodes", "CartoonVoiceOver_CartoonVoiceOverId");
            CreateIndex("dbo.CartoonVoiceOverCartoonEpisodes", "CartoonEpisode_CartoonEpisodeId");
            CreateIndex("dbo.CartoonVoiceOverCartoonEpisodes", "CartoonVoiceOver_CartoonVoiceOverId");
            AddForeignKey("dbo.CartoonVoiceOverCartoonEpisodes", "CartoonEpisode_CartoonEpisodeId", "dbo.CartoonEpisodes", "CartoonEpisodeId", cascadeDelete: true);
            AddForeignKey("dbo.CartoonVoiceOverCartoonEpisodes", "CartoonVoiceOver_CartoonVoiceOverId", "dbo.CartoonVoiceOvers", "CartoonVoiceOverId", cascadeDelete: true);
        }
    }
}
