namespace CartoonViewer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifiedJumper : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Jumpers", "SkipCount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Jumpers", "SkipCount", c => c.Int());
        }
    }
}
