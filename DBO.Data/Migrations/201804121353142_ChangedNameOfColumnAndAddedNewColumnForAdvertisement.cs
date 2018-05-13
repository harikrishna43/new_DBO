namespace DBO.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedNameOfColumnAndAddedNewColumnForAdvertisement : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Advertisements", "Location", c => c.String());
            AddColumn("dbo.Advertisements", "LocationType", c => c.Int(nullable: false));
            DropColumn("dbo.Advertisements", "City");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Advertisements", "City", c => c.String());
            DropColumn("dbo.Advertisements", "LocationType");
            DropColumn("dbo.Advertisements", "Location");
        }
    }
}
