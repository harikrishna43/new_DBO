namespace DBO.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Registrations : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Registrations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyId = c.Int(nullable: false),
                        Email = c.String(maxLength: 63),
                        RequestTime = c.DateTime(nullable: false),
                        ApproveTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: true)
                .Index(t => t.CompanyId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Registrations", "CompanyId", "dbo.Companies");
            DropIndex("dbo.Registrations", new[] { "CompanyId" });
            DropTable("dbo.Registrations");
        }
    }
}
