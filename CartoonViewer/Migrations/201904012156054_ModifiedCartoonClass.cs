namespace CartoonViewer.Migrations
{
	using System.Data.Entity.Migrations;

	public partial class ModifiedCartoonClass : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.Cartoons", "Url", c => c.String());
		}

		public override void Down()
		{
			DropColumn("dbo.Cartoons", "Url");
		}
	}
}
