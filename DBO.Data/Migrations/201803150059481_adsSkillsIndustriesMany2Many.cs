namespace DBO.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adsSkillsIndustriesMany2Many : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Advertisements", "IndustryCodes");
            DropColumn("dbo.Advertisements", "Skills");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Advertisements", "Skills", c => c.String());
            AddColumn("dbo.Advertisements", "IndustryCodes", c => c.String());
        }
    }
}
