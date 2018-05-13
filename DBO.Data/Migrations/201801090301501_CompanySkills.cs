namespace DBO.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompanySkills : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CompanySkills",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyId = c.Int(nullable: false),
                        SkillId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: true)
                .ForeignKey("dbo.Skills", t => t.SkillId, cascadeDelete: true)
                .Index(t => t.CompanyId)
                .Index(t => t.SkillId);
            
            CreateTable(
                "dbo.Skills",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CompanySkills", "SkillId", "dbo.Skills");
            DropForeignKey("dbo.CompanySkills", "CompanyId", "dbo.Companies");
            DropIndex("dbo.CompanySkills", new[] { "SkillId" });
            DropIndex("dbo.CompanySkills", new[] { "CompanyId" });
            DropTable("dbo.Skills");
            DropTable("dbo.CompanySkills");
        }
    }
}
