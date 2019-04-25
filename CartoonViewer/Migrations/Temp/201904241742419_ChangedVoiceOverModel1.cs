namespace CartoonViewer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedVoiceOverModel1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CartoonEpisodes", "SelectedVoiceOverId", c => c.Int());
            AddColumn("dbo.CartoonEpisodes", "SelectedVoiceOver_CartoonVoiceOverId", c => c.Int());
            CreateIndex("dbo.CartoonEpisodes", "SelectedVoiceOver_CartoonVoiceOverId");
            AddForeignKey("dbo.CartoonEpisodes", "SelectedVoiceOver_CartoonVoiceOverId", "dbo.CartoonVoiceOvers", "CartoonVoiceOverId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CartoonEpisodes", "SelectedVoiceOver_CartoonVoiceOverId", "dbo.CartoonVoiceOvers");
            DropIndex("dbo.CartoonEpisodes", new[] { "SelectedVoiceOver_CartoonVoiceOverId" });
            DropColumn("dbo.CartoonEpisodes", "SelectedVoiceOver_CartoonVoiceOverId");
            DropColumn("dbo.CartoonEpisodes", "SelectedVoiceOverId");
        }
    }
}
