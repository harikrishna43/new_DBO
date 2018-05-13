namespace DBO.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserFiles : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        Type = c.Int(nullable: false),
                        FilePath = c.String(),
                        DateOfCreate = c.DateTime(nullable: false),
                        IsRemoved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserFiles", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.UserFiles", new[] { "UserId" });
            DropTable("dbo.UserFiles");
        }
    }
}
