namespace DBO.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixClaimRequest : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClaimRequests", "ClaimStatus", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ClaimRequests", "ClaimStatus");
        }
    }
}
