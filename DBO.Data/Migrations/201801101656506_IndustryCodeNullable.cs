namespace DBO.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IndustryCodeNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Companies", "IndustryCode", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Companies", "IndustryCode", c => c.Int(nullable: false));
        }
    }
}
