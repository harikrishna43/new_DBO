namespace DBO.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAdvertisements : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Advertisements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Headline = c.String(maxLength: 50),
                        Text = c.String(maxLength: 200),
                        Link = c.String(),
                        ImagePath = c.String(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        ClicksCount = c.Int(nullable: false),
                        City = c.String(),
                        IndustryCodes = c.String(),
                        Skills = c.String(),
                        MyProperty = c.Int(nullable: false),
                        ApearOnLogin = c.Boolean(nullable: false),
                        ApearOnLogout = c.Boolean(nullable: false),
                        ApearForPrivatePerson = c.Boolean(nullable: false),
                        ApearForCompany = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Advertisements");
        }
    }
}
