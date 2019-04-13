namespace CartoonViewer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLastDateViewedToEpisodeModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Episodes", "LastDateViewed", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Episodes", "LastDateViewed");
        }
    }
}
