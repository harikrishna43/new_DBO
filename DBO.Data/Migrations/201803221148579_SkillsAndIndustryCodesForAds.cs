namespace DBO.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SkillsAndIndustryCodesForAds : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AdsIndustries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IndustryId = c.Int(nullable: false),
                        AdvertisemenId = c.Int(nullable: false),
                        Advertisement_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Advertisements", t => t.Advertisement_Id)
                .ForeignKey("dbo.Industries", t => t.IndustryId, cascadeDelete: true)
                .Index(t => t.IndustryId)
                .Index(t => t.Advertisement_Id);
            
            CreateTable(
                "dbo.AdsSkills",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SkillId = c.Int(nullable: false),
                        AdvertisementId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Advertisements", t => t.AdvertisementId, cascadeDelete: true)
                .ForeignKey("dbo.Skills", t => t.SkillId, cascadeDelete: true)
                .Index(t => t.SkillId)
                .Index(t => t.AdvertisementId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AdsSkills", "SkillId", "dbo.Skills");
            DropForeignKey("dbo.AdsSkills", "AdvertisementId", "dbo.Advertisements");
            DropForeignKey("dbo.AdsIndustries", "IndustryId", "dbo.Industries");
            DropForeignKey("dbo.AdsIndustries", "Advertisement_Id", "dbo.Advertisements");
            DropIndex("dbo.AdsSkills", new[] { "AdvertisementId" });
            DropIndex("dbo.AdsSkills", new[] { "SkillId" });
            DropIndex("dbo.AdsIndustries", new[] { "Advertisement_Id" });
            DropIndex("dbo.AdsIndustries", new[] { "IndustryId" });
            DropTable("dbo.AdsSkills");
            DropTable("dbo.AdsIndustries");
        }
    }
}
