namespace DBO.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PhoneAndEmailIndexes : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Companies", "Phone");
            CreateIndex("dbo.Companies", "Email");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Companies", new[] { "Email" });
            DropIndex("dbo.Companies", new[] { "Phone" });
        }
    }
}
