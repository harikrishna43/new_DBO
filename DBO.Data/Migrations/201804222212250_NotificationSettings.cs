namespace DBO.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NotificationSettings : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NotificationSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        OnConnectionRequest = c.Boolean(nullable: false),
                        OnConnectionAccepts = c.Boolean(nullable: false),
                        OnNewFollower = c.Boolean(nullable: false),
                        NotificationIteration = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NotificationSettings", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.NotificationSettings", new[] { "UserId" });
            DropTable("dbo.NotificationSettings");
        }
    }
}
