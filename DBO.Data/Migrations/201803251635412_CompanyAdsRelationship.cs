namespace DBO.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompanyAdsRelationship : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Advertisements", "CompanyId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Advertisements", "CompanyId");
        }
    }
}
