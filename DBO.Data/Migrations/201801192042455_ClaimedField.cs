namespace DBO.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClaimedField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "Claimed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companies", "Claimed");
        }
    }
}
