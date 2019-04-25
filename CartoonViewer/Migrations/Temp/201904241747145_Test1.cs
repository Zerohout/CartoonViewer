namespace CartoonViewer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Test1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CartoonVoiceOvers", "CartoonEpisode_CartoonEpisodeId", "dbo.CartoonEpisodes");
            DropIndex("dbo.CartoonEpisodes", new[] { "CartoonVoiceOver_CartoonVoiceOverId" });
            DropColumn("dbo.CartoonEpisodes", "CartoonEpisodeId");
            RenameColumn(table: "dbo.CartoonEpisodes", name: "CartoonVoiceOver_CartoonVoiceOverId", newName: "__mig_tmp__0");
            RenameColumn(table: "dbo.CartoonEpisodes", name: "CartoonVoiceOver_CartoonVoiceOverId1", newName: "CartoonVoiceOver_CartoonVoiceOverId");
            RenameColumn(table: "dbo.CartoonEpisodes", name: "__mig_tmp__0", newName: "CartoonEpisodeId");
            RenameIndex(table: "dbo.CartoonEpisodes", name: "IX_CartoonVoiceOver_CartoonVoiceOverId1", newName: "IX_CartoonVoiceOver_CartoonVoiceOverId");
            DropPrimaryKey("dbo.CartoonEpisodes");
            AlterColumn("dbo.CartoonEpisodes", "CartoonEpisodeId", c => c.Int(nullable: false));
            AlterColumn("dbo.CartoonEpisodes", "CartoonEpisodeId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.CartoonEpisodes", "CartoonEpisodeId");
            CreateIndex("dbo.CartoonEpisodes", "CartoonEpisodeId");
            AddForeignKey("dbo.CartoonVoiceOvers", "CartoonEpisode_CartoonEpisodeId", "dbo.CartoonEpisodes", "CartoonEpisodeId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CartoonVoiceOvers", "CartoonEpisode_CartoonEpisodeId", "dbo.CartoonEpisodes");
            DropIndex("dbo.CartoonEpisodes", new[] { "CartoonEpisodeId" });
            DropPrimaryKey("dbo.CartoonEpisodes");
            AlterColumn("dbo.CartoonEpisodes", "CartoonEpisodeId", c => c.Int());
            AlterColumn("dbo.CartoonEpisodes", "CartoonEpisodeId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.CartoonEpisodes", "CartoonEpisodeId");
            RenameIndex(table: "dbo.CartoonEpisodes", name: "IX_CartoonVoiceOver_CartoonVoiceOverId", newName: "IX_CartoonVoiceOver_CartoonVoiceOverId1");
            RenameColumn(table: "dbo.CartoonEpisodes", name: "CartoonEpisodeId", newName: "__mig_tmp__0");
            RenameColumn(table: "dbo.CartoonEpisodes", name: "CartoonVoiceOver_CartoonVoiceOverId", newName: "CartoonVoiceOver_CartoonVoiceOverId1");
            RenameColumn(table: "dbo.CartoonEpisodes", name: "__mig_tmp__0", newName: "CartoonVoiceOver_CartoonVoiceOverId");
            AddColumn("dbo.CartoonEpisodes", "CartoonEpisodeId", c => c.Int(nullable: false, identity: true));
            CreateIndex("dbo.CartoonEpisodes", "CartoonVoiceOver_CartoonVoiceOverId");
            AddForeignKey("dbo.CartoonVoiceOvers", "CartoonEpisode_CartoonEpisodeId", "dbo.CartoonEpisodes", "CartoonEpisodeId");
        }
    }
}
