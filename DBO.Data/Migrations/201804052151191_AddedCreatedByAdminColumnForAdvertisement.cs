namespace DBO.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCreatedByAdminColumnForAdvertisement : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Advertisements", "CreatedByAdmin", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Advertisements", "CreatedByAdmin");
        }
    }
}
