namespace CartoonViewer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifiedJumpers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Jumpers", "Number", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Jumpers", "Number");
        }
    }
}
