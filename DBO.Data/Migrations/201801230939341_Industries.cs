namespace DBO.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Industries : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Industries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Companies", "IndustryId", c => c.Int());
            CreateIndex("dbo.Companies", "IndustryId");
            AddForeignKey("dbo.Companies", "IndustryId", "dbo.Industries", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Companies", "IndustryId", "dbo.Industries");
            DropIndex("dbo.Companies", new[] { "IndustryId" });
            DropColumn("dbo.Companies", "IndustryId");
            DropTable("dbo.Industries");
        }
    }
}
