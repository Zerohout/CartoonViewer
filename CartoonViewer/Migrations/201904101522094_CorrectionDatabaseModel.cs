namespace CartoonViewer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CorrectionDatabaseModel : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.CartoonUrl", newName: "CartoonUrls");
            AddColumn("dbo.Cartoons", "CartoonType", c => c.String());
            AddColumn("dbo.CartoonUrls", "Url", c => c.String());
            DropColumn("dbo.CartoonUrls", "WebSiteUrl");
            DropColumn("dbo.CartoonUrls", "MainUrl");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CartoonUrls", "MainUrl", c => c.String());
            AddColumn("dbo.CartoonUrls", "WebSiteUrl", c => c.String());
            DropColumn("dbo.CartoonUrls", "Url");
            DropColumn("dbo.Cartoons", "CartoonType");
            RenameTable(name: "dbo.CartoonUrls", newName: "CartoonUrl");
        }
    }
}
