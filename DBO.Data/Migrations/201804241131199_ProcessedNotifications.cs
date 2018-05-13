namespace DBO.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProcessedNotifications : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProcessedNotifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        NotificationId = c.Int(nullable: false),
                        DateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Notifications", t => t.NotificationId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.NotificationId);
            
            DropColumn("dbo.Notifications", "IsProcessed");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Notifications", "IsProcessed", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.ProcessedNotifications", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ProcessedNotifications", "NotificationId", "dbo.Notifications");
            DropIndex("dbo.ProcessedNotifications", new[] { "NotificationId" });
            DropIndex("dbo.ProcessedNotifications", new[] { "UserId" });
            DropTable("dbo.ProcessedNotifications");
        }
    }
}
