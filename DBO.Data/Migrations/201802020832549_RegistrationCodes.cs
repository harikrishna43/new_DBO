namespace DBO.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RegistrationCodes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RegistrationCodes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Generated = c.DateTime(nullable: false),
                        Registered = c.DateTime(),
                        CompanyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: true)
                .Index(t => t.CompanyId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RegistrationCodes", "CompanyId", "dbo.Companies");
            DropIndex("dbo.RegistrationCodes", new[] { "CompanyId" });
            DropTable("dbo.RegistrationCodes");
        }
    }
}
