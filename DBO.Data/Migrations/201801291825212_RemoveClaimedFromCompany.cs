namespace DBO.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveClaimedFromCompany : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Companies", "Claimed");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Companies", "Claimed", c => c.Boolean(nullable: false));
        }
    }
}
