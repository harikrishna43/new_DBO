namespace DBO.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompanyConnectionsAndFollowers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Connections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyId1 = c.Int(nullable: false),
                        CompanyId2 = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId1, cascadeDelete: false)
                .ForeignKey("dbo.Companies", t => t.CompanyId2, cascadeDelete: false)
                .Index(t => t.CompanyId1)
                .Index(t => t.CompanyId2);
            
            CreateTable(
                "dbo.Followers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyId = c.Int(nullable: false),
                        UserId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Connections", "CompanyId2", "dbo.Companies");
            DropForeignKey("dbo.Connections", "CompanyId1", "dbo.Companies");
            DropIndex("dbo.Connections", new[] { "CompanyId2" });
            DropIndex("dbo.Connections", new[] { "CompanyId1" });
            DropTable("dbo.Followers");
            DropTable("dbo.Connections");
        }
    }
}
