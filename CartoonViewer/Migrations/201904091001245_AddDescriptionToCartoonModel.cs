namespace CartoonViewer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDescriptionToCartoonModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cartoons", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Cartoons", "Description");
        }
    }
}
