namespace DBO.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewsAndComments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(),
                        UserId = c.String(maxLength: 128),
                        CreatedAt = c.DateTime(nullable: false),
                        NewsId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.News", t => t.NewsId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.NewsId);
            
            CreateTable(
                "dbo.News",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        ImagePath = c.String(),
                        Description = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        UserId = c.String(maxLength: 128),
                        Views = c.Long(nullable: false),
                        CompanyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.CompanyId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Comments", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.News", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.News", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Comments", "NewsId", "dbo.News");
            DropIndex("dbo.News", new[] { "CompanyId" });
            DropIndex("dbo.News", new[] { "UserId" });
            DropIndex("dbo.Comments", new[] { "NewsId" });
            DropIndex("dbo.Comments", new[] { "UserId" });
            DropTable("dbo.News");
            DropTable("dbo.Comments");
        }
    }
}
