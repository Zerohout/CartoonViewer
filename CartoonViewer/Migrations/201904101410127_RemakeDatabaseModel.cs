namespace CartoonViewer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemakeDatabaseModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ElementValues", "CartoonId", "dbo.Cartoons");
            DropForeignKey("dbo.CartoonUrl", "CartoonId", "dbo.Cartoons");
            DropIndex("dbo.ElementValues", new[] { "CartoonId" });
            DropPrimaryKey("dbo.CartoonUrl");
            CreateTable(
                "dbo.WebSites",
                c => new
                    {
                        WebSiteId = c.Int(nullable: false, identity: true),
                        Url = c.String(),
                    })
                .PrimaryKey(t => t.WebSiteId);
            
            CreateTable(
                "dbo.WebSiteCartoons",
                c => new
                    {
                        WebSite_WebSiteId = c.Int(nullable: false),
                        Cartoon_CartoonId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.WebSite_WebSiteId, t.Cartoon_CartoonId })
                .ForeignKey("dbo.WebSites", t => t.WebSite_WebSiteId, cascadeDelete: true)
                .ForeignKey("dbo.Cartoons", t => t.Cartoon_CartoonId, cascadeDelete: true)
                .Index(t => t.WebSite_WebSiteId)
                .Index(t => t.Cartoon_CartoonId);
            
            AddColumn("dbo.CartoonUrl", "CartoonUrlId", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.CartoonUrl", "WebSiteUrl", c => c.String());
            AddColumn("dbo.CartoonUrl", "WebSiteId", c => c.Int(nullable: false));
            AddColumn("dbo.ElementValues", "ElementName", c => c.String());
            AddColumn("dbo.ElementValues", "WebSiteId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.CartoonUrl", "CartoonUrlId");
            CreateIndex("dbo.CartoonUrl", "WebSiteId");
            CreateIndex("dbo.ElementValues", "WebSiteId");
            AddForeignKey("dbo.ElementValues", "WebSiteId", "dbo.WebSites", "WebSiteId", cascadeDelete: true);
            AddForeignKey("dbo.CartoonUrl", "WebSiteId", "dbo.WebSites", "WebSiteId", cascadeDelete: true);
            AddForeignKey("dbo.CartoonUrl", "CartoonId", "dbo.Cartoons", "CartoonId", cascadeDelete: true);
            DropColumn("dbo.CartoonUrl", "AdditionalUrl");
            DropColumn("dbo.CartoonUrl", "UrlParameter");
            DropColumn("dbo.CartoonUrl", "AdditionalUrlParameter");
            DropColumn("dbo.ElementValues", "UserElementName");
            DropColumn("dbo.ElementValues", "CartoonId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ElementValues", "CartoonId", c => c.Int(nullable: false));
            AddColumn("dbo.ElementValues", "UserElementName", c => c.String());
            AddColumn("dbo.CartoonUrl", "AdditionalUrlParameter", c => c.String());
            AddColumn("dbo.CartoonUrl", "UrlParameter", c => c.String());
            AddColumn("dbo.CartoonUrl", "AdditionalUrl", c => c.String());
            DropForeignKey("dbo.CartoonUrl", "CartoonId", "dbo.Cartoons");
            DropForeignKey("dbo.CartoonUrl", "WebSiteId", "dbo.WebSites");
            DropForeignKey("dbo.ElementValues", "WebSiteId", "dbo.WebSites");
            DropForeignKey("dbo.WebSiteCartoons", "Cartoon_CartoonId", "dbo.Cartoons");
            DropForeignKey("dbo.WebSiteCartoons", "WebSite_WebSiteId", "dbo.WebSites");
            DropIndex("dbo.WebSiteCartoons", new[] { "Cartoon_CartoonId" });
            DropIndex("dbo.WebSiteCartoons", new[] { "WebSite_WebSiteId" });
            DropIndex("dbo.ElementValues", new[] { "WebSiteId" });
            DropIndex("dbo.CartoonUrl", new[] { "WebSiteId" });
            DropPrimaryKey("dbo.CartoonUrl");
            DropColumn("dbo.ElementValues", "WebSiteId");
            DropColumn("dbo.ElementValues", "ElementName");
            DropColumn("dbo.CartoonUrl", "WebSiteId");
            DropColumn("dbo.CartoonUrl", "WebSiteUrl");
            DropColumn("dbo.CartoonUrl", "CartoonUrlId");
            DropTable("dbo.WebSiteCartoons");
            DropTable("dbo.WebSites");
            AddPrimaryKey("dbo.CartoonUrl", "CartoonId");
            CreateIndex("dbo.ElementValues", "CartoonId");
            AddForeignKey("dbo.CartoonUrl", "CartoonId", "dbo.Cartoons", "CartoonId");
            AddForeignKey("dbo.ElementValues", "CartoonId", "dbo.Cartoons", "CartoonId", cascadeDelete: true);
        }
    }
}
