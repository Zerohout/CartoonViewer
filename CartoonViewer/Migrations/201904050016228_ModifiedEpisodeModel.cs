namespace CartoonViewer.Migrations
{
	using System.Data.Entity.Migrations;

	public partial class ModifiedEpisodeModel : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.Episodes", "DelayedSkip", c => c.Time(nullable: false, precision: 7));
			DropColumn("dbo.Episodes", "DelayedStart");
		}

		public override void Down()
		{
			AddColumn("dbo.Episodes", "DelayedStart", c => c.Time(nullable: false, precision: 7));
			DropColumn("dbo.Episodes", "DelayedSkip");
		}
	}
}
