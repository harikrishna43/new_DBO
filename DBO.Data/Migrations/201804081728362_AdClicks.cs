namespace DBO.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdClicks : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AdClicks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        AdvertisementId = c.Int(nullable: false),
                        IpAddress = c.String(maxLength: 15),
                        DateTime = c.DateTime(nullable: false),
                        Charged = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Advertisements", t => t.AdvertisementId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.AdvertisementId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AdClicks", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AdClicks", "AdvertisementId", "dbo.Advertisements");
            DropIndex("dbo.AdClicks", new[] { "AdvertisementId" });
            DropIndex("dbo.AdClicks", new[] { "UserId" });
            DropTable("dbo.AdClicks");
        }
    }
}
