namespace DBO.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateAdvertisementModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Advertisements", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Advertisements", "UserId");
            AddForeignKey("dbo.Advertisements", "UserId", "dbo.AspNetUsers", "Id");
            DropColumn("dbo.Advertisements", "CompanyId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Advertisements", "CompanyId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Advertisements", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Advertisements", new[] { "UserId" });
            DropColumn("dbo.Advertisements", "UserId");
        }
    }
}
