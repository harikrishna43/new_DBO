namespace DBO.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompanyNewFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "CVR", c => c.Int(nullable: false));
            AddColumn("dbo.Companies", "Web", c => c.String());
            AddColumn("dbo.Companies", "Email", c => c.String());
            AddColumn("dbo.Companies", "PersonName", c => c.String());
            AddColumn("dbo.Companies", "Chairman", c => c.String());
            AddColumn("dbo.Companies", "IndustryCode", c => c.Int(nullable: false));
            AddColumn("dbo.Companies", "IndustryText", c => c.String());
            AddColumn("dbo.Companies", "AdvertisingProtection", c => c.Boolean(nullable: false));
            AddColumn("dbo.Companies", "Owner", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companies", "Owner");
            DropColumn("dbo.Companies", "AdvertisingProtection");
            DropColumn("dbo.Companies", "IndustryText");
            DropColumn("dbo.Companies", "IndustryCode");
            DropColumn("dbo.Companies", "Chairman");
            DropColumn("dbo.Companies", "PersonName");
            DropColumn("dbo.Companies", "Email");
            DropColumn("dbo.Companies", "Web");
            DropColumn("dbo.Companies", "CVR");
        }
    }
}
