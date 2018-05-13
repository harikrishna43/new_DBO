namespace DBO.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateAds : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AdsIndustries", "Advertisement_Id", "dbo.Advertisements");
            DropIndex("dbo.AdsIndustries", new[] { "Advertisement_Id" });
            DropColumn("dbo.AdsIndustries", "AdvertisemenId");
            RenameColumn(table: "dbo.AdsIndustries", name: "Advertisement_Id", newName: "AdvertisemenId");
            AddColumn("dbo.Advertisements", "IsFullWidth", c => c.Boolean(nullable: false));
            AddColumn("dbo.Advertisements", "ClickPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Advertisements", "Budget", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Advertisements", "BudgetSpent", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.AdsIndustries", "AdvertisemenId", c => c.Int(nullable: false));
            CreateIndex("dbo.AdsIndustries", "AdvertisemenId");
            AddForeignKey("dbo.AdsIndustries", "AdvertisemenId", "dbo.Advertisements", "Id", cascadeDelete: true);
            DropColumn("dbo.Advertisements", "MyProperty");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Advertisements", "MyProperty", c => c.Int(nullable: false));
            DropForeignKey("dbo.AdsIndustries", "AdvertisemenId", "dbo.Advertisements");
            DropIndex("dbo.AdsIndustries", new[] { "AdvertisemenId" });
            AlterColumn("dbo.AdsIndustries", "AdvertisemenId", c => c.Int());
            DropColumn("dbo.Advertisements", "BudgetSpent");
            DropColumn("dbo.Advertisements", "Budget");
            DropColumn("dbo.Advertisements", "ClickPrice");
            DropColumn("dbo.Advertisements", "IsFullWidth");
            RenameColumn(table: "dbo.AdsIndustries", name: "AdvertisemenId", newName: "Advertisement_Id");
            AddColumn("dbo.AdsIndustries", "AdvertisemenId", c => c.Int(nullable: false));
            CreateIndex("dbo.AdsIndustries", "Advertisement_Id");
            AddForeignKey("dbo.AdsIndustries", "Advertisement_Id", "dbo.Advertisements", "Id");
        }
    }
}
