namespace DBO.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ScheduledEmails : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ScheduledEmails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyId = c.Int(nullable: false),
                        Subject = c.String(maxLength: 63),
                        Email = c.String(maxLength: 63),
                        Body = c.String(),
                        Status = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: true)
                .Index(t => t.CompanyId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ScheduledEmails", "CompanyId", "dbo.Companies");
            DropIndex("dbo.ScheduledEmails", new[] { "CompanyId" });
            DropTable("dbo.ScheduledEmails");
        }
    }
}
