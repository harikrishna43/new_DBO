namespace DBO.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDateTimeToNotification : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Notifications", "DateTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Notifications", "DateTime");
        }
    }
}
