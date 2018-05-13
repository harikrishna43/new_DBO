namespace DBO.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingStatusToAd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Advertisements", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Advertisements", "Status");
        }
    }
}
